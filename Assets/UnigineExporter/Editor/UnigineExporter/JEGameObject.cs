using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEGameObject
    {
        public string name;

        public JETransform transform;

        public JEGameObject parent;

        public List<JEGameObject> children = new List<JEGameObject>();

        public List<JEComponent> components = new List<JEComponent>();

        public GameObject unityGameObject;

        public static Dictionary<GameObject, JEGameObject> GameObjectLookup;

        public JEPrefab prefab = null;
        public bool isPrefabGO;

        public string id;

        public static string GetNewID()
        {
            return GameObjectLookup.Count.ToString();
        }

        public void AddComponent(JEComponent component)
        {
            components.Add(component);
        }

        public JEGameObject(GameObject go, JEGameObject parent, bool isPrefabGO = false)
        {
            GameObjectLookup[go] = this;
            this.unityGameObject = go;

            this.parent = parent;
            this.id = GetNewID();
            this.name = go.name;

            this.isPrefabGO = isPrefabGO;

            if (parent != null)
                parent.children.Add(this);

            JEComponent.QueryComponents(this);

            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(unityGameObject);
            if (prefabAssetType == PrefabAssetType.Regular && !isPrefabGO && PrefabUtility.IsAnyPrefabInstanceRoot(unityGameObject))
            {
                // Check pure prefab or not
                var overrides = PrefabUtility.GetObjectOverrides(unityGameObject, false);
                bool hasOverrides = false;
                foreach (var o in overrides)
                {
                    if (o.instanceObject is Component)
                    {
                        Component c = o.instanceObject as Component;
                        if (c.gameObject != unityGameObject)
                            hasOverrides = true;
                    }
                    else if (o.instanceObject is GameObject)
                    {
                        GameObject c = o.instanceObject as GameObject;
                        if (c != unityGameObject)
                            hasOverrides = true;
                    }
                    else
                    {
                        hasOverrides = true;
                    }
                }

                if (!hasOverrides)
                {
                    prefab = JEPrefab.RegisterPrefab(unityGameObject);
                }
            }
        }

        public void Preprocess()
        {
            foreach (var component in components)
                component.Preprocess();

            foreach (var child in children)
                child.Preprocess();
        }

        public void QueryResources()
        {
            foreach (var component in components)
                component.QueryResources();

            foreach (var child in children)
                child.QueryResources();
        }

        public void Process()
        {
            foreach (var component in components)
                component.Process();

            foreach (var child in children)
                child.Process();
        }

        public void PostProcess()
        {
            foreach (var component in components)
                component.PostProcess();

            foreach (var child in children)
                child.PostProcess();
        }

        public static void Reset()
        {
            GameObjectLookup = new Dictionary<GameObject, JEGameObject>();
        }

        public static string GetObjectId(GameObject go)
        {
            if (!GameObjectLookup.ContainsKey(go))
                return "";
            return GameObjectLookup[go].id;
        }

        public JSONGameObject ToJSON()
        {
            JSONGameObject json = new JSONGameObject();

            json.id = id;
            json.name = name;
            json.enabled = unityGameObject.activeSelf;

            json.components = new List<JSONComponent>();
            foreach (var component in components)
            {
                JSONComponent jsonComponent = component.ToJSON();
                if (jsonComponent != null)
                    json.components.Add(jsonComponent);
            }

            if (prefab != null)
            {
                json.isPrefab = true;
                json.prefabId = prefab.id;
            }
            else
            {
                json.children = new List<JSONGameObject>();
                foreach (var child in children)
                    json.children.Add(child.ToJSON());
            }

            return json;
        }
    }
}
