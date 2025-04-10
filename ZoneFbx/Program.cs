using Lumina.Data.Files;
using ZoneFbx;
String usage = """
    Usage: zonefbx.exe [game_sqpack_path] [zone_path] [output_path]
    For example, if you have the default install location, and want to export Central Shroud to your desktop,
    zonefbx.exe "C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack" 
    ffxiv/fst_f1/fld/f1f1/level/f1f1 "C:\Users\Username\Desktop\
    """;
if (args.Length != 3)
{
    Console.WriteLine(usage);
    return;
}

if (!args[0].Replace("\\", "").EndsWith("sqpack"))
{
    Console.WriteLine("Error: Game path must point to the sqpack folder!\n");
    Console.WriteLine(usage);
    return;
}

if (args[1].EndsWith("/"))
{
    Console.WriteLine("Error: Level path must not have a trailing slash.\n");
    Console.WriteLine(usage);
    return;
}

if (args[1].StartsWith("bg/"))
{
    Console.WriteLine("Error: Level path must not begin with bg/.\n");
    Console.WriteLine(usage);
    return;
}

if (!Directory.Exists(args[2]))
{
    Console.WriteLine("Error: Export folder must exist.\n");
    Console.WriteLine(usage);
    return;
}

if (!args[2].EndsWith("\\"))
{
    Console.WriteLine("Error: Export folder must have a trailing slash.\n");
    Console.WriteLine(usage);
    return;
}

var test = new ZoneExporter(args[0], args[1], args[2]);