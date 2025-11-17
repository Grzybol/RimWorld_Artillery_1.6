# RimWorld Artillery Command Platform

A RimWorld 1.6 mod that adds a dedicated research tab and research project for a remote artillery platform capable of firing on world map settlements up to 12 tiles away.

## Features
- **New Research Tab** – "Artillery command" appears as its own tab, independent from vanilla research pages.
- **Artillery Command Platform** – Unlocks after completing Mortars and investing 2000 research points in the new project.
- **World Map Bombardment** – Select the platform and launch a strike on hostile settlements within 12 tiles to permanently destroy them.

## Building requirements
- Powered structure that consumes 600 W while active.
- Requires mortar shells as ammunition (consumes 1 shell per strike).

## Development
1. Add RimWorld's managed assemblies (e.g. `Assembly-CSharp.dll`, `RimWorld.exe`, `UnityEngine.CoreModule.dll`, `Verse.dll`) to `Source/RimWorldArtillery/References`.
2. Build the project using `dotnet build Source/RimWorldArtillery/RimWorldArtillery.csproj`.
3. Copy the resulting `RimWorldArtillery.dll` into the `Assemblies` folder.

The `Assemblies` directory is intentionally empty so the repository does not contain game binaries.
