namespace UnigineExporter
{
    public class JEResources
    {
        public static void Reset()
        {
            JEMesh.Reset();
            JEMaterial.Reset();
            JETexture.Reset();
            JEScript.Reset();
            JEPrefab.Reset();
        }

        public static void Preprocess()
        {
            JEPrefab.Preprocess();
        }

        public static void Process()
        {
            JEPrefab.Process();
        }

        public static void PostProcess()
        {
            JEPrefab.PostProcess();
        }

        public static JSONResources ToJSON()
        {
            var json = new JSONResources
            {
                rootFolder = ExporterUtils.GetProjectRoot(),
                textures = JETexture.GenerateJSONTextureList(),
                materials = JEMaterial.GenerateJSONMaterialList(),
                meshes = JEMesh.GenerateJSONMeshList(),
                scripts = JEScript.GenerateJSONScriptsList(),
                prefabs = JEPrefab.GenerateJSONPrefabList()
            };

            return json;
        }
    }
}
