using Lumina.Data.Parsing;
using Lumina.Models.Materials;
using System.Numerics;
using System.Text;

namespace ZoneFbx.Processor
{
    internal class MaterialInfo
    {
        public Vector3? DiffuseFactor { get; set; } = null;
        public Vector3? BlendDiffuseFactor { get; set; } = null;
        public Vector3? SpecularFactor { get; set; } = null;
        //public Vector3? BlendSpecularFactor { get; set; } = null;
        public Vector3? EmissiveFactor { get; set; } = null;
        public Vector3? BlendEmissiveFactor { get; set; } = null;
        public Vector3? NormalFactor { get; set; } = null;
        public Vector3? LightshaftFactor { get; set; } = null;

        public Vector4? UvScale { get; set; } = null;

        private float? Unk1 { get; set; } = null;
        private float? Unk2 { get; set; } = null;

        private ushort? diffuseOffset = null;
        private ushort? blendDiffuseOffset = null;
        private ushort? blendEmissiveOffset = null;
        private ushort? specularOffset = null;
        //private ushort? specular2Offset = null;
        private ushort? emissiveOffset = null;
        //private ushort? emissive2Offset = null;
        private ushort? normalOffset = null;
        private ushort? lightshaftOffset = null;
        private ushort? uvScaleOffset = null;
        private ushort? unk1Offset = null;  // emmissive scale?
        private ushort? unk2Offset = null;  // version?


        public MaterialInfo(Material material, string outputPath, ZoneExporter.Options options)
        {
            if (material.File == null) return;

            readConstants(material);
            readColorConstants(material.File.ShaderValues);

            if (options.enableJsonExport) exportShaderConstants(material, outputPath);
        }

        private void readConstants(Material material)
        {
            for (int i = 0; i < material.File!.Constants.Length; i++)
            {
                var constant = material.File.Constants[i];
                switch (constant.ConstantId)
                {
                    case 0x2C2A34DD:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        diffuseOffset = constant.ValueOffset;
                        break;
                    case 0x141722D5:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for specular color. May cause unexpected results.");
                        }
                        specularOffset = constant.ValueOffset;
                        break;
                    case 0x38A64362:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for emmisive color. May cause unexpected results.");
                        }
                        emissiveOffset = constant.ValueOffset;
                        break;
                    case 0x3F8AC211:
                        // diffuse blend color
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        blendDiffuseOffset = constant.ValueOffset;
                        break;
                    case 0xAA676D0F:
                        // emissive blend color
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        blendEmissiveOffset = constant.ValueOffset;
                        break;
                    case 0xB5545FBB:
                        // TODO: actually implement this
                        if (constant.ValueSize != 4)
                        {
                            Console.WriteLine("Unexpected size for normal factor. May cause unexpected results.");
                        }
                        normalOffset = constant.ValueOffset;
                        break;
                    case 0x8F8B0070:
                        if (constant.ValueSize != 4)
                        {
                            Console.WriteLine("Unexpected size for unk1Offset. May cause unexpected results.");
                        }
                        unk1Offset = constant.ValueOffset;
                        // emissive?
                        break;
                    case 0x8981D4D9:
                        if (constant.ValueSize != 4)
                        {
                            Console.WriteLine("Unexpected size for unk2Offset. May cause unexpected results.");
                        }
                        unk2Offset = constant.ValueOffset;
                        // version?
                        break;
                    case 0xBB99CF76:
                        // normal UV scale
                        if (constant.ValueSize != 16)
                        {
                            Console.WriteLine($"Unexpected size for UV scale ({constant.ValueSize}). May cause unexpected results.");
                        }
                        uvScaleOffset = constant.ValueOffset;
                        break;
                    case 0xD27C58B9:
                        // this might be a vector 4 with size 16, textools only has it as 3 floats though
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine($"Unexpected size for lightshaftOffset ({constant.ValueSize}). May cause unexpected results.");
                        }
                        lightshaftOffset = constant.ValueOffset;
                        break;
                    //case 0x793AC5A3:
                    // normal blend?
                }
            }
        }

        private void readColorConstants(float[] values)
        {
            if (diffuseOffset.HasValue)
            {
                DiffuseFactor = readVector3Constant(values, diffuseOffset.Value, filterOne: true);
            }

            if (blendDiffuseOffset.HasValue)
            {
                BlendDiffuseFactor = readVector3Constant(values, blendDiffuseOffset.Value);
            }
            if (specularOffset.HasValue)
            {
                SpecularFactor = readVector3Constant(values, specularOffset.Value, filterOne: true);
            }
            if (emissiveOffset.HasValue)
            {
                EmissiveFactor = readVector3Constant(values, emissiveOffset.Value);
            }
            if (normalOffset.HasValue)
            {
                //NormalFactor = readVector3Constant(values, normalOffset.Value);
            }
            if (blendEmissiveOffset.HasValue)
            {
                BlendEmissiveFactor = readVector3Constant(values, blendEmissiveOffset.Value);
            }
            if (uvScaleOffset.HasValue)
            {
                UvScale = readVector4Constant(values, uvScaleOffset.Value);
            }
            if (unk1Offset.HasValue)
            {
                Unk1 = readFloatConstant(values, unk1Offset.Value);
            }
            if (unk2Offset.HasValue)
            {
                Unk2 = readFloatConstant(values, unk2Offset.Value);
            }
            if (lightshaftOffset.HasValue)
            {
                LightshaftFactor = readVector3Constant(values, lightshaftOffset.Value);
            }


            if (Unk2 == 7f)
            {
                EmissiveFactor = Vector3.Zero;
            }
        }

        private static Vector3? readVector3Constant(float[] values, ushort byteOffset, bool filterZero = false, bool filterOne = false)
        {
            int idx = byteOffset / 4;
            if (idx + 2 >= values.Length) return null;
            var v = new Vector3(values[idx], values[idx + 1], values[idx + 2]);

            if (filterZero && v == Vector3.Zero || filterOne && v == Vector3.One) return null;

            return v;
        }

        private static Vector4? readVector4Constant(float[] values, ushort byteOffset, bool filterZero = false, bool filterOne = false)
        {
            int idx = byteOffset / 4;
            if (idx + 3 >= values.Length) return null;
            var v = new Vector4(values[idx], values[idx + 1], values[idx + 2], values[idx + 3]);

            if (filterZero && v == Vector4.Zero || filterOne && v == Vector4.One) return null;

            return v;
        }

        private static float? readFloatConstant(float[] values, ushort byteOffset)
        {
            int idx = byteOffset / 4;
            if (idx >= values.Length) return null;
            return values[idx];
        }

        private static void exportShaderConstants(Material material, string outputPath)
        {
            var values = material.File!.ShaderValues;
            var sb = new StringBuilder();
            sb.Append("ConstantId,0,1,2,3\n");
            foreach (var constant in material.File.Constants)
            {
                int idx = constant.ValueOffset / 4;
                int count = constant.ValueSize / 4;
                sb.Append($"{constant.ConstantId:X}");
                for (int i = 0; i < count; i++)
                {
                    sb.Append($",{values[idx + i]}");
                }
                for (int i = 0; i < 4 - count; i++)
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            var folder = Path.Combine(outputPath, "constants");
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(material.MaterialPath)}.csv"), sb.ToString());
        }
    }
}
