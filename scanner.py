import sys
import zipfile
import json
import toml
from pathlib import Path

def load_manifest(manifest_path):
    if Path.exists(manifest_path):
        return toml.load(manifest_path)
    else:
        print(f"no manifest in {manifest_path}")

    return {}

def save_manifest(manifest, manifest_path: Path):
    with open(manifest_path, "w", encoding="utf-8") as f:
        toml.dump(manifest, f)

def get_primary_neoforge_mod(mod_manifest):
    mods = mod_manifest.get("mods")
    if not isinstance(mods, list) or not mods:
        return None
    return mods[0]

def scan_jar(jar_path):
    info = {
        "mod_id": None,
        "name": "unknown",
        "notes": [],
        "priority": 0,

        "file": jar_path.name,
        "version": "unknown",
        "loader": "unknown",
        "side": "unknown",

        "importance": "unknown",
        "impact": "unknown",

        "requires": ["unknown"],
        "recommends": ["unknown"],
    }

    with zipfile.ZipFile(jar_path, "r") as jar:
        files = jar.namelist()

        if "META-INF/mods.toml" in files:
            info["loader"] = "neoforge"
            mod_manifest = toml.loads(jar.read("META-INF/mods.toml").decode())

            primary_mod = get_primary_neoforge_mod(mod_manifest)
            if not primary_mod:
                print(f"Error with mod {jar}")
                return None

            info["mod_id"] = primary_mod.get("modId")
            info["name"] = primary_mod.get("displayName", "unknown")
            
            # Add code to add these properties
            info["side"] = "unknown"
        
        elif "fabric.mod.json" in files:
            info["loader"] = "fabric"
            mod_manifest = json.loads(jar.read("fabric.mod.json").decode())

            info["mod_id"] = mod_manifest["id"]
            info["name"] = mod_manifest["name"]

            environment = mod_manifest.get("environment")
            if environment == "*":
                info["side"] = "both"
            elif environment == "client":
                info["side"] = "client"
            elif environment == "server":
                info["side"] = "server"
            else:
                info["side"] = "unknown"
            
        info["requires"] = extract_requires(
            info["mod_id"],
            info["loader"],
            mod_manifest=mod_manifest
        )

        #info["recommends"] = extract_recommends(info["loader"],mod_manifest=mod_manifest)

    return info if info["mod_id"] else None

def extract_requires(mod_id, mod_loader, mod_manifest):
    recommends = list()
    requires = list()

    if (mod_loader == "neoforge"):
        if "dependencies" in mod_manifest:
            for dependency_id, dependency_list in mod_manifest["dependencies"].items():
                for dependency in dependency_list:
                    if dependency["mandatory"] == True:
                        recommends.append(dependency["modId"])
                    else:
                        requires.append(dependency["modId"])
        else:
            print(f"No dependencies found for mod {mod_id}")
    
    elif (mod_loader == "fabric"):
        requires = list(dict.fromkeys(requires))
    
    else:
        print("Unknown modloader, setting requires to unknown")
    
    return requires

def prompt_for_mod(mod, prompt_user):
    def prompt_for_enum(prompt, choices, default="unknown"):
        while True:
            answer = input(prompt).lower()
            if not answer:
                return default
            if answer in choices:
                return choices[answer]
            print("Invalid input.")

    impact_choices = {
        "p": "performance",
        "l": "light",
        "h": "heavy",
        "u": "unknown"
    }
    
    importance_choices = {
        "r": "required",
        "c": "reccomended",
        "o": "optional",
        "u": "unknown"
    }

    entry = {
        "name": mod['name'],
        "notes": mod["notes"],
        "priority": 0,

        "file": mod["file"],
        "version": mod["version"],
        "loader": mod["loader"],
        "side": mod["side"],

        "importance": "unknown",
        "impact": "unknown",

        "requires": [],
        "recommends": [],
    }

    if not prompt_user:
        return entry

    print(f"\nNew mod detected:")
    print(f"Name: {mod['name']}")
    print(f"ID: {mod['mod_id']}")
    print(f"Loader: {mod['loader']}")
    print(f"Detected side: {mod['side']}")

    entry["importance"] = prompt_for_enum(
        "Importance: (r) Required, (c) Recommended, (o) Optional, (u) Unknown [default: unknown]: ",
        impact_choices
    )

    entry["impact"] = prompt_for_enum(
        "Impact: (p) Performance, (l) Light, (h) Heavy, (u) Unknown [default: unknown]: ",
        impact_choices
    )

    return entry

def main(running_dir: Path):
    mods_dir = running_dir / "mods"
    manifest_path = running_dir / "manifest.toml"

    manifest = load_manifest(manifest_path)
    mods = manifest.setdefault("mods", {})

    prompt_user = input("Input fields now? [y/N]: ").lower().startswith("y")

    for jar in mods_dir.glob("*.jar"):
        info = scan_jar(jar)
        if not info:
            print(f"Skipping mod: {jar.name}")
            continue

        mod_id = info["mod_id"]
        if mod_id in mods:
            continue

        entry = prompt_for_mod(info, prompt_user)
        mods[mod_id] = entry

    save_manifest(manifest, manifest_path)
    print("\nManifest updated.")


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python scanner.py <modpack_directory>")
        sys.exit(1)

    main(Path(sys.argv[1]).resolve())
