using UnityEngine;

namespace UnigineExporter
{
    public class JESpringJoint : JEComponent
    {
        SpringJoint unitySpringJoint;

        string connectedBodyId = string.Empty;

        public override void Preprocess()
        {
            unitySpringJoint = unityComponent as SpringJoint;
        }

        public override void Process()
        {
            if (unitySpringJoint.connectedBody)
            {
                connectedBodyId = JEGameObject.GetObjectId(unitySpringJoint.connectedBody.gameObject);
            }
        }

        public override JSONComponent ToJSON()
        {
            if (connectedBodyId == string.Empty)
            {
                return null;
            }
            
            var json = new JSONSpringJoint
            {
                type = GetTypeName(),
                connectedObjectId = connectedBodyId,
                maxForce = unitySpringJoint.breakForce, 
                maxTorque = unitySpringJoint.breakTorque,
                collision = unitySpringJoint.enableCollision,

                anchor = unitySpringJoint.anchor.ToUnigineVector(),
                connectedAnchor = unitySpringJoint.connectedAnchor.ToUnigineVector(),
                axis = unitySpringJoint.axis.ToUnigineVector(),
            };

            return json;
        }
    }
}
