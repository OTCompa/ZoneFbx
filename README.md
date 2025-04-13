# ZoneFbx
(Most of the README copied from the original)
ZoneFbx, based on its namesake from a now deleted [repository](https://github.com/lmcintyre/zonefbx),
is a program that exports FFXIV zones to FBX format, complete with textures
and proper hierarchy/object grouping.

This fork of ZoneFbx is a rewrite of the original in C# with some mappings to C++ code to utilize the FBX SDK.
A large amount of the logic is kept 1:1 with the original. In other words, I have 0 idea what I'm doing.

For all surfaces:
- Specular factor = 0.3
- Emissive factor = 0.2 (Applied when baking textures since the FBX SDK's emissiveFactor didn't seem to be doing anything)
- Normal factor = 0.2
These values were set arbitrarily, pretty much just what was in the
original code or what made it look close enough to the game. I
do not know where this information is located (if it is) in the .mtrl
file. If you do know or have any ideas, please let me know.

If normal maps look weird in Blender, try setting the Normal node
to sRGB color space, and set the Normal/Map to World Space.

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

https://streamable.com/tjg45n

- download the file
- extract the file
- shift click on empty space in the folder
- click Open Powershell Window Here
- enter ".\zonefbx.exe", the path to your XIV install, the path to the zone you want to extract (in the video i grab it from godbert), and the path you want the FBX to be saved to, in the exact format the video does it. when stuff is filled in automatically, that's me pressing "tab" - you don't have to type the full thing, only enough for windows to fill the rest in
- press enter
- import into blender!

note: the program is really bad with slashes and spaces in file paths, type it exactly like the video

# Issues
If you encounter the issue: `Object bg/ffxiv/wil_w1/evt/w1eb/bgplate/0015.mdl could not be resolved from game data.`
followed by specifically:
```
Specified argument was out of the range of valid values. (Parameter 'usage')
Actual value was 3845360663.
```
This is because of what I think is a bug in a [Lumina](https://github.com/NotAdam/Lumina) which this program uses.
I have a [fork](https://github.com/OTCompa/Lumina) of it that "fixes" the issue, but it'll probably get fixed in the main branch soon.
You should only be seeing the above error if you built it by yourself, as I have included the modified Lumina DLL in the release.  

For any other errors, please open an issue.
