using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx
{
    internal class MaterialInfo
    {
        public Vector3? DiffuseColor { get; set; } = null;
        public Vector3? SpecularColor { get; set; } = null;
        public Vector3? EmissiveColor { get; set; } = null;
        public MaterialInfo(Vector3? diffuseColor = null, Vector3? specularColor = null, Vector3? emissiveColor = null)
        {
            if (diffuseColor != null) this.DiffuseColor = diffuseColor;
            if (specularColor != null) this.SpecularColor = specularColor;
            if (emissiveColor != null) this.EmissiveColor = emissiveColor;
        }
    }
}
