using UnityEngine;

namespace UnigineExporter
{
    public class JEHingeJoint : JEComponent
    {
        HingeJoint unityHingeJoint;

        string connectedBodyId = string.Empty;

        public override void Preprocess()
        {
            unityHingeJoint = unityComponent as HingeJoint;
        }

        public override void Process()
        {
            if (unityHingeJoint.connectedBody)
            {
                connectedBodyId = JEGameObject.GetObjectId(unityHingeJoint.connectedBody.gameObject);
            }
        }

        public override JSONComponent ToJSON()
        {
            if (connectedBodyId == string.Empty)
            {
                return null;
            }
            
            var json = new JSONHingeJoint
            {
                type = GetTypeName(),
                connectedObjectId = connectedBodyId,
                maxForce = unityHingeJoint.breakForce, 
                maxTorque = unityHingeJoint.breakTorque,
                collision = unityHingeJoint.enableCollision,

                anchor = unityHingeJoint.anchor.ToUnigineVector(),
                connectedAnchor = unityHingeJoint.connectedAnchor.ToUnigineVector(),
                axis = unityHingeJoint.axis.ToUnigineVector(),
            };

            return json;
        }
    }
}
