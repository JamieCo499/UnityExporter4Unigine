using UnityEngine;

namespace UnigineExporter
{
    public class JEFixedJoint : JEComponent
    {
        FixedJoint unityFixedJoint;

        string connectedBodyId = string.Empty;

        public override void Preprocess()
        {
            unityFixedJoint = unityComponent as FixedJoint;
        }

        public override void Process()
        {
            if (unityFixedJoint.connectedBody)
            {
                connectedBodyId = JEGameObject.GetObjectId(unityFixedJoint.connectedBody.gameObject);
            }
        }

        public override JSONComponent ToJSON()
        {
            if (connectedBodyId == string.Empty)
            {
                return null;
            }
            
            var json = new JSONFixedJoint
            {
                type = GetTypeName(),
                connectedObjectId = connectedBodyId,
                maxForce = unityFixedJoint.breakForce, 
                maxTorque = unityFixedJoint.breakTorque,
                collision = unityFixedJoint.enableCollision,
            };

            return json;
        }
    }
}
