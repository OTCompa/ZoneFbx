# ZoneFbx
(Most of the README copied from the original)
ZoneFbx, based on its namesake from a now deleted [repository](https://github.com/lmcintyre/zonefbx),
is a program that exports FFXIV zones to FBX format, complete with textures
and proper hierarchy/object grouping.

This version of ZoneFbx is a rewrite of the original in C# with some mappings to C++ code to utilize the FBX SDK.
A large amount of the logic is kept 1:1 with the original. In other words, I have 0 idea what I'm doing.

# Features
- FFXIV zone export to .FBX with textures
- Light sources
- Extracting blended textures and mapping to materials
- Collision model exports
- Festival model exports

# Issues
If normal maps look weird in Blender, try setting the Normal node
to sRGB color space, and set the Normal/Map to World Space.  
If lights look weird, try disabling "Shadows" for the specific light source. 
Unfortunately, FBX's `CastShadows` option doesn't do anything with regards to this
in Blender so I'm not sure how to handle this.  

For any other errors, please open an issue.

## Notes
In this program, there are some arbitrary constant multipliers used
to make the preview in Blender look closer to what it does in-game
by default. You are able to set these constants manually if you desire
but here are the defaults:

For all meshes:
- Specular factor = 0.3
- Normal factor = 0.2

For all lighting:
- Intensity factor = 10000

## Blended textures
Some of the textures ingame are blended with another texture, or with the same texture with a different tint.

The alpha channel of the vertex color dictates how much of each texture should show  
0 -> only original texture shows  
1 -> only secondary texture shows  

Since blending textures is more in shader territory, it's not possible (that I know of) to make this happen in just
an FBX file without a proprietary solution. To make up for this, I've added the following custom properties to the
materials where relevant when the blend textures flag is toggled:
- `BlendDiffuse`
- `BlendEmissive`
- `BlendSpecular`
- `BlendNormal`

The values for these custom properties is just the file name of the secondary texture. You can then post-process
the imported FBX file in whatever 3D modeling program you prefer. Since I use Blender, I made a simple plugin
that automates the process of adding the requisite things to blend textures: https://github.com/OTCompa/ZoneFbxBlenderPlugin

If custom properties aren't available in the program you use, the material texture map flag outputs
a JSON file `materialTextureMap.json` that maps materials to their respective textures in this format:
```
{
  "material_name_without_ext": {
    "Diffuse": "file_name_without_ext",
    "BlendDiffuse": "file2",
    // same idea for the rest
  },
}
```


# Usage
## GUI
![image](https://github.com/user-attachments/assets/f71bb76f-6fc5-46d6-b30f-134028348e99)


The GUI is a wrapper for the CLI that makes it easier to input in the correct arguments.
From top to bottom:
1. Path to the `sqpack` folder
2. Desired output directory
3. Level/map that you want to extract (Options are populated upon selecting a valid `sqpack` folder)
4. Miscellaneous flags and variables, described in the following section
5. Execute button, disabled until the level/map text box has a value

## CLI
### Flags
- `-l`    Allows lightshafts to be included in the final export
- `-f`    Exports festival models in a separate export
- `-b`    Disables texture baking
- `-j`    Exports all relevant LGB/SGB files as JSON for debugging purposes
- `-i`    Allows light sources to be included in the final export
- `-m`    Exports a material map for json
- `-c`    Exports collision map in a separate export

### Variables
- `--specular`        Sets the specular factor. Range: [0, 1]
- `--normal`          Sets the normal factor. Range: [0, 1]
- `--lightIntensity`  Sets the light intensity factor. Range: [0, Int32.Max]

https://streamable.com/tjg45n

- download the file
- extract the file
- shift click on empty space in the folder
- click Open Powershell Window Here
- enter ".\zonefbx.exe", the path to your XIV install, the path to the zone you want to extract (in the video i grab it from godbert), and the path you want the FBX to be saved to, in the exact format the video does it. when stuff is filled in automatically, that's me pressing "tab" - you don't have to type the full thing, only enough for windows to fill the rest in
- press enter
- import into blender!

note: the program is really bad with slashes and spaces in file paths, type it exactly like the video

# Building
To build this from scratch, you need the [FBX SDK](https://aps.autodesk.com/developer/overview/fbx-sdk)
installed. This project specifically uses `FBX SDK 2020.3.7 VS2022` at the time of writing this.
There shouldn't be any issue with changing to a different version if needed, just make sure to update
the paths in the properties.  
Building this project is only tested with Visual Studio Community 2022.

# Credits
In the process of writing this code, I heavily referenced other people's code for research. 
I'd like to thank the people behind these projects for the amount of information that I've learned from them.
- [Imcintyre for the original ZoneFbx](https://github.com/lmcintyre/ZoneFbx) (Deleted repository)
- [Lumina](https://github.com/NotAdam/Lumina)
- [Sapphire](https://github.com/SapphireServer/Sapphire) and their Discord
- [TexTools](https://github.com/TexTools)
- [Takhlaq](https://github.com/takhlaq/ZoneFbx)
- [AzureRain1](https://github.com/AzureRain1/ZoneFbx)
