using System.Collections.Generic;
using UnityEngine;

namespace UnigineExporter
{
    public class JEScene
    {
        public static string name;

        public static JEScene TraverseScene()
        {
            var scene = new JEScene();

            List<GameObject> root = new List<GameObject>();

            object[] objects = GameObject.FindObjectsOfType(typeof(GameObject), true);

            foreach (object o in objects)
            {
                GameObject go = (GameObject)o;

                if (go.transform.parent == null)
                    root.Add(go);
            }

            if (root.Count == 0)
            {
                ExportError.FatalError("Cannot Export Empty Scene");
            }

            foreach (var go in root)
            {
                scene.rootGameObjects.Add(Traverse(go));
            }

            return scene;
        }

        public void Preprocess()
        {
            foreach (var jgo in rootGameObjects)
                jgo.Preprocess();

            // register all resources
            foreach (var jgo in rootGameObjects)
                jgo.QueryResources();

            JEResources.Preprocess();
        }

        public void Process()
        {
            JEResources.Process();

            foreach (var jgo in rootGameObjects)
                jgo.Process();
        }

        public void PostProcess()
        {
            JEResources.PostProcess();

            foreach (var jgo in rootGameObjects)
                jgo.PostProcess();
        }

        List<JEGameObject> rootGameObjects = new List<JEGameObject>();

        public static JEGameObject Traverse(GameObject obj, JEGameObject jparent = null, bool isPrefab = false)
        {
            JEGameObject jgo = new JEGameObject(obj, jparent, isPrefab);

            foreach (Transform child in obj.transform)
            {
                Traverse(child.gameObject, jgo);
            }

            return jgo;
        }

        public JSONScene ToJSON()
        {
            var json = new JSONScene
            {
                name = name,
                resources = JEResources.ToJSON(),
                hierarchy = new List<JSONGameObject>()
            };

            foreach (var go in rootGameObjects)
                json.hierarchy.Add(go.ToJSON());

            return json;

        }
    }
}
