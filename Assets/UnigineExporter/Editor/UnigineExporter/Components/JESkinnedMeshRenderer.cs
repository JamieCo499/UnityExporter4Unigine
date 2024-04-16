using System.Collections.Generic;
using UnityEngine;

namespace UnigineExporter
{
    public class JESkinnedMeshRenderer : JEComponent
    {
        private SkinnedMeshRenderer unityMeshRenderer;

        private JEMesh mesh;
        private List<JEMaterial> materials = new List<JEMaterial>();

        override public void Preprocess()
        {
            unityMeshRenderer = unityComponent as SkinnedMeshRenderer;
        }

        override public void QueryResources()
        {
            Mesh sharedMesh = unityMeshRenderer.sharedMesh;
            if (sharedMesh != null)
            {
                mesh = JEMesh.RegisterMesh(sharedMesh);
                mesh.bones = unityMeshRenderer.bones;
                mesh.rootBone = unityMeshRenderer.rootBone == null ? "" : unityMeshRenderer.rootBone.gameObject.name;
            }

            for (int i = 0; i < unityMeshRenderer.sharedMaterials.Length; i++)
            {
                materials.Add(JEMaterial.RegisterMaterial(unityMeshRenderer.sharedMaterials[i]));
            }
        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONSkinnedMeshRenderer
            {
                type = GetTypeName(),
                enabled = unityMeshRenderer.enabled
            };

            if (mesh == null)
            {
                return null;
            }

            if (mesh != null)
            {
                json.meshId = mesh.id;
                json.file = mesh.name;
                json.mesh = unityMeshRenderer.sharedMesh.name;
                json.rootBone = unityMeshRenderer.rootBone.name; //TODO by id
            }
            
            json.materialsId = new string[materials.Count];
            for (int i = 0; i < materials.Count; i++)
            {
                json.materialsId[i] = materials[i].id;
            }

            return json;
        }
    }
}
