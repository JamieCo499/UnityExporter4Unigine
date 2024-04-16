using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JEMaterial
    {
        private static Dictionary<Material, JEMaterial> allMaterials;

        public string id;
        private string relativePath = "";
        private Material unityMaterial;

        private Dictionary<string, JETexture> textures = new Dictionary<string, JETexture>();

        private static string GetPath(Material material) => ExporterUtils.GetRelativePath(Path.GetFullPath(AssetDatabase.GetAssetPath(material)));

        public static string GetNewID()
        {
            return allMaterials.Count.ToString();
        }

        private JEMaterial(Material material)
        {
            this.unityMaterial = material;
            this.id = GetNewID();
            allMaterials[material] = this;

            relativePath = GetPath(material);

            var shader = material.shader;

            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                //Debug.Log(ShaderUtil.GetPropertyName(shader.unityShader, i));
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    Texture texture = material.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                    if (texture != null)
                    {
                        var tex = JETexture.RegisterTexture(texture);
                        textures[ShaderUtil.GetPropertyName(shader, i)] = tex;
                    }
                }
            }
        }

        public static void Reset()
        {
            allMaterials = new Dictionary<Material, JEMaterial>();
        }

        public static JEMaterial RegisterMaterial(Material material)
        {
            foreach (Material m in allMaterials.Keys)
            {
                if (m.name == material.name)
                    return allMaterials[m];
            }

            return new JEMaterial(material);
        }

        public JSONMaterial ToJSON()
        {
            var json = new JSONMaterial
            {
                id = id,
                relativePath = relativePath,
                shader = unityMaterial.shader.name,

                textures = textures.Select(x => new JSONMaterialTexture() { parameter = x.Key, textureId = x.Value.id }).ToArray()
            };

            List<JSONMaterialParameterInt> parametersInt = new List<JSONMaterialParameterInt>();
            foreach (string propertyName in unityMaterial.GetPropertyNames(MaterialPropertyType.Int))
                parametersInt.Add(new JSONMaterialParameterInt() { name = propertyName, value = unityMaterial.GetInteger(propertyName) });
            json.parameters_int = parametersInt.ToArray();

            List<JSONMaterialParameterFloat> parametersFloat = new List<JSONMaterialParameterFloat>();
            foreach (string propertyName in unityMaterial.GetPropertyNames(MaterialPropertyType.Float))
                parametersFloat.Add(new JSONMaterialParameterFloat() { name = propertyName, value = unityMaterial.GetFloat(propertyName) });
            json.parameters_float = parametersFloat.ToArray();

            List<JSONMaterialParameterVector> parametersVector = new List<JSONMaterialParameterVector>();
            foreach (string propertyName in unityMaterial.GetPropertyNames(MaterialPropertyType.Vector))
                parametersVector.Add(new JSONMaterialParameterVector() { name = propertyName, value = unityMaterial.GetVector(propertyName) });
            json.parameters_vector = parametersVector.ToArray();

            return json;
        }

        public static List<JSONMaterial> GenerateJSONMaterialList()
        {
            List<JSONMaterial> materials = new List<JSONMaterial>();

            foreach (var material in allMaterials.Values)
                materials.Add(material.ToJSON());

            return materials;
        }
    }
}
