using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnigineExporter
{
    public class JEComponent
    {
        private static Dictionary<Type, Type> conversions;
        private static Dictionary<Type, string> conversionsTypes;

        public Component unityComponent;
        public JEGameObject jeGameObject;

        public virtual void QueryResources() { }
        public virtual void Preprocess() { }
        public virtual void Process() { }
        public virtual void PostProcess() { }

        public static void QueryComponents(JEGameObject jgo)
        {
            foreach (KeyValuePair<Type, Type> pair in conversions)
            {
                Component[] components = jgo.unityGameObject.GetComponents(pair.Key);

                foreach (Component component in components)
                {
                    var jcomponent = Activator.CreateInstance(pair.Value) as JEComponent;

                    if (jcomponent == null)
                    {
                        ExportError.FatalError("Export component creation failed");
                    }

                    if (component == null)
                        continue;

                    jcomponent.unityComponent = component;
                    jcomponent.jeGameObject = jgo;
                    jgo.AddComponent(jcomponent);
                }
            }
        }

        public virtual JSONComponent ToJSON()
        {
            throw new NotImplementedException();
        }

        public static void Reset()
        {
            conversions = new Dictionary<Type, Type>();
            conversionsTypes = new Dictionary<Type, string>();
        }

        public static void RegisterConversion(Type componentType, Type exportType, string type)
        {
            conversions[componentType] = exportType;
            conversionsTypes[exportType] = type; 
        }

        public string GetTypeName()
        {
            return conversionsTypes[GetType()];
        }

        public static void RegisterConversions()
        {
            RegisterConversion(typeof(Transform), typeof(JETransform), "TRANSFORM");
            RegisterConversion(typeof(MeshRenderer), typeof(JEMeshRenderer), "MESH_RENDERER");
            RegisterConversion(typeof(SkinnedMeshRenderer), typeof(JESkinnedMeshRenderer), "SKINNED_MESH_RENDERER");

            RegisterConversion(typeof(FixedJoint), typeof(JEFixedJoint), "FIXED_JOINT");
            RegisterConversion(typeof(HingeJoint), typeof(JEHingeJoint), "HINGE_JOINT");
            RegisterConversion(typeof(SpringJoint), typeof(JESpringJoint), "SPRING_JOINT");

            RegisterConversion(typeof(BoxCollider), typeof(JEBoxCollider), "BOX_COLLIDER");
            RegisterConversion(typeof(SphereCollider), typeof(JESphereCollider), "SPHERE_COLLIDER");
            RegisterConversion(typeof(CapsuleCollider), typeof(JECapsuleCollider), "CAPSULE_COLLIDER");
            RegisterConversion(typeof(MeshCollider), typeof(JEMeshCollider), "MESH_COLLIDER");

            RegisterConversion(typeof(Camera), typeof(JECamera), "CAMERA");
            RegisterConversion(typeof(Rigidbody), typeof(JERigidBody), "RIGIDBODY");
            RegisterConversion(typeof(Light), typeof(JELight), "LIGHT");
            RegisterConversion(typeof(MonoBehaviour), typeof(JEMonoBehaviour), "SCRIPT");
        }
    }
}
