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

def scan_jar(jar_path):
    info = {
        "mod_id": None,
        "loader": "unknown",
        "side": ["both"],
        "file": jar_path.name,
        "dependencies": [None],
        "note": None,
    }

    with zipfile.ZipFile(jar_path, "r") as jar:
        files = jar.namelist()

        if "META-INF/mods.toml" in files:
            info["loader"] = "neoforge"
            mods_toml = toml.loads(jar.read("META-INF/mods.toml").decode())
        
        elif "fabric.mod.json" in files:
            info["loader"] = "fabric"
            fabric_json = json.loads(jar.read("fabric.mod.json").decode())

            info["mod_id"] = fabric_json["id"]
            info["name"] = fabric_json.get("name", info["mod_id"])

            env = fabric_json.get("environment", "*")
            if env == "client":
                info["side"] = ["client"]
            elif env == "server":
                info["side"] = ["server"]

        info["dependencies"] = extract_requirements(
            is_forge_mod=(info["loader"] == "neoforge"),
            mod_manifest=mods_toml if info["loader"] == "neoforge" else fabric_json
        )

    return info if info["mod_id"] else None

def extract_requirements(is_forge_mod, mod_manifest):
    requirements = list()

    if (is_forge_mod):
        if "dependencies" in mod_manifest:
            for dependency_id, dependency_list in mod_manifest["dependencies"].items():
                for dependency in dependency_list:
                    requirements.append(dependency["modId"])
    
    else:
        requirements = list(dict.fromkeys(requirements))
    
    return requirements

def prompt_for_mod(mod):
    print(f"\nNew mod detected:")
    print(f"Name: {mod['name']}")
    print(f"ID: {mod['mod_id']}")
    print(f"Loader: {mod['loader']}")
    print(f"Detected side: {mod['side']}")

    def yn(prompt):
        return input(prompt + " [y/N]: ").lower().startswith("y")

    return {
        "loader": mod["loader"],
        "file": mod["file"],
        "side": mod["side"],
        "required": yn("Required mod?"),
        "recommended": yn("Recommended optional mod?"),
        "performance": yn("Performance-related?"),
        "heavy": yn("Heavy?"),
        "note": mod["note"],
    }

def main(running_dir: Path):
    mods_dir = running_dir / "mods"
    manifest_path = running_dir / "manifest.toml"

    manifest = load_manifest(manifest_path)
    mods = manifest.setdefault("mods", {})

    for jar in mods_dir.glob("*.jar"):
        info = scan_jar(jar)
        if not info:
            print(f"Skipping mod: {jar.name}")
            continue

        mod_id = info["mod_id"]
        if mod_id in mods:
            continue

        entry = prompt_for_mod(info)
        mods[mod_id] = entry

    save_manifest(manifest, manifest_path)
    print("\nManifest updated.")


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python scanner.py <modpack_directory>")
        sys.exit(1)

    main(Path(sys.argv[1]).resolve())
