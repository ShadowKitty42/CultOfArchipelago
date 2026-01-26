# CultOfArchipelago

## What is this?

This is a development project intended to make Cult of the Lamb work on [Archipelago Multiworld Randomizer](https://archipelago.gg).<br/>
Nothing is currently working as we are still figuring out how the game works internally.

## For Developers

Setting up the development environment is fairly simple:
1. Download and install the latest stable release of [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/latest). Make sure to get the `x64` version.
2. Clone this repo.
3. Rename the `Directory.Build.props.default` file to `Directory.Build.props.user`.
4. Open `Directory.Build.props.user` using a text editor.
5. Edit the `GameFolder` property to match your game installation.

Your dev tools should automatically find the libraries and will copy the `.dll` to the BepInEx's plugins folder after building the project.