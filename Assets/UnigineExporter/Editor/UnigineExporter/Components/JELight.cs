using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JELight : JEComponent
    {
        Light unityLight;

        override public void Preprocess()
        {
            unityLight = unityComponent as Light;
        }

        private string GetLightName()
        {
            switch (unityLight.type)
            {
                case LightType.Area:
                case LightType.Disc:
                case LightType.Spot:
                    return "Projected";
                case LightType.Directional:
                    return "World";
                case LightType.Point:
                    return "Point";
                default:
                    return "";
            }
        }

        public override JSONComponent ToJSON()
        {
            // TOOO: Check pipeline
            var json = new JSONLight
            {
                type = GetTypeName(),

                enabled = unityLight.enabled,
                lightType = GetLightName(),
                color = unityLight.color,
                intensity = unityLight.intensity,

                castsShadows = unityLight.shadows != LightShadows.None,
                fov = unityLight.spotAngle,
            };

            if (unityLight.type == LightType.Point || unityLight.type == LightType.Spot)
            {
                json.range = unityLight.range;
            }

            json.realtime = true;
            SerializedObject serial = new SerializedObject(unityLight);
            SerializedProperty lightmapProp = serial.FindProperty("m_Lightmapping");
            if (lightmapProp.intValue != 0)
            {
                json.realtime = false;
            }

            return json;
        }
    }
}
