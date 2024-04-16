using UnityEngine;

namespace UnigineExporter
{
    public class JESphereCollider : JEComponent
    {
        SphereCollider unitySphereCollider;
        override public void Preprocess()
        {
            unitySphereCollider = unityComponent as SphereCollider;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONSphereCollider
            {
                type = GetTypeName(),

                enabled = unitySphereCollider.enabled,

                center = unitySphereCollider.center.ToUnigineVector(),
                radius = unitySphereCollider.radius,

                collisionMask = unitySphereCollider.includeLayers.value | PhysicsCollisionMatrix.GetMask(unitySphereCollider.gameObject.layer),
                exclusionMask = unitySphereCollider.excludeLayers.value
            };
            return json;
        }
    }
}
