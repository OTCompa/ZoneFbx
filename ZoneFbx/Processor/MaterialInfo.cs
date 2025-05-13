using Lumina.Data;
using Lumina.Extensions;
using Lumina.Models.Materials;
using System.Numerics;

namespace ZoneFbx.Processor
{
    internal class MaterialInfo
    {
        public Vector3? DiffuseColor { get; set; } = null;
        public Vector3? Diffuse2Color { get; set; } = null;
        public Vector3? SpecularColor { get; set; } = null;
        public Vector3? Specular2Color { get; set; } = null;
        public Vector3? EmissiveColor { get; set; } = null;
        public Vector3? Emissive2Color { get; set; } = null;
        public bool BlendEnabled { get; set; } = false;

        private ushort? diffuseOffset = null;
        private ushort? diffuse2Offset = null;
        private ushort? specularOffset = null;
        private ushort? specular2Offset = null;
        private ushort? emissiveOffset = null;
        private ushort? emissive2Offset = null;


        public MaterialInfo(Material material)
        {
            if (material.File == null) return;

            foreach (var key in material.File.ShaderKeys)
            {
                if (key.Category == 0xB616DC5A && key.Value == 0x1DF2985C)
                {
                    BlendEnabled = true;
                    break;
                }
            }

            readConstants(material);

            if (diffuseOffset == -1 && specularOffset == -1 && emissiveOffset == -1 && diffuse2Offset == -1 && emissive2Offset == -1)
                return;

            var br = material.File.Reader;
            long baseOffset = getMaterialConstantBaseOffset(br);

            readColorConstants(br, baseOffset);
            if ((EmissiveColor == Vector3.Zero || EmissiveColor == null) && (Emissive2Color == Vector3.Zero || Emissive2Color == null))
            {
                EmissiveColor = null;
                Emissive2Color = null;
            }

            if (!BlendEnabled)
            {
                diffuse2Offset = null;
                specular2Offset = null;
                emissive2Offset = null;
            }
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
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        diffuse2Offset = constant.ValueOffset;
                        break;
                    case 0x3F8AC211:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for emmisive color. May cause unexpected results.");
                        }
                        emissive2Offset = constant.ValueOffset;
                        break;
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
                DiffuseColor = readVector3Constant(br, baseOffset + diffuseOffset.Value, false, true);
            }
            if (diffuse2Offset.HasValue)
            {
                Diffuse2Color = readVector3Constant(br, baseOffset + diffuse2Offset.Value, true, true);
            }
            if (specularOffset.HasValue)
            {
                SpecularColor = readVector3Constant(br, baseOffset + specularOffset.Value, false, true);
            }
            if (emissiveOffset.HasValue)
            {
                EmissiveColor = readVector3Constant(br, baseOffset + emissiveOffset.Value, false, false);
            }
            if (emissive2Offset.HasValue)
            {
                Emissive2Color = readVector3Constant(br, baseOffset + emissive2Offset.Value, false, false);
            }
        }

        private Vector3? readVector3Constant(LuminaBinaryReader br, long offset, bool filterZero = false, bool filterOne = false)
        {
            br.Seek(offset);
            var v = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            if (filterZero && v == Vector3.Zero || filterOne && v == Vector3.One) return null;

            return v;
        }
    }
}
