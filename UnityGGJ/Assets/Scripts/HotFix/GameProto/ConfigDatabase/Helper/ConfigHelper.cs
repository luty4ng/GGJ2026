using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GameFramework;
using LitJson;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameConfig
{
    public static class ConfigHelper
    {
        public static string ToJson<T>(T config) where T : ScriptableObject
        {
            return JsonMapper.ToJson(config);
        }

        public static T ToConfig<T>(string json) where T : ScriptableObject
        {
            return JsonMapper.ToObject<T>(json);
        }

        public static object ToConfig(Type objectType, string json)
        {
            return JsonMapper.ToObject(json, objectType);
        }

        public static void ToJsonFile<T>(T config, string path) where T : ScriptableObject
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is null or empty.", nameof(path));

            var fullPath = GetFullPath(path);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                var json = JsonMapper.ToJson(config);
                File.WriteAllText(fullPath, json, new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConfigHelper] ToJsonFile failed. Path={fullPath}\n{e}");
                throw;
            }
        }

        public static void ToConfigFile<T>(string json, string path) where T : ScriptableObject
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Json is null or empty.", nameof(json));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path is null or empty.", nameof(path));

            var assetDir = NormalizeAssetDirectory(path);
            EnsureDirectory(assetDir);

            T temp = null;
            try
            {
                var type = typeof(T);
                var data = JsonMapper.ToObject(json);

                temp = ScriptableObject.CreateInstance<T>();
                FromJsonOverwrite(data, temp);

                var fileName = BuildAssetFileName(type, temp);
                var assetPath = CombineAssetPath(assetDir, fileName + ".asset");

                var target = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (target == null)
                {
                    temp.name = fileName;
                    AssetDatabase.CreateAsset(temp, assetPath);
                    temp = null;
                }
                else
                {
                    FromJsonOverwrite(data, target);
                    EditorUtility.SetDirty(target);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConfigHelper] ToConfigFile failed. Type={typeof(T).FullName} Path={path}\n{e}");
                throw;
            }
            finally
            {
                if (temp != null)
                    UnityEngine.Object.DestroyImmediate(temp);
            }
#else
            throw new NotSupportedException("ToConfigFile requires UNITY_EDITOR (AssetDatabase).");
#endif
        }

        private static string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            string unityRootPath = Application.dataPath.Replace("Assets", "");
            return Path.Combine(unityRootPath, path);
        }

#if UNITY_EDITOR
        private static void FromJsonOverwrite(JsonData data, object target)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!data.IsObject)
                throw new ArgumentException("Root json must be an object.", nameof(data));

            var type = target.GetType();
            var dict = (IDictionary)data;
            foreach (var keyObj in dict.Keys)
            {
                var key = keyObj as string;
                if (string.IsNullOrEmpty(key))
                    continue;

                var valueData = data[key];
                TrySetMember(type, target, key, valueData);
            }
        }

        private static bool TrySetMember(Type type, object target, string key, JsonData valueData)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var field = type.GetField(key, flags);
            if (field == null)
            {
                var mapped = MapToPrivateFieldName(key);
                if (!string.IsNullOrEmpty(mapped))
                    field = type.GetField(mapped, flags);
            }

            if (field != null)
            {
                var value = ConvertJsonData(valueData, field.FieldType);
                field.SetValue(target, value);
                return true;
            }

            var prop = type.GetProperty(key, flags);
            if (prop != null && prop.CanWrite)
            {
                var value = ConvertJsonData(valueData, prop.PropertyType);
                prop.SetValue(target, value);
                return true;
            }

            return false;
        }

        private static object ConvertJsonData(JsonData data, Type targetType)
        {
            if (data == null)
                return null;

            if (targetType == typeof(string))
                return data.IsString ? (string)data : JsonMapper.ToJson(data);

            if (typeof(UnityEngine.Object).IsAssignableFrom(targetType))
                return null;

            if (data.IsNull())
                return null;

            var json = JsonMapper.ToJson(data);
            return JsonMapper.ToObject(json, targetType);
        }

        private static string MapToPrivateFieldName(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var firstLower = char.ToLowerInvariant(key[0]) + key.Substring(1);
            return "m_" + firstLower;
        }

        private static string BuildAssetFileName(Type type, ScriptableObject instance)
        {
            var attr = type.GetCustomAttributes(typeof(ConfigSourceAttribute), false)
                .FirstOrDefault() as ConfigSourceAttribute;

            var configName = attr?.ConfigName;
            if (string.IsNullOrEmpty(configName))
                configName = type.Name;

            var idMemberName = attr?.IdMemberName;
            if (!string.IsNullOrEmpty(idMemberName))
            {
                var id = TryGetIdValue(type, instance, idMemberName);
                if (id != null && !string.IsNullOrEmpty(id.ToString()))
                    return $"{configName}_{id}";
            }

            return configName;
        }

        private static object TryGetIdValue(Type type, ScriptableObject instance, string idMemberName)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var prop = type.GetProperty(idMemberName, flags);
            if (prop != null && prop.CanRead)
                return prop.GetValue(instance);
            var field = type.GetField(idMemberName, flags);
            if (field != null)
                return field.GetValue(instance);
            var mapped = MapToPrivateFieldName(idMemberName);
            field = type.GetField(mapped, flags);
            return field?.GetValue(instance);
        }

        private static string NormalizeAssetDirectory(string path)
        {
            var normalized = path.Replace('\\', '/').TrimEnd('/');
            if (normalized.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                normalized = Path.GetDirectoryName(normalized)?.Replace('\\', '/');

            if (string.IsNullOrEmpty(normalized))
                throw new ArgumentException("Invalid asset directory path.", nameof(path));

            if (!normalized.StartsWith("Assets/", StringComparison.Ordinal) && normalized != "Assets")
                throw new ArgumentException("Asset directory must be under Assets/.", nameof(path));

            return normalized;
        }

        private static void EnsureDirectory(string assetDir)
        {
            var full = Path.GetFullPath(assetDir);
            if (!Directory.Exists(full))
                Directory.CreateDirectory(full);
        }

        private static string CombineAssetPath(string dir, string fileName)
        {
            var d = dir.Replace('\\', '/').TrimEnd('/');
            return $"{d}/{fileName}";
        }
#endif
    }
}