using Lumina.Data;
using Lumina.Extensions;
using Lumina.Models.Materials;
using System.Numerics;

namespace ZoneFbx.Processor
{
    internal class MaterialInfo
    {
        public Vector3? DiffuseColor { get; set; } = null;
        public Vector3? SpecularColor { get; set; } = null;
        public Vector3? EmissiveColor { get; set; } = null;

        private ushort? diffuseOffset = null;
        private ushort? specularOffset = null;
        private ushort? emissiveOffset = null;

        public MaterialInfo(Vector3? diffuseColor = null, Vector3? specularColor = null, Vector3? emissiveColor = null)
        {
            if (diffuseColor != null) DiffuseColor = diffuseColor;
            if (specularColor != null) SpecularColor = specularColor;
            if (emissiveColor != null) EmissiveColor = emissiveColor;
        }

        public MaterialInfo(Material material)
        {
            if (material.File == null) return;

            readConstants(material);

            if (diffuseOffset == -1 && specularOffset == -1 && emissiveOffset == -1)
                return;

            var br = material.File.Reader;
            long baseOffset = getMaterialConstantBaseOffset(br);

            readColorConstants(br, baseOffset);
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
                DiffuseColor = readVector3Constant(br, baseOffset + diffuseOffset.Value, true, true);
            }
            if (specularOffset.HasValue)
            {
                SpecularColor = readVector3Constant(br, baseOffset + specularOffset.Value, false, true);
            }
            if (emissiveOffset.HasValue)
            {
                EmissiveColor = readVector3Constant(br, baseOffset + emissiveOffset.Value, true, false);
                EmissiveColor *= .2f;
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
