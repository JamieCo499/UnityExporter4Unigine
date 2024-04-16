using UnityEngine;

namespace UnigineExporter
{
    public class JEMeshCollider : JEComponent
    {
        MeshCollider unityMeshCollider;
        override public void Preprocess()
        {
            unityMeshCollider = unityComponent as MeshCollider;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONMeshCollider
            {
                type = GetTypeName(),

                enabled = unityMeshCollider.enabled,

                collisionMask = unityMeshCollider.includeLayers.value | PhysicsCollisionMatrix.GetMask(unityMeshCollider.gameObject.layer),
                exclusionMask = unityMeshCollider.excludeLayers.value
            };
            return json;
        }
    }
}
