using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var ins = UnityEngine.Object.FindObjectOfType<T>();
                    if (ins != null)
                    {
                        var obj = ins.gameObject;
                        obj.name = typeof(T).Name;
                        _instance = ins;
                        SingletonManager.Cache(obj);
                        return _instance;
                    }

                    System.Type thisType = typeof(T);
                    string instName = thisType.Name;
                    GameObject go = SingletonManager.GetCachedSingleton(instName);
                    if (go == null)
                    {
                        go = GameObject.Find($"{instName}");
                        if (go == null)
                        {
                            go = new GameObject(instName);
                            go.transform.position = Vector3.zero;
                        }
                    }

                    _instance = go.GetComponent<T>();
                    if (_instance == null)
                    {
                        _instance = go.AddComponent<T>();
                    }

                    if (_instance == null)
                    {
                        Log.Error($"Can't create UnitySingleton<{typeof(T)}>");
                    }
                }

                return _instance;
            }
        }
        
        private bool CheckInstance()
        {
            if (this == Instance)
            {
                return true;
            }

            Destroy(gameObject);
            return false;
        }

        protected virtual void OnLoad()
        {
        }

        public virtual void Awake()
        {
            if (CheckInstance())
            {
                OnLoad();
            }
#if UNITY_EDITOR
            Log.Debug($"UnitySingleton Instance:{typeof(T).Name}");
#endif
            GameObject tEngine = SingletonManager.Root;
            if (tEngine != null)
            {
                this.gameObject.transform.SetParent(tEngine.transform);
            }
        }

        protected virtual void OnDestroy()
        {
            Release();
        }

        public static void Release()
        {
            if (_instance != null)
            {
                SingletonManager.Release(_instance.gameObject);
                _instance = null;
            }
        }
    }
}