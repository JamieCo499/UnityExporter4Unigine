using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JECamera : JEComponent
    {
        Camera unityCamera;

        override public void Preprocess()
        {
            unityCamera = unityComponent as Camera;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONCamera
            {
                type = GetTypeName(),
                perspective = !unityCamera.orthographic,
                nearClip = unityCamera.nearClipPlane,
                farClip = unityCamera.farClipPlane,
                fov = unityCamera.fieldOfView,
                height = unityCamera.orthographicSize * 2
            };

            return json;
        }
    }
}
