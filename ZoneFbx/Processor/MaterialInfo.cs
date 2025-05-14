using Lumina.Data;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using Lumina.Models.Materials;
using System.Numerics;
using System.Reflection.Metadata;
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
        //public Vector3? Emissive2Color { get; set; } = null;
        public Vector3? NormalFactor { get; set; } = null;
        public bool DiffuseBlendEnabled { get; set; } = false;

        private ushort? diffuseOffset = null;
        private ushort? blendDiffuseOffset = null;
        private ushort? backupDiffuseOffset = null;
        private ushort? specularOffset = null;
        //private ushort? specular2Offset = null;
        private ushort? emissiveOffset = null;
        //private ushort? emissive2Offset = null;
        private ushort? normalOffset = null;


        public MaterialInfo(Material material, string outputPath, ZoneExporter.Flags flags)
        {
            if (material.File == null) return;

            foreach (var key in material.File.ShaderKeys)
            {
                // texture mode == blend?
                // every material with 2 different textures or 2 different diffuse factors has this keyval combo
                // removing this keyval combo also disables blending so this should be correct
                if (key.Category == 0xB616DC5A && key.Value == 0x1DF2985C)
                {
                    DiffuseBlendEnabled = true;
                }
            }

            readConstants(material);

            if (diffuseOffset == -1 && specularOffset == -1 && emissiveOffset == -1 && blendDiffuseOffset == -1)
                return;

            var br = material.File.Reader;
            long baseOffset = getMaterialConstantBaseOffset(br);

            readColorConstants(br, baseOffset);

            if (!DiffuseBlendEnabled)
            {
                BlendDiffuseFactor = null;
            }

            if (flags.enableJsonExport) exportShaderConstants(material, outputPath, br, baseOffset);
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
                    case 0xAA676D0F:
                        // diffuse blend color
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        blendDiffuseOffset = constant.ValueOffset;
                        break;
                    case 0x3F8AC211:
                        // secondary entry for diffuses?
                        // if both primary and blend diffuses exist,
                        // whichever component is larger among this and the primary vector is the value for the final DiffuseColor vector
                        // if the blend diffuse doesn't exist, then this seems to be used as the blend diffuse
                        // see: n4g8 (E8S) crystals, n4gw (FRU) crystals, x6r7 (M8S) P1/P2 floor
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        backupDiffuseOffset = constant.ValueOffset;
                        break;
                    case 0xB5545FBB:
                        // TODO: actually implement this
                        if (constant.ValueSize != 4)
                        {
                            Console.WriteLine("Unexpected size for normal factor. May cause unexpected results.");
                        }
                        normalOffset = constant.ValueOffset;
                        break;
                    //case 0x793AC5A3:
                    // normal blend?
                }
            }
        }

        private long getMaterialConstantBaseOffset(LuminaBinaryReader br)
        {
            int colorsetBlockSize;
            int stringBlockSize;
            int additionalDataSize;

            // get string block size
            br.Seek(6);
            colorsetBlockSize = br.ReadUInt16();
            stringBlockSize = br.ReadUInt16();

            // get tex/map/colorset numbers to figure out where string block is
            br.Seek(12);
            var numStrings = 0;
            for (int i = 0; i < 3; i++)
            {
                numStrings += br.ReadByte();
            }

            additionalDataSize = br.ReadByte();

            long cursor = 16;
            // put cursor at beginning of shader area
            cursor += numStrings * 4 + additionalDataSize + stringBlockSize + colorsetBlockSize + 2;  // skip shader constants data size
            br.Seek(cursor);

            var numShaderKeys = br.ReadUInt16();
            var numShaderConstants = br.ReadUInt16();
            var numTextureSampler = br.ReadUInt16();
            br.Seek(br.Position + 4 + numShaderKeys * 8);  // position at the start of shader constants ids/offsets/sizes
            cursor = br.Position + numShaderConstants * 8 + numTextureSampler * 12;

            return cursor;
        }

        private void readColorConstants(LuminaBinaryReader br, long baseOffset)
        {
            // get all the relevant information
            if (diffuseOffset.HasValue)
            {
                DiffuseFactor = readVector3Constant(br, baseOffset + diffuseOffset.Value, false, true);
            }

            if (blendDiffuseOffset.HasValue)
            {
                BlendDiffuseFactor = readVector3Constant(br, baseOffset + blendDiffuseOffset.Value, true, true);
            }
            if (specularOffset.HasValue)
            {
                SpecularFactor = readVector3Constant(br, baseOffset + specularOffset.Value, false, true);
            }
            if (emissiveOffset.HasValue)
            {
                EmissiveFactor = readVector3Constant(br, baseOffset + emissiveOffset.Value, false, false);
            }
            if (normalOffset.HasValue)
            {
                NormalFactor = readVector3Constant(br, baseOffset + normalOffset.Value, false, false);
            }

            // for notes on this: see the comment around where this is set
            if (backupDiffuseOffset.HasValue)
            {
                var temp = readVector3Constant(br, baseOffset + backupDiffuseOffset.Value, true, true);
                if (DiffuseFactor == null)
                {
                    DiffuseFactor = temp;
                } else if (BlendDiffuseFactor != null)
                {
                    if (temp != null)
                    {
                        DiffuseFactor = new Vector3(
                            Math.Max(temp.Value.X, DiffuseFactor.Value.X),
                            Math.Max(temp.Value.Y, DiffuseFactor.Value.Y),
                            Math.Max(temp.Value.Z, DiffuseFactor.Value.Z)
                        );
                    }
                } else
                {
                    BlendDiffuseFactor = temp;
                }
            }

        }

        private Vector3? readVector3Constant(LuminaBinaryReader br, long offset, bool filterZero = false, bool filterOne = false)
        {
            br.Seek(offset);
            var v = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            if (filterZero && v == Vector3.Zero || filterOne && v == Vector3.One) return null;

            return v;
        }

        private void exportShaderConstants(Material material, string outputPath, LuminaBinaryReader br, long baseOffset)
        {
            var sb = new StringBuilder();
            sb.Append("ConstantId,0,1,2,3\n");
            foreach (var constant in material.File!.Constants)
            {
                br.Seek(baseOffset + constant.ValueOffset);
                var shaderConstantValues = br.ReadSingleArray(constant.ValueSize / 4);
                sb.Append($"{constant.ConstantId:X}");
                foreach (var value in shaderConstantValues)
                {
                    sb.Append($",{value.ToString()}");
                }
                for (int i = 0; i < 4 - shaderConstantValues.Length; i++)
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
