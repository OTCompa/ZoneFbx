using Lumina.Data.Parsing.Layer;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using ZoneFbx.Fbx;
namespace ZoneFbx.Processor
{
    internal class InstanceObjectProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, ModelProcessor modelProcessor, CollisionProcessor collisionProcessor) : Processor(data, contextManager, options)
    {
        private readonly ModelProcessor modelProcessor = modelProcessor;
        private readonly CollisionProcessor collisionProcessor = collisionProcessor;

        private Dictionary<FbxLightObject, IntPtr> lightCache = new();

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

        public void ResetCache() => lightCache.Clear();

        public IntPtr ProcessInstanceObjectBG(LayerCommon.InstanceObject obj)
        {
            var bgObj = (BGInstanceObject) obj.Object;
            IntPtr modelNode;

            if (options.mode != ZoneExporter.Mode.Collision)
            {
                var modelFilePath = bgObj.AssetPath;
                var model = modelProcessor.LoadModel(modelFilePath);
                if (model == null) return IntPtr.Zero;

                modelNode = Node.Create(contextManager, $"{Path.GetFileNameWithoutExtension(modelFilePath)} ({obj.InstanceId})");
                Util.InitChildNode(obj, modelNode);

                if (modelProcessor.ProcessModel(model, modelNode)) return modelNode;
            } else
            {
                var collisionFilePath = bgObj.CollisionAssetPath;
                
                modelNode = Node.Create(contextManager, $"{Path.GetFileNameWithoutExtension(collisionFilePath)} ({obj.InstanceId})");
                Util.InitChildNode(obj, modelNode);

                if (bgObj.CollisionType == ModelCollisionType.Replace && collisionProcessor.ProcessCollisionAsset(collisionFilePath, modelNode))
                {
                    return modelNode;
                } else if (bgObj.CollisionType == ModelCollisionType.Box)
                {
                    // not 100% accurate. some models with this CollisionType aren't solid. 
                    // however, anything with this CollisionType that isn't actually solid is usually out of bounds
                    // would rather be slightly inaccurate out of bounds than in bounds
                    var objectFilePath = bgObj.AssetPath;
                    var model = modelProcessor.LoadModel(objectFilePath);
                    if (model == null) {
                        Node.Delete(modelNode);
                        return IntPtr.Zero;
                    }
                    if (modelProcessor.ProcessModelWithoutTexture(model, modelNode)) return modelNode;
                }
            }

            Node.Delete(modelNode);
            return IntPtr.Zero;
        }

        // Records get structural Equals/GetHashCode for free, which is what we want for the cache key.
        private record DiffuseColorHDRI(double Red, double Green, double Blue);
        private record FbxLightObject(float Intensity, DiffuseColorHDRI DiffuseColor, Light.EType LightType, Light.EDecayType DecayType);

        public IntPtr ProcessInstanceObjectLayLight(LayerCommon.InstanceObject obj)
        {
            if (!options.enableLighting) return IntPtr.Zero;


            var lightObj = (LightInstanceObject)obj.Object;
            if (lightObj.DiffuseColorHDRI.Intensity == 0.0) return IntPtr.Zero;

            DiffuseColorHDRI diffuseColor = new(lightObj.DiffuseColorHDRI.Red, lightObj.DiffuseColorHDRI.Green, lightObj.DiffuseColorHDRI.Blue);
            FbxLightObject toCache = new(lightObj.DiffuseColorHDRI.Intensity, diffuseColor, lightTypeDict.GetValueOrDefault(lightObj.LightType, Light.EType.ePoint), lightDecayTypeDict.GetValueOrDefault(lightObj.Attenuation, Light.EDecayType.eNone));

            var lightNode = Node.Create(contextManager, $"light_{obj.InstanceId}");
            Util.InitChildNode(obj, lightNode);
            if (lightCache.TryGetValue(toCache, out var light))
            {
                Node.SetNodeAttribute(lightNode, light);
                return lightNode;
            }

            light = Light.Create(contextManager, $"light_{obj.InstanceId}");


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
            lightCache.Add(toCache, light);
            return lightNode;
        }
    }
}
