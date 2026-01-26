using GameFramework.ObjectPool;
using GameFramework;
using UnityEngine;

namespace GameLogic
{
    public class GameObjectBase : ObjectBase
    {
        public GameObject GameObject => (GameObject)Target;
        private static Transform s_root;
        private static Transform s_activeRoot;
        private static Transform s_deactiveRoot;

        public static GameObjectBase Create(GameObject effect)
        {
            GameObjectBase objBase = ReferencePool.Acquire<GameObjectBase>();
            objBase.Initialize(effect.name, effect);
            return objBase;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (GameObject != null)
            {
                Transform t = GameObject.transform;
                if (t.parent == null)
                {
                    t.SetParent(GetActiveRoot(), false);
                }
                GameObject.SetActive(true);
            }
        }

        protected override void OnUnspawn()
        {
            base.OnUnspawn();
            if (GameObject != null)
            {
                Transform t = GameObject.transform;
                t.SetParent(GetDeactiveRoot(), false);
                GameObject.SetActive(false);
            }
        }

        private static Transform GetActiveRoot()
        {
            if (s_activeRoot == null)
            {
                Transform root = EnsureRoot();
                Transform child = root.Find("Active");
                if (child == null)
                {
                    GameObject go = new GameObject("Active");
                    child = go.transform;
                    child.SetParent(root, false);
                }
                s_activeRoot = child;
            }
            return s_activeRoot;
        }

        private static Transform GetDeactiveRoot()
        {
            if (s_deactiveRoot == null)
            {
                Transform root = EnsureRoot();
                Transform child = root.Find("Deactive");
                if (child == null)
                {
                    GameObject go = new GameObject("Deactive");
                    child = go.transform;
                    child.SetParent(root, false);
                }
                s_deactiveRoot = child;
            }
            return s_deactiveRoot;
        }

        private static Transform EnsureRoot()
        {
            if (s_root != null)
                return s_root;

            GameObject rootGo = GameObject.Find("PooledRoot");
            if (rootGo == null)
            {
                rootGo = new GameObject("PooledRoot");
                Object.DontDestroyOnLoad(rootGo);
            }

            s_root = rootGo.transform;
            return s_root;
        }

        protected override void Release(bool isShutdown)
        {
            if (GameObject != null)
            {
                Object.Destroy(GameObject);
            }
        }
    }
}


