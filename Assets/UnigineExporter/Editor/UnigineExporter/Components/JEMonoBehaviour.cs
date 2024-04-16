using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnigineExporter
{
    public class JEMonoBehaviour : JEComponent
    {
        MonoBehaviour unityScript;
        JEScript script;

        override public void Preprocess()
        {
            unityScript = unityComponent as MonoBehaviour;
        }

        public override void QueryResources()
        {
            script = JEScript.RegisterScript(unityScript);
        }

        public override JSONComponent ToJSON()
        {
            if (script == null)
                return null;
            
            var json = new JSONScriptComponent
            {
                type = GetTypeName(),
                scriptId = script.id,
                enabled = unityScript.enabled
            };

            List<JSONScriptParameter> parameters = new List<JSONScriptParameter>();

            FieldInfo[] fields = unityScript.GetType().GetFields();
            foreach (var field in fields)
            {
                var parameter = new JSONScriptParameter
                {
                    name = field.Name,
                    type = ""
                };
                JEScript.UnityFieldToUnigine(field, unityScript, ref parameter.type, ref parameter.valueStr, ref parameter.valueArr);
                if (parameter.type == "")
                {
                    continue;
                }
                parameters.Add(parameter);
            }

            json.parametersSet = parameters.ToArray();

            return json;
        }
    }
}
