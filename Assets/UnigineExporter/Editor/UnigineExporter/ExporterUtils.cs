using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnigineExporter
{
    public static class ExporterUtils
    {
        public static Vector3 ToUnigineVector(this Vector3 vec)
        {
            (vec.y, vec.z) = (vec.z, vec.y);
            return vec;
        }

        public static Quaternion ToUnigineQuaternion(this Quaternion quat)
        {
            (quat.y, quat.z) = (quat.z, quat.y);
            return quat;
        }

        public static string GetProjectRoot() => Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        public static string GetRelativePath(string globalPath) => Path.GetRelativePath(GetProjectRoot(), globalPath).Replace("\\", "/");
    }

    public static class PhysicsCollisionMatrix
    {
        private static Dictionary<int, int> masksByLayer;

        public static void Init()
        {
            masksByLayer = new Dictionary<int, int>();
            for (int i = 0; i < 32; i++)
            {
                int mask = 0;
                for (int j = 0; j < 32; j++)
                {
                    if (!Physics.GetIgnoreLayerCollision(i, j))
                    {
                        mask |= 1 << j;
                    }
                }
                masksByLayer.Add(i, mask);
            }
        }

        public static int GetMask(int layer)
        {
            return masksByLayer[layer];
        }
    }
}
