using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace UnigineExporter
{
    public class UnigineExporter : ScriptableObject
    {
        static void Reset()
        {
            PhysicsCollisionMatrix.Init();

            JEResources.Reset();
            JEComponent.Reset();
            JEGameObject.Reset();

            JEComponent.RegisterConversions();
        }

        public static JSONScene GenerateJSONScene()
        {
            Reset();

            JEScene.name = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name);

            JEScene scene = JEScene.TraverseScene();

            scene.Preprocess();
            scene.Process();
            scene.PostProcess();

            JSONScene jsonScene = scene.ToJSON() as JSONScene;

            Reset();

            return jsonScene;
        }

        [MenuItem("Tools/Unigine Exporter/Export to JSON")]
        public static void DoExport()
        {
            var defaultFileName = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name) + ".json";

            var path = EditorUtility.SaveFilePanel("Export Scene to JSON", "", defaultFileName, "json");

            if (path.Length != 0)
            {
                var jsonScene = GenerateJSONScene();
                JsonConverter[] converters = new JsonConverter[] { new BasicTypeConverter() };
                string json = JsonConvert.SerializeObject(jsonScene, Formatting.Indented, converters);
                File.WriteAllText(path, json);

                EditorUtility.DisplayDialog("Unigine Exporter", "Export Successful", "OK");
            }
        }
    }
}
