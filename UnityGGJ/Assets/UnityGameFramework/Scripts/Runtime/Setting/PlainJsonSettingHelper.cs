using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public class PlainJsonSettingHelper : SettingHelperBase
    {
        private const string SettingFileName = "PlainJsonSetting.json";
        private SortedDictionary<string, string> m_settings = new SortedDictionary<string, string>(StringComparer.Ordinal);
        private string m_filePath = null;
        public override int Count => m_settings.Count;
        public override bool Load()
        {
            try
            {
                if (!File.Exists(m_filePath))
                    return true;
 
                string settingText = File.ReadAllText(m_filePath);
                m_settings = Utility.Json.ToObject<SortedDictionary<string, string>>(settingText);
                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Load settings failure with exception '{0}'.", exception);
                return false;
            }
        }

        public override bool Save()
        {
            try
            {
                string settingText = Utility.Json.ToJson(m_settings);
                File.WriteAllText(m_filePath, settingText, Encoding.UTF8);
                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Save settings failure with exception '{0}'.", exception);
                return false;
            }
        }

        public override string[] GetAllSettingNames()
        {
            int index = 0;
            string[] allSettingNames = new string[m_settings.Count];
            foreach (KeyValuePair<string, string> setting in m_settings)
            {
                allSettingNames[index++] = setting.Key;
            }
            return allSettingNames;
        }
        
        public override void GetAllSettingNames(List<string> results)
        {
            if (results == null)
            {
                throw new GameFrameworkException("Results is invalid.");
            }
            results.Clear();
            foreach (KeyValuePair<string, string> setting in m_settings)
            {
                results.Add(setting.Key);
            }
        }

        public override bool HasSetting(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new GameFrameworkException("Setting name is invalid.");
            }
            return m_settings.ContainsKey(settingName);
        }
        
        public override bool RemoveSetting(string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new GameFrameworkException("Setting name is invalid.");
            }
            return m_settings.Remove(settingName);
        }
        
        public override void RemoveAllSettings()
        {
            m_settings.Clear();
        }
        
        public override bool GetBool(string settingName)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return false;
            }
            return int.Parse(value) != 0;
        }

        public override bool GetBool(string settingName, bool defaultValue)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                return defaultValue;
            }
            return int.Parse(value) != 0;
        }

        public override void SetBool(string settingName, bool value)
        {
            m_settings[settingName] = value ? "1" : "0";
        }
        
        public override int GetInt(string settingName)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return 0;
            }
            return int.Parse(value);
        }
        
        public override int GetInt(string settingName, int defaultValue)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                return defaultValue;
            }
            return int.Parse(value);
        }
        
        public override void SetInt(string settingName, int value)
        {
            m_settings[settingName] = value.ToString();
        }
        
        public override float GetFloat(string settingName)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return 0f;
            }
            return float.Parse(value);
        }
        
        public override float GetFloat(string settingName, float defaultValue)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                return defaultValue;
            }
            return float.Parse(value);
        }
        
        public override void SetFloat(string settingName, float value)
        {
            m_settings[settingName] = value.ToString();
        }
        
        public override string GetString(string settingName)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                Log.Warning("Setting '{0}' is not exist.", settingName);
                return null;
            }
            return value;
        }
        
        public override string GetString(string settingName, string defaultValue)
        {
            string value = null;
            if (!m_settings.TryGetValue(settingName, out value))
            {
                return defaultValue;
            }
            return value;
        }
        
        public override void SetString(string settingName, string value)
        {
            m_settings[settingName] = value;
        }
        
        public override T GetObject<T>(string settingName)
        {
            return Utility.Json.ToObject<T>(GetString(settingName));
        }
        
        public override object GetObject(Type objectType, string settingName)
        {
            return Utility.Json.ToObject(objectType, GetString(settingName));
        }
        
        public override T GetObject<T>(string settingName, T defaultObj)
        {
            string json = GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return Utility.Json.ToObject<T>(json);
        }
        
        public override object GetObject(Type objectType, string settingName, object defaultObj)
        {
            string json = GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }
            return Utility.Json.ToObject(objectType, json);
        }
        
        public override void SetObject<T>(string settingName, T obj)
        {
            string json = Utility.Json.ToJson(obj);
            SetString(settingName, json);
        }
        
        public override void SetObject(string settingName, object obj)
        {
            string json = Utility.Json.ToJson(obj);
            SetString(settingName, json);
        }
        
        private void Awake()
        {
            m_filePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, SettingFileName));
        }
    }
}