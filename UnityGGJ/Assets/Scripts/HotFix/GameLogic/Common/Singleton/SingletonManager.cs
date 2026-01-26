using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    /// <summary>
    /// 单例接口
    /// </summary>
    public interface ISingleton
    {
        void Active();

        void Release();
    }
    
    /// <summary>
    /// 通用单例。
    /// </summary>
    /// <typeparam name="T">泛型T。</typeparam>
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new T();
                    Log.Assert(_instance != null);
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// 单例管理器(统一化持久和释放)。
    /// </summary>
    public static class SingletonManager
    {
        private static List<ISingleton> _singletonList;
        private static Dictionary<string, GameObject> _gameObjects;
        private static GameObject _root;

        public static GameObject Root
        {
            get
            {
                if (_root == null)
                {
                    _root = GameObject.Find("[SingletonMgr]");

                    if (_root == null)
                    {
                        _root = new GameObject("[SingletonMgr]")
                        {
                            transform =
                            {
                                position = Vector3.zero
                            }
                        };
                    }
                    UnityEngine.Object.DontDestroyOnLoad(_root);
                }
                return _root;
            }
        }

        public static void Retain(ISingleton go)
        {
            if (_singletonList == null)
            {
                _singletonList = new List<ISingleton>();
            }

            _singletonList.Add(go);
        }

        public static void Cache(GameObject go)
        {
            if (_gameObjects == null)
            {
                _gameObjects = new Dictionary<string, GameObject>();
            }

            if (!_gameObjects.ContainsKey(go.name))
            {
                _gameObjects.Add(go.name, go);
                if (Application.isPlaying)
                {
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
            }
        }

        public static void Release(GameObject go)
        {
            if (_gameObjects != null && _gameObjects.ContainsKey(go.name))
            {
                _gameObjects.Remove(go.name);
                UnityEngine.Object.Destroy(go);
            }
        }

        public static void Release(ISingleton go)
        {
            if (_singletonList != null && _singletonList.Contains(go))
            {
                _singletonList.Remove(go);
            }
        }

        public static void Release()
        {
            if (_gameObjects != null)
            {
                foreach (var item in _gameObjects)
                {
                    UnityEngine.Object.Destroy(item.Value);
                }

                _gameObjects.Clear();
            }

            if (_singletonList != null)
            {
                for (int i = 0; i < _singletonList.Count; ++i)
                {
                    _singletonList[i].Release();
                }

                _singletonList.Clear();
            }

            Resources.UnloadUnusedAssets();
        }

        public static GameObject GetCachedSingleton(string name)
        {
            GameObject go = null;
            if (_gameObjects != null)
            {
                _gameObjects.TryGetValue(name, out go);
            }

            return go;
        }

        internal static bool ContainsKey(string name)
        {
            if (_gameObjects != null)
            {
                return _gameObjects.ContainsKey(name);
            }

            return false;
        }

        internal static ISingleton GetSingleton(string name)
        {
            for (int i = 0; i < _singletonList.Count; ++i)
            {
                if (_singletonList[i].ToString() == name)
                {
                    return _singletonList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 释放所有单例。
        /// </summary>
        public static void ReStart()
        {
            Release();

            SceneManager.LoadScene(0);
        }
    }
}