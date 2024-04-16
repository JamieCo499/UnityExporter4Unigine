using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEPrefab
    {
        private static Dictionary<string, JEPrefab> allPrefabs = new Dictionary<string, JEPrefab>();

        public string id;
        private GameObject unityGameObject;
        private string relativePath = "";
        private JEGameObject jgo;

        public static string GetPath(GameObject gameObject) 
            => ExporterUtils.GetRelativePath(Path.GetFullPath(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject)));

        public static string GetNewID()
        {
            return allPrefabs.Count.ToString();
        }

        private JEPrefab(GameObject gameObject)
        {
            unityGameObject = gameObject;
            relativePath = GetPath(unityGameObject);
            allPrefabs[relativePath] = this;
            id = GetNewID();

            jgo = JEScene.Traverse(gameObject, null, true);
        }

        void PreprocessPrefab()
        {
            jgo.Preprocess();
            jgo.QueryResources();
        }

        void ProcessPrefab()
        {
            jgo.Process();
        }

        void PostProcessPrefab()
        {
            jgo.PostProcess();
        }

        public static void Preprocess()
        {
            foreach (var prefab in allPrefabs.Values)
                prefab.PreprocessPrefab();
        }

        public static void Process()
        {
            foreach (var prefab in allPrefabs.Values)
                prefab.ProcessPrefab();
        }

        public static void PostProcess()
        {
            foreach (var prefab in allPrefabs.Values)
                prefab.PostProcessPrefab();
        }

        public static JEPrefab RegisterPrefab(GameObject gameObject)
        {
            string path = GetPath(gameObject);
            Debug.Log($"{gameObject.name} -> {path}");
            if (allPrefabs.ContainsKey(path))
            {
                return allPrefabs[path];
            }

            return new JEPrefab(gameObject);
        }

        public static void Reset()
        {
            allPrefabs = new Dictionary<string, JEPrefab>();
        }

        public JSONPrefab ToJSON()
        {
            var json = new JSONPrefab
            {
                id = id,
                relativePath = relativePath,
                node = jgo.ToJSON()
            };
            return json;
        }
        private class GraphNode
        {
            public string Id => obj.id;
            public HashSet<string> references = new HashSet<string>();
            public JSONPrefab obj;
        }

        private static List<GraphNode> TopologicalSort(List<GraphNode> nodes)
        {
            List<GraphNode> sortedList = new List<GraphNode>();
            HashSet<string> visitedIds = new HashSet<string>();

            void DFS(GraphNode obj)
            {
                if (!visitedIds.Contains(obj.Id))
                {
                    visitedIds.Add(obj.Id);
                    foreach (var refId in obj.references)
                    {
                        GraphNode refObj = nodes.Find(o => o.Id == refId);
                        if (refObj != null)
                        {
                            DFS(refObj);
                        }
                    }
                    sortedList.Add(obj);
                }
            }

            foreach (var obj in nodes)
            {
                DFS(obj);
            }

            return sortedList;
        }

        public static List<JSONPrefab> GenerateJSONPrefabList()
        {
            List<JSONPrefab> prefabs = new List<JSONPrefab>();

            foreach (var prefab in allPrefabs.Values)
                prefabs.Add(prefab.ToJSON());

            // Create dependencies graph
            static void GetReferences(JSONGameObject go, ref HashSet<string> refs)
            {
                if (go.isPrefab)
                    refs.Add(go.prefabId);
                if (go.children != null)
                {
                    foreach (var child in go.children)
                        GetReferences(child, ref refs);
                }
            }

            List<GraphNode> nodes = new List<GraphNode>();
            foreach (var prefab in prefabs)
            {
                var node = new GraphNode
                {
                    obj = prefab
                };
                GetReferences(prefab.node, ref node.references);
                nodes.Add(node);
            }

            // Topological sort of prefabs dependencies
            nodes = TopologicalSort(nodes);

            prefabs = nodes.Select(x => x.obj).ToList();
            return prefabs;
        }
    }
}
