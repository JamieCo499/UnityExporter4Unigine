using System;
using UnityEditor;

namespace UnigineExporter
{
    static class ExportError
    {
        public static void FatalError(string message)
        {
            EditorUtility.DisplayDialog("Error", message, "Ok");
            throw new Exception(message);
        }
    }
}
