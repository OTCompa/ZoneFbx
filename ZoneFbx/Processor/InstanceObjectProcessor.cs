using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using ZoneFbx.Fbx;
using Lumina.Data.Files.Pcb;
namespace ZoneFbx.Processor
{
    internal class InstanceObjectProcessor
    {
        private readonly Lumina.GameData data;
        private readonly ModelProcessor modelProcessor;
        private readonly CollisionProcessor collisionProcessor;
        private readonly IntPtr scene;
        private readonly ZoneExporter.Options options;

        private Dictionary<int, IntPtr> lightCache = new();

        private Dictionary<LightType, Light.EType> lightTypeDict = new()
        {
            {LightType.Directional, Light.EType.eDirectional },
            {LightType.Point, Light.EType.ePoint },
            {LightType.Spot, Light.EType.eSpot },
            {LightType.Plane, Light.EType.eArea },
        };

        private Dictionary<float, Light.EDecayType> lightDecayTypeDict = new()
        {
            {1, Light.EDecayType.eLinear },
            {2, Light.EDecayType.eQuadratic },
            {3, Light.EDecayType.eCubic },
        };

        public InstanceObjectProcessor(Lumina.GameData data, ModelProcessor modelProcessor, CollisionProcessor collisionProcessor, IntPtr scene, ZoneExporter.Options options) { 
            this.data = data;
            this.modelProcessor = modelProcessor;
            this.collisionProcessor = collisionProcessor;
            this.scene = scene;
            this.options = options;
        }

        public IntPtr ProcessInstanceObjectBG(LayerCommon.InstanceObject obj)
        {
            var bgObj = (BGInstanceObject) obj.Object;
            var objectFilePath = ((BGInstanceObject)obj.Object).AssetPath;
            var collisionFilePath = bgObj.CollisionAssetPath;



            var modelNode = Node.Create(scene, objectFilePath.Substring(objectFilePath.LastIndexOf("/") + 1));
            Util.InitChildNode(obj, modelNode);

            if (bgObj.CollisionType == ModelCollisionType.Replace && collisionProcessor.ProcessCollisionAsset(collisionFilePath, modelNode))
            {
                return modelNode;
            } else if (bgObj.CollisionType == ModelCollisionType.Box)
            {
                // not 100% accurate. some models with this CollisionType aren't solid. 
                // however, anything with this CollisionType that isn't actually solid is usually out of bounds
                // would rather be slightly inaccurate out of bounds than in bounds
                var model = modelProcessor.LoadModel(objectFilePath);
                if (model == null) return IntPtr.Zero;
                modelProcessor.ProcessModelWithoutTexture(model, modelNode);

                return modelNode;
            } else
            {
                return IntPtr.Zero;
            }

            //var model = modelProcessor.LoadModel(objectFilePath);
            //if (model == null) return IntPtr.Zero;

            //var modelNode = Node.Create(scene, objectFilePath.Substring(objectFilePath.LastIndexOf("/") + 1));
            //Util.InitChildNode(obj, modelNode);

            //if (modelProcessor.ProcessModel(model, modelNode))
            //{
            //    return modelNode;
            //} else
            //{
            //    return IntPtr.Zero;
            //}
        }

        private class FbxLightObject
        {
            internal class DiffuseColorHDRI
            {
                public double Red;
                public double Green;
                public double Blue;
                public DiffuseColorHDRI(double red, double green, double blue)
                {
                    Red = red;
                    Green = green;
                    Blue = blue;
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(Red, Green, Blue);
                }
            }
            public float Intensity;
            public DiffuseColorHDRI DiffuseColor;
            public Light.EType LightType;
            public Light.EDecayType DecayType;

            public FbxLightObject(float intensity, DiffuseColorHDRI diffuseColor, Light.EType lightType, Light.EDecayType decayType)
            {
                Intensity = intensity;
                DiffuseColor = diffuseColor;
                LightType = lightType;
                DecayType = decayType;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Intensity, DiffuseColor, LightType, DecayType);
            }
        }

        public IntPtr ProcessInstanceObjectLayLight(LayerCommon.InstanceObject obj)
        {
            if (!options.enableLighting) return IntPtr.Zero;


            var lightObj = (LightInstanceObject)obj.Object;
            if (lightObj.DiffuseColorHDRI.Intensity == 0.0) return IntPtr.Zero;

            FbxLightObject.DiffuseColorHDRI diffuseColor = new(lightObj.DiffuseColorHDRI.Red, lightObj.DiffuseColorHDRI.Green, lightObj.DiffuseColorHDRI.Blue);
            FbxLightObject toCache = new(lightObj.DiffuseColorHDRI.Intensity, diffuseColor, lightTypeDict.GetValueOrDefault(lightObj.LightType, Light.EType.ePoint), lightDecayTypeDict.GetValueOrDefault(lightObj.Attenuation, Light.EDecayType.eNone));

            var lightNode = Node.Create(scene, $"light_{obj.InstanceId}");
            Util.InitChildNode(obj, lightNode);
            if (lightCache.TryGetValue(toCache.GetHashCode(), out var light))
            {
                Node.SetNodeAttribute(lightNode, light);
                return lightNode;
            }

            light = Light.Create(scene, $"light_{obj.InstanceId}");


            Light.SetIntensity(light, lightObj.DiffuseColorHDRI.Intensity * options.lightIntensityFactor);
            Light.SetColor(light, lightObj.DiffuseColorHDRI.Red / 255f, lightObj.DiffuseColorHDRI.Green / 255f, lightObj.DiffuseColorHDRI.Blue / 255f);
            Light.SetLightType(light, lightTypeDict.GetValueOrDefault(lightObj.LightType, Light.EType.ePoint));
            Light.SetDecay(light, lightDecayTypeDict.GetValueOrDefault(lightObj.Attenuation, Light.EDecayType.eNone));

            if (lightObj.BGShadowEnabled == 1) Light.CastShadows(light);

            if (lightObj.LightType == LightType.Spot)
            {
                Light.SetAngle(light, lightObj.ConeDegree + lightObj.AttenuationConeCoefficient, lightObj.ConeDegree);
            }

            Node.SetNodeAttribute(lightNode, light);
            lightCache.Add(toCache.GetHashCode(), light);
            return lightNode;
        }
    }
}
