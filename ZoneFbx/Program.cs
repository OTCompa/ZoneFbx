using Lumina.Data.Parsing.Layer;
using ZoneFbx;
using static Lumina.Models.Materials.Texture;

namespace ZoneFbx
{
    class Program
    {
        private static string usage = """
        Usage: zonefbx.exe [game_sqpack_path] [zone_path] [output_path]
        For example, if you have the default install location, and want to export Central Shroud to your desktop,
        zonefbx.exe "C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack" 
        ffxiv/fst_f1/fld/f1f1/level/f1f1 "C:\Users\Username\Desktop\" [flags] [variables]

        Example with flags and variables; flags: light sources, blend; variables: specular=0.2, lightIntensity=20000
        zonefbx.exe "C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack" 
        ffxiv/fst_f1/fld/f1f1/level/f1f1 "C:\Users\Username\Desktop\" -si --specular 0.2 --lightIntensity 20000

        Available flags:
        -l    Enables lightshaft models in the export
        -f    Enables festival models in the export
        -b    Disables baking textures
        -j    Exports all relevant LGB/SGB files as JSON for debugging purposes
        -i    Allows light sources to be included in the final export
        -s    Texture blending; Extracts secondary textures and adds the filename a custom property in each material. Actual blending happens after importing through your own means. More info in the repo's README
        -m    Adds a material to texture map for processing in case custom properties aren't supported.

        Available variables:
        --specular          Number; Sets the specular factor (Default: 0.3)
        --normal            Number; Sets the normal factor (Default: 0.2)
        --lightIntensity    Number; Sets the light intensity factor (Default: 10000)
        """;

        static void Main(string[] args)
        {
            if (!SanitizeInput(args)) Environment.Exit(1);

            ZoneExporter.Options options = new();

            if (args.Length >= 4 && !ProcessArgs(args, ref options)) Environment.Exit(1);

            test(args, options);
            //var zoneExporter = new ZoneExporter(args[0], args[1], args[2], options);
        }

        static void test(string[] args, ZoneExporter.Options options)
        {

            var zoneExporter = new ZoneExporter(args[0], args[1], args[2], options);
        }

        private static bool SanitizeInput(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(usage);
                return false;
            }

            if (!args[0].Replace("\\", "").EndsWith("sqpack"))
            {
                ColorMessage("Error: Game path must point to the sqpack folder!\n");
                Console.WriteLine(usage);
                return false;
            }

            if (args[1].EndsWith("/"))
            {
                ColorMessage("Error: Level path must not have a trailing slash.\n");
                Console.WriteLine(usage);
                return false;
            }

            if (args[1].StartsWith("bg/"))
            {
                ColorMessage("Error: Level path must not begin with bg/.\n");
                Console.WriteLine(usage);
                return false;
            }

            if (!Directory.Exists(args[2]))
            {
                ColorMessage("Error: Export folder must exist.\n");
                Console.WriteLine(usage);
                return false;
            }

            if (!args[2].EndsWith("\\"))
            {
                ColorMessage("Error: Export folder must have a trailing slash.\n");
                Console.WriteLine(usage);
                return false;
            }
            return true;
        }

        private static bool ProcessArgs(string[] args, ref ZoneExporter.Options options)
        {
            if (args.Length >= 4)
            {
                int i = 3;
                while (i < args.Length) 
                {
                    var arg = args[i];

                    if (arg.StartsWith("--"))
                    {
                        if (i + 1 >= args.Length)
                        {
                            ColorMessage($"Error: unexpected end of input for arg {arg}");
                            Console.WriteLine(usage);
                            return false;
                        }
                        if (!ProcessVariableArgs(arg, args[i + 1], ref options))
                        {
                            return false;
                        }
                        i += 2;
                    } else if (arg.StartsWith('-')) {
                        ProcessFlags(arg, ref options);
                        i++;
                    } else
                    {
                        ColorMessage($"Ignoring possibly misplaced argument: {arg}", ConsoleColor.Yellow);
                        i++;
                    }
                }
            }

            return true;
        }

        private static void ProcessFlags(string arg, ref ZoneExporter.Options options)
        {
            foreach (char flag in arg.Substring(1))
            {
                switch (flag)
                {
                    case 'l':
                        options.enableLightshaftModels = true; break;
                    case 'f':
                        options.enableFestivals = true; break;
                    case 'b':
                        options.disableBaking = true; break;
                    case 'j':
                        options.enableJsonExport = true; break;
                    case 'i':
                        options.enableLighting = true; break;
                    case 's':
                        options.enableBlend = true; break;
                    case 'm':
                        options.enableMTMap = true; break;
                    case 'c':
                        options.enableCollisions = true; break;
                    default:
                        ColorMessage($"Unknown flag \"{flag}\", ignoring...", ConsoleColor.Yellow);
                        Console.WriteLine(usage);
                        break;
                }
            }
        }

        private static bool ProcessVariableArgs(string arg, string value, ref ZoneExporter.Options options)
        {
            double doubleValue;
            switch (arg.Substring(2).ToLower())
            {
                case "specular":
                    if (!double.TryParse(value, out doubleValue))
                    {
                        ColorMessage($"Invalid value for {arg}: {value}");
                        return false;
                    }
                    options.specularFactor = doubleValue;
                    break;
                case "normal":
                    if (!double.TryParse(value, out doubleValue))
                    {
                        ColorMessage($"Invalid value for {arg}: {value}");
                        return false;
                    }
                    options.normalFactor = doubleValue;
                    break;
                case "lightintensity":
                    if (!double.TryParse(value, out doubleValue))
                    {
                        ColorMessage($"Invalid value for {arg}: {value}");
                        return false;
                    }
                    options.lightIntensityFactor = doubleValue;
                    break;
                default:
                    ColorMessage($"Unknown argument \"{arg}\", ignoring...", ConsoleColor.Yellow);
                    Console.WriteLine(usage);
                    break;

            }
            return true;
        }

        private static void ColorMessage(string error, ConsoleColor color = ConsoleColor.Red)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(error);
            Console.ResetColor();
        }
    }
}

