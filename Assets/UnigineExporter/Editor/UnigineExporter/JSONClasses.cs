using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnigineExporter
{
    // Resources
    public class JSONTexture
    {
        public string id;
        public string relativePath;
    }

    public class JSONMaterialTexture
    {
        public string parameter;
        public string textureId;
    }

    public class JSONMaterialParameterInt
    {
        public string name;
        public int value;
    }

    public class JSONMaterialParameterFloat
    {
        public string name;
        public float value;
    }
    public class JSONMaterialParameterVector
    {
        public string name;
        public Vector4 value;
    }

    public class JSONMaterial
    {
        public string id;
        public string relativePath;
        public string shader;
        public JSONMaterialTexture[] textures;
        public JSONMaterialParameterInt[] parameters_int;
        public JSONMaterialParameterFloat[] parameters_float;
        public JSONMaterialParameterVector[] parameters_vector;
    }

    public class JSONMesh
    {
        public string id;
        public string name;
        public string relativePath;
        public string boundMeshName;
        public Vector3 boundSize;
    }

    public class JSONScriptParameter
    {
        public string name;
        public string type;
        public string valueStr;
        public string[] valueArr;
    }

    public class JSONScriptResource
    {
        public string id;
        public string relativePath;
        public JSONScriptParameter[] parameters;
    }

    public class JSONPrefab
    {
        public string id;
        public string relativePath;
        public JSONGameObject node;
        // TODO: Add modified prefabs support
    }
    // Hierarchy
    public class JSONGameObject
    {
        public string id;
        public string name;
        public List<JSONComponent> components;
        public List<JSONGameObject> children;

        public bool enabled;
        public bool isPrefab;
        public string prefabId;

        public T GetComponent<T>() where T : JSONComponent
        {
            foreach (var component in components)
                if (component.GetType() == typeof(T))
                    return (T)component;

            return null;
        }
    }

    // Components
    public class JSONComponent
    {
        public string type;
    }

    public class JSONTransform : JSONComponent
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }

    public class JSONCamera : JSONComponent
    {
        public bool perspective;
        public float nearClip;
        public float farClip;
        public float fov;
        public float height;
    }

    public class JSONLight : JSONComponent
    {
        public string lightType;
        public Color color;
        public float intensity;
        public float range;
        public bool castsShadows;
        public bool realtime;
        public float fov;
        public bool enabled;
    }

    public class JSONJointComponent : JSONComponent
    {
        public string connectedObjectId;
        public float maxForce;
        public float maxTorque;
        public bool collision;
    }

    public class JSONFixedJoint : JSONJointComponent
    {

    }

    public class JSONHingeJoint : JSONJointComponent
    {
        public Vector3 anchor;
        public Vector3 connectedAnchor;
        public Vector3 axis;
    }
    public class JSONSpringJoint : JSONJointComponent
    {
        public Vector3 anchor;
        public Vector3 connectedAnchor;
        public Vector3 axis;
    }

    public class JSONColliderComponent : JSONComponent
    {
        public bool enabled;
        public int collisionMask;
        public int exclusionMask;
    }

    public class JSONBoxCollider : JSONColliderComponent
    {
        public Vector3 center;
        public Vector3 size;
    }

    public class JSONSphereCollider : JSONColliderComponent
    {
        public Vector3 center;
        public float radius;
    }

    public class JSONCapsuleCollider : JSONColliderComponent
    {
        public Vector3 center;
        public float radius;
        public float height;
    }

    public class JSONMeshCollider : JSONColliderComponent
    {

    }

    public class JSONRigidBody : JSONComponent
    {
        public bool useGravity;
        public float mass;
    }

    public class JSONMeshRenderer : JSONComponent
    {
        public string meshId;
        public string file;
        public string mesh; // actually object name
        public string[] materialsId;
        public float minDistance;
        public float maxDistance;
        public bool enabled;
    }

    public class JSONSkinnedMeshRenderer : JSONComponent
    {
        public string meshId;
        public string file;
        public string mesh; // actually object name
        public string rootBone;
        public string[] materialsId;
        public bool enabled;
    }

    public class JSONScriptComponent : JSONComponent
    {
        public string scriptId;
        public bool enabled;
        public JSONScriptParameter[] parametersSet;
    }

    public class JSONResources
    {
        public string rootFolder;
        public List<JSONTexture> textures;
        public List<JSONMaterial> materials;
        public List<JSONMesh> meshes;
        public List<JSONScriptResource> scripts;
        public List<JSONPrefab> prefabs;
    }

    public class JSONScene
    {
        public string name;
        public JSONResources resources;
        public List<JSONGameObject> hierarchy;
    }

    public class BasicTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(Color))
                {
                    Color color = (Color)value;
                    writer.WriteStartArray();
                    writer.WriteValue(color.r);
                    writer.WriteValue(color.g);
                    writer.WriteValue(color.b);
                    writer.WriteValue(color.a);
                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(Vector2))
                {
                    Vector2 v = (Vector2)value;
                    writer.WriteStartArray();
                    writer.WriteValue(v.x);
                    writer.WriteValue(v.y);
                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(Vector3))
                {
                    Vector3 v = (Vector3)value;
                    writer.WriteStartArray();
                    writer.WriteValue(v.x);
                    writer.WriteValue(v.y);
                    writer.WriteValue(v.z);
                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(Vector4))
                {
                    Vector4 v = (Vector4)value;
                    writer.WriteStartArray();
                    writer.WriteValue(v.x);
                    writer.WriteValue(v.y);
                    writer.WriteValue(v.z);
                    writer.WriteValue(v.w);
                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(Quaternion))
                {
                    Quaternion q = (Quaternion)value;
                    writer.WriteStartArray();
                    writer.WriteValue(q.x);
                    writer.WriteValue(q.y);
                    writer.WriteValue(q.z);
                    writer.WriteValue(q.w);
                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(Matrix4x4))
                {
                    Matrix4x4 m = (Matrix4x4)value;
                    writer.WriteStartArray();

                    for (int y = 0; y < 4; y++)
                        for (int x = 0; x < 4; x++)
                            writer.WriteValue(m[y, x]);

                    writer.WriteEndArray();
                }
                else if (value.GetType() == typeof(float))
                {
                    float v = (float)value;

                    const float UNIGINE_INF = 1e+9f;
                    if (float.IsInfinity(v))
                    {
                        writer.WriteValue(UNIGINE_INF * Math.Sign(v));
                    }
                    else
                    {
                        writer.WriteValue(v);
                    }
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            Type[] types = new Type[] { typeof(Color), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Quaternion), typeof(Matrix4x4), typeof(float) };
            return Array.IndexOf(types, objectType) != -1;
        }
    }
}