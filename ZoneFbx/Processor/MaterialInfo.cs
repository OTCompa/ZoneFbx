using Lumina.Data.Parsing;
using Lumina.Models.Materials;
using System.Numerics;
using System.Text;

namespace ZoneFbx.Processor
{
    internal class MaterialInfo
    {
        public Vector3? DiffuseFactor { get; set; }
        public Vector3? BlendDiffuseFactor { get; set; }
        public Vector3? SpecularFactor { get; set; }
        public Vector3? EmissiveFactor { get; set; }
        public Vector3? BlendEmissiveFactor { get; set; }
        public Vector3? NormalFactor { get; set; }
        public Vector3? LightshaftFactor { get; set; }
        public Vector4? UvScale { get; set; }

        private float? Unk2 { get; set; }

        public MaterialInfo(Material material, string outputPath, ZoneExporter.Options options)
        {
            if (material.File == null) return;

            var values = material.File.ShaderValues;

            foreach (var c in material.File.Constants)
            {
                switch (c.ConstantId)
                {
                    case 0x2C2A34DD:
                        DiffuseFactor = ReadVec3(values, c, 12, "diffuse color", filterOne: true);
                        break;
                    case 0x141722D5:
                        SpecularFactor = ReadVec3(values, c, 12, "specular color", filterOne: true);
                        break;
                    case 0x38A64362:
                        EmissiveFactor = ReadVec3(values, c, 12, "emissive color");
                        break;
                    case 0x3F8AC211:
                        BlendDiffuseFactor = ReadVec3(values, c, 12, "blend diffuse color");
                        break;
                    case 0xAA676D0F:
                        BlendEmissiveFactor = ReadVec3(values, c, 12, "blend emissive color");
                        break;
                    case 0xB5545FBB:
                        // TODO: actually implement this
                        WarnUnexpectedSize(c, 4, "normal factor");
                        break;
                    case 0x8F8B0070:
                        WarnUnexpectedSize(c, 4, "unk1");
                        break;
                    case 0x8981D4D9:
                        // version?
                        WarnUnexpectedSize(c, 4, "unk2");
                        Unk2 = ReadFloat(values, c);
                        break;
                    case 0xBB99CF76:
                        UvScale = ReadVec4(values, c, 16, "UV scale");
                        break;
                    case 0xD27C58B9:
                        // this might be a vector4 with size 16, textools only has it as 3 floats though
                        LightshaftFactor = ReadVec3(values, c, 12, "lightshaft");
                        break;
                    //case 0x793AC5A3:
                    // normal blend?
                }
            }

            if (Unk2 == 7f)
                EmissiveFactor = Vector3.Zero;

            if (options.enableJsonExport)
                ExportShaderConstants(material, outputPath);
        }

        private static void WarnUnexpectedSize(Constant c, int expected, string name)
        {
            if (c.ValueSize != expected)
                Console.WriteLine($"Unexpected size for {name}. May cause unexpected results.");
        }

        private static Vector3? ReadVec3(float[] values, Constant c, int expectedSize, string name, bool filterOne = false)
        {
            WarnUnexpectedSize(c, expectedSize, name);
            int idx = c.ValueOffset / 4;
            if (idx + 2 >= values.Length) return null;
            var v = new Vector3(values[idx], values[idx + 1], values[idx + 2]);
            return (filterOne && v == Vector3.One) ? null : v;
        }

        private static Vector4? ReadVec4(float[] values, Constant c, int expectedSize, string name)
        {
            WarnUnexpectedSize(c, expectedSize, name);
            int idx = c.ValueOffset / 4;
            if (idx + 3 >= values.Length) return null;
            return new Vector4(values[idx], values[idx + 1], values[idx + 2], values[idx + 3]);
        }

        private static float? ReadFloat(float[] values, Constant c)
        {
            int idx = c.ValueOffset / 4;
            return idx < values.Length ? values[idx] : null;
        }

        private static void ExportShaderConstants(Material material, string outputPath)
        {
            var values = material.File!.ShaderValues;
            var sb = new StringBuilder("ConstantId,0,1,2,3\n");
            foreach (var c in material.File.Constants)
            {
                int idx = c.ValueOffset / 4;
                int count = c.ValueSize / 4;
                sb.Append($"{c.ConstantId:X}");
                for (int i = 0; i < count; i++) sb.Append($",{values[idx + i]}");
                for (int i = 0; i < 4 - count; i++) sb.Append(',');
                sb.Append('\n');
            }
            var folder = Path.Combine(outputPath, "constants");
            Directory.CreateDirectory(folder);
            File.WriteAllText(
                Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(material.MaterialPath)}.csv"),
                sb.ToString());
        }
    }
}
