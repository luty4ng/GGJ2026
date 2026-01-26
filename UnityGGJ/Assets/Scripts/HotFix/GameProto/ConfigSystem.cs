using Luban;
using GameConfig;
using GameFramework;
using UnityEngine;
using UnityGameFramework;
using SimpleJSON;

namespace GameProto
{
    /// <summary>
    /// 配置加载器。
    /// </summary>
    public class ConfigSystem
    {
        private static ConfigSystem _instance;
        
        public static ConfigSystem Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigSystem();
                return _instance;
            }
        }

        private bool _init = false;

        private Tables _tables;

        public Tables Tables
        {
            get
            {
                if (!_init)
                {
                    Load();
                }

                return _tables;
            }
        }

        /// <summary>
        /// 加载配置。
        /// </summary>
        public void Load()
        {
            _tables = new Tables(LoadJson);
            _init = true;
        }

        private JSONNode LoadJson(string file)
        {
            TextAsset textAsset = GameModule.Resource.LoadAsset<TextAsset>(file);
            if (textAsset == null)
                throw new GameFrameworkException($"LoadByteBuf failed: {file}");
            GameModule.Resource.UnloadAsset(textAsset);
            return JSONNode.Parse(textAsset.text);
        }
    }
}