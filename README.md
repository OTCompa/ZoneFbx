# ZoneFbx
(Most of the README copied from the original)
ZoneFbx, based on its namesake from a now deleted [repository](https://github.com/lmcintyre/zonefbx),
is a program that exports FFXIV zones to FBX format, complete with textures
and proper hierarchy/object grouping.

This fork of ZoneFbx is a rewrite of the original in C# with some mappings to C++ code to utilize the FBX SDK.
Majority of the logic is kept 1:1 with the original. In other words, I have 0 idea what I'm doing.

If normal maps look weird in Blender, which they will, set the Normal node
to sRGB color space, and set the Normal/Map to World Space.

# Usage
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
