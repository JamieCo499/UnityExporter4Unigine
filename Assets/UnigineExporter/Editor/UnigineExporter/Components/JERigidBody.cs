using UnityEngine;

namespace UnigineExporter
{
    public class JERigidBody : JEComponent
    {
        Rigidbody unityRigidBody;
        override public void Preprocess()
        {
            unityRigidBody = unityComponent as Rigidbody;
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONRigidBody
            {
                type = GetTypeName(),
                useGravity = unityRigidBody.useGravity,
                mass = unityRigidBody.mass
            };
            return json;
        }
    }
}
