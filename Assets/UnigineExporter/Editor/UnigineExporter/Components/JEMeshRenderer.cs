using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEMeshRenderer : JEComponent
    {
        private MeshRenderer unityMeshRenderer;
        private MeshFilter unityMeshFilter;

        private JEMesh mesh;
        private List<JEMaterial> materials = new List<JEMaterial>();

        override public void Preprocess()
        {
            unityMeshRenderer = unityComponent as MeshRenderer;
            unityMeshFilter = jeGameObject.unityGameObject.GetComponent<MeshFilter>();
        }

        override public void QueryResources()
        {
            if (unityMeshFilter != null)
            {
                Mesh sharedMesh = unityMeshFilter.sharedMesh;
                if (sharedMesh != null)
                    mesh = JEMesh.RegisterMesh(sharedMesh);
            }

            for (int i = 0; i < unityMeshRenderer.sharedMaterials.Length; i++)
            {
                if (unityMeshRenderer.sharedMaterials[i] == null)
                    continue;
                materials.Add(JEMaterial.RegisterMaterial(unityMeshRenderer.sharedMaterials[i]));
            }
        }

        public float RelativeHeightToDistance(Camera camera, float relativeHeight, float size)
        {
            if (camera.orthographic)
            {
                return -1f;
            }

            float num = Mathf.Tan(MathF.PI / 180f * camera.fieldOfView * 0.5f);
            return size * 0.5f / (relativeHeight * num);
        }
 
        public override JSONComponent ToJSON()
        {
            var json = new JSONMeshRenderer
            {
                type = GetTypeName(),
                enabled = unityMeshRenderer.enabled,
            };

            if (mesh == null)
            {
                return null;
            }

            if (mesh != null)
            {
                json.meshId = mesh.id;
                json.file = mesh.name;
                json.mesh = unityMeshFilter.sharedMesh.name;
            }

            json.materialsId = new string[materials.Count];
            for (int i = 0; i < materials.Count; i++)
            {
                json.materialsId[i] = materials[i].id;
            }

            // Select all lod groups where this renderer used
            var groups = MonoBehaviour.FindObjectsOfType<LODGroup>().Where(x => x.GetLODs().Any(l => l.renderers.Contains(unityMeshRenderer))).ToList();

            if (groups.Count != 0)
            {
                List<float> distances = new List<float>(groups.Max(x => x.lodCount));
                int myLod = 0;
                foreach (var group in groups)
                {
                    //TODO: get minimum
                    LOD[] lods = group.GetLODs();

                    for (int i = 0; i < lods.Length; i++)
                    {
                        LOD l = lods[i];

                        float percentage = l.screenRelativeTransitionHeight;
                        float distance = RelativeHeightToDistance(SceneView.lastActiveSceneView.camera, l.screenRelativeTransitionHeight, group.size);
                        distances.Add(distance);
                        if (l.renderers.Contains(unityMeshRenderer))
                            myLod = i;
                    }
                    break;
                }
                distances.Sort();



                float minDist = myLod == 0 ? 0.0f : distances[myLod - 1];
                float maxDist = distances[myLod];
                json.minDistance = minDist;
                json.maxDistance = maxDist;
            }

            return json;
        }
    }
}
