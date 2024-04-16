using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnigineExporter
{
    public class JETexture
    {
        public string id;
        public string relativePath = "";

        public static Dictionary<string, JETexture> allTextures;

        public static string GetPath(Texture texture) => ExporterUtils.GetRelativePath(Path.GetFullPath(AssetDatabase.GetAssetPath(texture)));

        public static string GetNewID()
        {
            return allTextures.Count.ToString();
        }

        private JETexture(Texture texture)
        {
            relativePath = GetPath(texture);
            allTextures[relativePath] = this;
            id = GetNewID();
        }

        public static JETexture RegisterTexture(Texture texture)
        {
            string path = GetPath(texture);
            if (allTextures.ContainsKey(path))
                return allTextures[path];

            return new JETexture(texture);
        }

        public static void Reset()
        {
            allTextures = new Dictionary<String, JETexture>();
        }

        public JSONTexture ToJSON()
        {
            var json = new JSONTexture
            {
                id = id,
                relativePath = relativePath
            };

            return json;
        }

        public static List<JSONTexture> GenerateJSONTextureList()
        {
            List<JSONTexture> textures = new List<JSONTexture>();

            foreach (var texture in allTextures.Values)
                textures.Add(texture.ToJSON());

            return textures;
        }
    }
}
