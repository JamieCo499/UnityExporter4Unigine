using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEMesh
    {
        private static Dictionary<Mesh, JEMesh> allMeshes = new Dictionary<Mesh, JEMesh>();
        private static Dictionary<string, JEMesh> allPaths = new Dictionary<string, JEMesh>();

        public string name;
        public string id;
        private string relativePath = "";
        private Mesh unityMesh;

        // set by skinned renderer if any
        public Transform[] bones;
        public string rootBone = "";

        public static string GetPath(Mesh mesh) => ExporterUtils.GetRelativePath(Path.GetFullPath(AssetDatabase.GetAssetPath(mesh)));

        public static string GetNewID()
        {
            return allMeshes.Count.ToString();
        }

        private JEMesh(Mesh mesh)
        {
            unityMesh = mesh;
            this.id = GetNewID();
            allMeshes[mesh] = this;

            relativePath = GetPath(mesh);
            if (!string.IsNullOrEmpty(relativePath))
            {
                allPaths[relativePath] = this;
                name = Path.GetFileName(relativePath);
            }
            else
            {
                name = Path.GetFileName(relativePath) + "_" + mesh.name;
            }
        }

        public static JEMesh RegisterMesh(Mesh mesh)
        {
            Debug.Log($"{mesh.name} -> {GetPath(mesh)}");
            if (allMeshes.ContainsKey(mesh))
            {
                return allMeshes[mesh];
            }

            string path = GetPath(mesh);
            if (allPaths.ContainsKey(path))
            {
                return allPaths[path];
            }

            return new JEMesh(mesh);
        }

        public static void Reset()
        {
            allMeshes = new Dictionary<Mesh, JEMesh>();
            allPaths = new Dictionary<string, JEMesh>();
        }

        public JSONMesh ToJSON()
        {
            var json = new JSONMesh
            {
                id = id,
                name = name,

                relativePath = relativePath,

                boundMeshName = unityMesh.name,
                boundSize = unityMesh.bounds.size.ToUnigineVector()
            };

            return json;
        }

        public static List<JSONMesh> GenerateJSONMeshList()
        {
            List<JSONMesh> meshes = new List<JSONMesh>();

            foreach (var mesh in allMeshes.Values.Union(allPaths.Values))
                meshes.Add(mesh.ToJSON());

            return meshes;
        }
    }
}
