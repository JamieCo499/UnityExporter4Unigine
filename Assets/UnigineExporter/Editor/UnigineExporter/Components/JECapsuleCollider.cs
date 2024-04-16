using UnityEngine;

namespace UnigineExporter
{
    public class JECapsuleCollider : JEComponent
    {
        CapsuleCollider unityCapsuleCollider;
        public override void Preprocess()
        {
            unityCapsuleCollider = unityComponent as CapsuleCollider;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONCapsuleCollider
            {
                type = GetTypeName(),

                enabled = unityCapsuleCollider.enabled,

                center = unityCapsuleCollider.center.ToUnigineVector(),
                radius = unityCapsuleCollider.radius,
                height = unityCapsuleCollider.height,

                collisionMask = unityCapsuleCollider.includeLayers.value | PhysicsCollisionMatrix.GetMask(unityCapsuleCollider.gameObject.layer),
                exclusionMask = unityCapsuleCollider.excludeLayers.value
            };

            return json;
        }
    }
}
