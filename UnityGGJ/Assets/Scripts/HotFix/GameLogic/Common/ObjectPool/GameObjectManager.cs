using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameFramework.ObjectPool;
using UnityEngine;

namespace GameLogic
{
    public class GameObjectManager : Singleton<GameObjectManager>
    {
        private readonly Dictionary<System.Type, ObjectPoolBase> m_objectPools = new Dictionary<System.Type, ObjectPoolBase>();
        public T GetComponet<T>(string goName, Vector3 position, Transform parent = null) where T : Component
        {
            GameObjectBase gameObjectBase = Spawn<T>(goName, position, parent);
            if (gameObjectBase == null)
                return null;
            T component = gameObjectBase.GameObject.GetComponent<T>();
            return component;
        }

        public async UniTask<T> GetComponetAsync<T>(string goName, Vector3 position, Transform parent = null) where T : Component
        {
            GameObjectBase gameObjectBase = await SpawnAsync<T>(goName, position, parent);
            if (gameObjectBase == null)
                return null;
            return gameObjectBase.GameObject.GetComponent<T>();
        }
        public void Release<T>(T component) where T : Component
        {
            if (component == null)
                return;

            GameObject go = component.gameObject;
            ReleaseInternal(typeof(T), go);
        }

        private GameObjectBase Spawn<T>(string assetName, Vector3 position, Transform parent, GameObject prefabOverride = null) where T : Component
        {
            ObjectPoolBase pool = GetOrCreatePoolInternal<T>();
            if (pool == null)
                return null;
            var typedPool = pool as IObjectPool<GameObjectBase>;
            if (typedPool == null)
                return null;

            GameObjectBase objBase = typedPool.Spawn();
            if (objBase == null)
            {
                GameObject prefab = prefabOverride != null ? prefabOverride : GameModule.Resource.LoadAsset<GameObject>(assetName);
                if (prefab == null)
                {
                    Debug.LogError($"[GameObjectManager] Failed to load prefab: {assetName}");
                    return null;
                }

                GameObject instance = Object.Instantiate(prefab);
                objBase = GameObjectBase.Create(instance);
                typedPool.Register(objBase, true);
            }

            Transform t = objBase.GameObject.transform;
            if (parent != null)
                t.SetParent(parent, false);
            t.position = position;
            return objBase;
        }

        private async UniTask<GameObjectBase> SpawnAsync<T>(string assetName, Vector3 position, Transform parent) where T : Component
        {
            ObjectPoolBase pool = GetOrCreatePoolInternal<T>();
            if (pool == null)
                return null;
            var typedPool = pool as IObjectPool<GameObjectBase>;
            if (typedPool == null)
                return null;

            GameObjectBase objBase = typedPool.Spawn();
            if (objBase == null)
            {
                GameObject prefab = await GameModule.Resource.LoadAssetAsync<GameObject>(assetName);
                if (prefab == null)
                {
                    Debug.LogError($"[GameObjectManager] Failed to Load Prefab: {assetName}");
                    return null;
                }

                GameObject instance = Object.Instantiate(prefab);
                objBase = GameObjectBase.Create(instance);
                typedPool.Register(objBase, true);
            }

            Transform t = objBase.GameObject.transform;
            if (parent != null)
                t.SetParent(parent, false);
            t.position = position;
            return objBase;
        }



        private void ReleaseInternal(System.Type type, GameObject go)
        {
            if (!m_objectPools.TryGetValue(type, out ObjectPoolBase pool))
                return;
            var typedPool = pool as IObjectPool<GameObjectBase>;
            if (typedPool == null)
                return;
            typedPool.Unspawn(go);
        }

        private ObjectPoolBase GetOrCreatePoolInternal<T>() where T : Component
        {
            if (m_objectPools.TryGetValue(typeof(T), out ObjectPoolBase pool) && pool != null)
                return pool;

            System.Type objectType = typeof(GameObjectBase);
            string poolName = $"GameObjectPool_{typeof(T).Name}";
            if (GameModule.ObjectPool.HasObjectPool(objectType, poolName))
            {
                pool = GameModule.ObjectPool.GetObjectPool(objectType, poolName);
            }
            else
            {
                pool = GameModule.ObjectPool.CreateSingleSpawnObjectPool(
                    objectType,
                    poolName,
                    autoReleaseInterval: 60f,
                    capacity: 16,
                    expireTime: 300f,
                    priority: 0
                );
            }

            m_objectPools[typeof(T)] = pool;
            return pool;
        }
    }
}


