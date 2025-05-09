# ZoneFbx
(Most of the README copied from the original)
ZoneFbx, based on its namesake from a now deleted [repository](https://github.com/lmcintyre/zonefbx),
is a program that exports FFXIV zones to FBX format, complete with textures
and proper hierarchy/object grouping.

This version of ZoneFbx is a rewrite of the original in C# with some mappings to C++ code to utilize the FBX SDK.
A large amount of the logic is kept 1:1 with the original. In other words, I have 0 idea what I'm doing.

# Issues
If normal maps look weird in Blender, try setting the Normal node
to sRGB color space, and set the Normal/Map to World Space.  
If lights look weird, try disabling "Shadows" for the specific light source. 
Unfortunately, FBX's `CastShadows` option doesn't do anything with regards to this
in Blender so I'm not sure how to handle this.  

For any other errors, please open an issue.

## Arbitrary constants
In this program, there are some arbitrary constant multipliers used
to make the preview in Blender look closer to what it does in-game
by default. I'm not really sure what's the best way to go about this,
so I'm just doing what I think is reasonable. Feel free to suggest
what I should do. I may add an option to set specify these factors
manually in the future.

For all meshes:
- Specular factor = 0.3
- Emissive factor = 0.2 (Applied when baking textures since the FBX SDK's emissiveFactor didn't seem to be doing anything)
- Normal factor = 0.2

For all lighting:
- Intensity factor = 1000

# Usage
## GUI
![image](https://github.com/user-attachments/assets/8c20a2e2-1b1f-4791-95ce-f318f1cbcfa4)

The GUI is a wrapper for the CLI that makes it easier to input in the correct arguments.
From top to bottom:
1. Path to the `sqpack` folder
2. Desired output directory
3. Level/map that you want to extract (Options are populated upon selecting a valid `sqpack` folder)
4. Miscellaneous flags, described in the following section
5. Execute button, disabled until the level/map text box has a value

## CLI
### Flags
- `-l`    Allows lightshafts to be included in the final export
- `-f`    Allows festival models to be included in the final export
- `-b`    Disables texture baking
- `-j`    Exports all relevant LGB/SGB files as JSON for debugging purposes
- `-i`    Allows light sources to be included in the final export

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
- [TexTools](https://github.com/TexTools)
- [Takhlaq](https://github.com/takhlaq/ZoneFbx)
- [AzureRain1](https://github.com/AzureRain1/ZoneFbx)
