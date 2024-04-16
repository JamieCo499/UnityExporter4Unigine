using UnityEngine;

namespace UnigineExporter
{
    public class JETransform : JEComponent
    {
        private Transform unityTransform;
        override public void Preprocess()
        {
            unityTransform = unityComponent as Transform;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONTransform
            {
                type = GetTypeName(),
                localPosition = unityTransform.localPosition.ToUnigineVector(),
                localScale = unityTransform.localScale.ToUnigineVector()
            };

            Quaternion localRotation = unityTransform.localRotation;
            if (unityTransform.TryGetComponent<Light>(out _) || unityTransform.TryGetComponent<Camera>(out _))
            {
                localRotation *= Quaternion.FromToRotation(Vector3.back, Vector3.down);
            }
            json.localRotation = localRotation.ToUnigineQuaternion();

            return json;
        }
    }
}
