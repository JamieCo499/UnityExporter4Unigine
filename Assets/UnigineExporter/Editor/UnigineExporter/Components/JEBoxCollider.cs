using UnityEngine;

namespace UnigineExporter
{
    public class JEBoxCollider : JEComponent
    {
        BoxCollider unityBoxCollider;
        public override void Preprocess()
        {
            unityBoxCollider = unityComponent as BoxCollider;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONBoxCollider
            {
                type = GetTypeName(),
                
                enabled = unityBoxCollider.enabled,

                size = unityBoxCollider.size.ToUnigineVector(),
                center = unityBoxCollider.center.ToUnigineVector(),

                collisionMask = unityBoxCollider.includeLayers.value | PhysicsCollisionMatrix.GetMask(unityBoxCollider.gameObject.layer),
                exclusionMask = unityBoxCollider.excludeLayers.value
            };

            return json;
        }
    }
}
