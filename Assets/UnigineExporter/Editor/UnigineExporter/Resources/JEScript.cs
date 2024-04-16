using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEScript
    {
        private static Dictionary<string, JEScript> allScripts = new Dictionary<string, JEScript>();

        public string id;
        private MonoBehaviour unityScript;
        private string relativePath;

        public static string GetPath(MonoBehaviour monoBehaviour)
        {
            string assetPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(monoBehaviour));
            if (string.IsNullOrEmpty(assetPath))
                return "";
            return ExporterUtils.GetRelativePath(Path.GetFullPath(assetPath));
        }
        public static string GetNewID()
        {
            return allScripts.Count.ToString();
        }

        private JEScript(MonoBehaviour script)
        {
            this.unityScript = script;
            this.id = GetNewID();
            this.relativePath = GetPath(script);
            allScripts[relativePath] = this;
        }

        public static JEScript RegisterScript(MonoBehaviour script)
        {
            string path = GetPath(script);
            if (string.IsNullOrEmpty(path))
                return null;
            if (allScripts.ContainsKey(path))
                return allScripts[path];

            return new JEScript(script);
        }

        public static void Reset()
        {
            allScripts = new Dictionary<string, JEScript>();
        }

        public static void UnityFieldToUnigine(FieldInfo field, MonoBehaviour instance, ref string typeStr, ref string valueStr, ref string[] valueArr)
        {
            Type type = field.FieldType;
            if (type == typeof(int))
                typeStr = "int";
            else if (type == typeof(float))
                typeStr = "float";
            else if (type == typeof(double))
                typeStr = "double";
            else if (type == typeof(string))
                typeStr = "string";
            else if (type == typeof(bool))
                typeStr = "bool";
            else if (type == typeof(Vector2))
                typeStr = "vec2";
            else if (type == typeof(Vector3))
                typeStr = "vec3";
            else if (type == typeof(Vector4))
                typeStr = "vec4";
            else if (type == typeof(Vector2Int))
                typeStr = "ivec2";
            else if (type == typeof(Vector3Int))
                typeStr = "ivec3";
            else if (type == typeof(GameObject))
                typeStr = "Node";
            else if (type == typeof(Material))
                typeStr = "Material";

            if (instance != null)
            {
                object value = field.GetValue(instance);
                if (type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(string))
                    valueStr = value.ToString();
                else if (type == typeof(bool))
                    valueStr = ((bool)value) ? "1" : "0";
                else if (type == typeof(Vector2))
                {
                    Vector2 vec = ((Vector2)value);
                    valueArr = new string[] { vec.x.ToString(), vec.y.ToString() };
                }
                else if (type == typeof(Vector3))
                {
                    Vector3 vec = ((Vector3)value);
                    valueArr = new string[] { vec.x.ToString(), vec.y.ToString(), vec.z.ToString() };
                }
                else if (type == typeof(Vector4))
                {
                    Vector4 vec = ((Vector4)value);
                    valueArr = new string[] { vec.x.ToString(), vec.y.ToString(), vec.z.ToString(), vec.w.ToString() };
                }
                else if (type == typeof(Vector2Int))
                {
                    Vector2Int vec = ((Vector2Int)value);
                    valueArr = new string[] { vec.x.ToString(), vec.y.ToString() };
                }
                else if (type == typeof(Vector3Int))
                {
                    Vector3Int vec = ((Vector3Int)value);
                    valueArr = new string[] { vec.x.ToString(), vec.y.ToString(), vec.z.ToString() };
                }
                else if (type == typeof(GameObject))
                    typeStr = "Node"; // TODO: Add
                else if (type == typeof(Material))
                    typeStr = "Material"; // TODO: Add
            }
        }

        public JSONScriptResource ToJSON()
        {
            var json = new JSONScriptResource
            {
                id = id,
                relativePath = relativePath
            };


            List<JSONScriptParameter> parameters = new List<JSONScriptParameter>();

            FieldInfo[] fields = unityScript.GetType().GetFields();
            foreach (var field in fields)
            {
                // TODO: Add default values
                var parameter = new JSONScriptParameter
                {
                    name = field.Name,
                    type = ""
                };
                UnityFieldToUnigine(field, null, ref parameter.type, ref parameter.valueStr, ref parameter.valueArr);
                
                if (parameter.type == "")
                {
                    continue;
                }
                parameters.Add(parameter);
            }

            json.parameters = parameters.ToArray();

            return json;
        }

        public static List<JSONScriptResource> GenerateJSONScriptsList()
        {
            List<JSONScriptResource> scripts = new List<JSONScriptResource>();

            foreach (var script in allScripts.Values)
                scripts.Add(script.ToJSON());

            return scripts;
        }
    }
}
