using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using ZoneFbx.Fbx;
namespace ZoneFbx.Processor
{
    internal class InstanceObjectProcessor
    {
        private readonly Lumina.GameData data;
        private readonly ModelProcessor modelProcessor;
        private readonly IntPtr scene;
        private readonly ZoneExporter.Flags flags;

        private const float LightIntensityFactor = 10000;

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

        public InstanceObjectProcessor(Lumina.GameData data, ModelProcessor modelProcessor, IntPtr scene, ZoneExporter.Flags flags) { 
            this.data = data;
            this.modelProcessor = modelProcessor;
            this.scene = scene;
            this.flags = flags;
        }

        public IntPtr ProcessInstanceObjectBG(LayerCommon.InstanceObject obj)
        {
            var objectFilePath = ((BGInstanceObject)obj.Object).AssetPath;
            var model = modelProcessor.LoadModel(objectFilePath);
            if (model == null) return IntPtr.Zero;

            var modelNode = Node.Create(scene, objectFilePath.Substring(objectFilePath.LastIndexOf("/") + 1));
            Util.InitChildNode(obj, modelNode);

            if (modelProcessor.ProcessModel(model, modelNode))
            {
                return modelNode;
            } else
            {
                return IntPtr.Zero;
            }
        }

        public IntPtr ProcessInstanceObjectLayLight(LayerCommon.InstanceObject obj)
        {
            if (!flags.enableLighting) return IntPtr.Zero;

            var lightNode = Node.Create(scene, $"light_{obj.InstanceId}");
            Util.InitChildNode(obj, lightNode);

            var lightObj = (LightInstanceObject)obj.Object;
            if (lightObj.DiffuseColorHDRI.Intensity == 0.0) return IntPtr.Zero;

            var light = Light.Create(scene, $"light_{obj.InstanceId}");

            Light.SetIntensity(light, lightObj.DiffuseColorHDRI.Intensity * LightIntensityFactor);
            Light.SetColor(light, lightObj.DiffuseColorHDRI.Red / 255f, lightObj.DiffuseColorHDRI.Green / 255f, lightObj.DiffuseColorHDRI.Blue / 255f);
            Light.SetLightType(light, lightTypeDict.GetValueOrDefault(lightObj.LightType, Light.EType.ePoint));
            Light.SetDecay(light, lightDecayTypeDict.GetValueOrDefault(lightObj.Attenuation, Light.EDecayType.eNone));

            if (lightObj.BGShadowEnabled == 1) Light.CastShadows(light);

            if (lightObj.LightType == LightType.Spot)
            {
                Light.SetAngle(light, lightObj.ConeDegree + lightObj.AttenuationConeCoefficient, lightObj.ConeDegree);
            }

            Node.SetNodeAttribute(lightNode, light);
            return lightNode;
        }
    }
}
