# Omnistarter Unity Toolkit

Omnistarter Unity Toolkit, or OUT for short, is a Unity toolkit developed in 2022.3.62f3, including

- handy scripts, e.g. simple logics, input, file IO, utilities
- editor tweakers, e.g. auto updating modified date in scripts
- useful sprite shaders
- etc.

It is made by myself and for myself, so it may be difficult to use for others. Sorry for that.

## Prerequisites

Before using OUT, you may need to import these packages to your project first:

- **Unity UI** by Unity (for UI)
- **Input System** by Unity (for input)
- **TextMesh Pro** by Unity (for UI)
- **Universal RP** by Unity (yes it's based on URP now)
- ~~**Netcode for GameObjects** by Unity (for network)~~ (WIP)
- **Newtonsoft Json** by Unity (for API)
  - You can add package by name `com.unity.nuget.newtonsoft-json`.
- [Unity Standalone File Browser](https://github.com/gkngkc/UnityStandaloneFileBrowser) by [gkngkc](https://github.com/gkngkc) (for IO)
  - I personally recommend [add from OpenUPM / GitHub](https://github.com/shiena/UnityStandaloneFileBrowser?tab=readme-ov-file#installation).
- [Odin Serializer](https://github.com/TeamSirenix/odin-serializer) by [TeamSirenix](https://github.com/TeamSirenix) (for IO)
  - I created a [GitHub repo](https://github.com/Omnisch/unity-odinserializer-package.git) to easily add to package manager.

## Special Thanks

- OUT included some codes from [Bordure](https://github.com/nani-core/Bordure) by the team [Nani Core](https://github.com/nani-core)
- Included `IniStorage.cs` from [unity-IniStorage](https://github.com/illa4257/unity-IniStorage)
