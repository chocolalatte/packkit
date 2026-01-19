using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Godot;
using Microsoft.VisualBasic;
using Packkit.Manifest;
using Tomlyn;
using Tomlyn.Model;

namespace Packkit.Core;

public static class Parser
{
    public static ModEntry ParseForge(ZipArchiveEntry modsToml, string filePath)
    {
        try
        {
            using StreamReader reader = new(modsToml.Open());
            string content = reader.ReadToEnd();

            TomlTable model = Toml.ToModel(content);

            TomlTableArray modsArray =
                model.TryGetValue("mods", out var modsObj) && modsObj is TomlTableArray array
                    ? array
                    : null;

            // Uses fallback if there is no mods field or if there are no mods in the field
            TomlTable mod =
                (modsArray?.FirstOrDefault())
                ?? throw new Exception(
                    "[PARSER] [ERROR-001] No \"mods\" field found in TOML, using fallback"
                );
            ModEntry modEntry = CreateForgeModEntry(mod, filePath);

            AddForgeDependencies(modEntry, model);
            return modEntry;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PARSER] [ERROR-002] : {ex.Message}");
            return CreateForgeFallbackModEntry(filePath);
        }
    }

    private static void AddForgeDependencies(ModEntry modEntry, TomlTable model)
    {
        if (
            model.TryGetValue("dependencies", out var dependenciesObject)
            && dependenciesObject is TomlTable dependenciesTable
            && modEntry.ModId != null
            && dependenciesTable.TryGetValue(modEntry.ModId, out var modDependenciesObject)
            && modDependenciesObject is TomlTableArray modDependenciesArray
        )
        {
            foreach (TomlTable dependency in modDependenciesArray)
            {
                string dependencyId = dependency.TryGetValue("modId", out var dependencyIdObject)
                    ? dependencyIdObject.ToString()
                    : null;
                bool mandatory =
                    dependency.TryGetValue("mandatory", out var mandatoryObject)
                    && mandatoryObject
                        .ToString()
                        ?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

                if (!string.IsNullOrEmpty(dependencyId))
                {
                    if (mandatory)
                        modEntry.Requires.Add(dependencyId);
                    else
                        modEntry.Recommends.Add(dependencyId);
                }
            }
        }
    }

    private static ModEntry CreateForgeFallbackModEntry(string filePath) =>
        new()
        {
            ModId = Path.GetFileNameWithoutExtension(filePath),
            Name = Path.GetFileNameWithoutExtension(filePath),
            File = Path.GetFileName(filePath),
            Version = "unknown",
            Loader = ModLoader.forge,
            Side = ModSide.unknown,
        };

    private static ModEntry CreateForgeModEntry(TomlTable mod, string filePath) =>
        new()
        {
            ModId = mod.TryGetValue("modId", out var id) ? id.ToString() : null,
            Name = mod.TryGetValue("displayName", out var name) ? name.ToString() : null,
            File = Path.GetFileName(filePath),
            Version = mod.TryGetValue("version", out var ver) ? ver.ToString() : null, // Fix issue with forge version showing "{file.jarVersion}"
            Loader = ModLoader.forge,
            Side = ModSide.unknown,
        };

    public static ModEntry ParseFabric(ZipArchiveEntry modsJson, string filePath)
    {
        using StreamReader reader = new(modsJson.Open());
        string json = reader.ReadToEnd();

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        ModEntry modEntry = CreateFabricModEntry(root, filePath);
        modEntry.Requires.AddRange(root.GetPropertyKeysIfObject("requires"));
        modEntry.Recommends.AddRange(root.GetPropertyKeysIfObject("recommends"));

        return modEntry;
    }

    private static ModEntry CreateFabricModEntry(JsonElement root, string filePath) =>
        new()
        {
            ModId = GetPropertyOrNull(root, "id"),
            Name = GetPropertyOrNull(root, "name"),
            File = Path.GetFileName(filePath),
            Version = GetPropertyOrNull(root, "version"),
            Loader = ModLoader.fabric,
            Side = GetSideFromEnvironment(root),
        };

    private static void GetFabricDependencies(ModEntry modEntry, JsonElement root)
    {
        if (
            root.TryGetProperty("depends", out var depends)
            && depends.ValueKind == JsonValueKind.Object
        )
        {
            foreach (var depend in depends.EnumerateObject())
            {
                modEntry.Requires.Add(depend.Name);
            }
        }

        if (
            root.TryGetProperty("recommends", out var recommends)
            && recommends.ValueKind == JsonValueKind.Object
        )
        {
            foreach (var recomend in recommends.EnumerateObject())
            {
                modEntry.Recommends.Add(recomend.Name);
            }
        }
    }

    private static string GetPropertyOrNull(this JsonElement root, string name) =>
        root.TryGetProperty(name, out var property) ? property.GetString() : null;

    private static ModSide GetSideFromEnvironment(this JsonElement root)
    {
        if (!root.TryGetProperty("environment", out var environmentProperty))
            return ModSide.unknown;

        return environmentProperty.GetString() switch
        {
            "client" => ModSide.client,
            "server" => ModSide.server,
            "*" => ModSide.both,
            _ => ModSide.unknown,
        };
    }

    private static IEnumerable<string> GetPropertyKeysIfObject(this JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var prop) || prop.ValueKind != JsonValueKind.Object)
            return Array.Empty<string>();

        return prop.EnumerateObject().Select(p => p.Name);
    }
}
