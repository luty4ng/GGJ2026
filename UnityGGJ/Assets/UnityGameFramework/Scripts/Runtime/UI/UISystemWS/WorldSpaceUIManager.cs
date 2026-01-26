using System.Collections.Generic;
using UnityEngine;


namespace UnityGameFramework.Runtime
{
    public class WorldspaceUIManager : GameFrameworkComponent
    {
        public Canvas worldCanvas;
        public Camera WorldCamera;
        private readonly Dictionary<Transform, List<WorldspaceUIBase>> m_target2Uis = new Dictionary<Transform, List<WorldspaceUIBase>>();
        private readonly Dictionary<string, GameObject> m_uiPrefabs = new Dictionary<string, GameObject>();

        protected override void Awake()
        {
            base.Awake();
            if (worldCanvas == null)
            {
                worldCanvas = GetComponentInChildren<Canvas>();
            }

            if (WorldCamera == null)
            {
                WorldCamera = Camera.main;
            }

            if (worldCanvas != null && WorldCamera != null)
            {
                worldCanvas.renderMode = RenderMode.WorldSpace;
                worldCanvas.worldCamera = WorldCamera;
            }
        }

        public T ShowUI<T>(Transform attachTarget, WorldSpaceUIFaceMode faceMode, object userData = null) where T : WorldspaceUIBase
        {
            GameObject prefab = LoadPrefab(typeof(T).Name);
            if (prefab == null || attachTarget == null)
                return null;

            GameObject go = Instantiate(prefab, worldCanvas.transform);
            go.hideFlags = HideFlags.None;
            T ui = go.GetOrAddComponent<T>();
            ui.Initialize(attachTarget, WorldCamera, faceMode, userData);
            attachTarget.GetOrAddComponent<WorldspaceUIAutoDestroyer>();
            if (!m_target2Uis.TryGetValue(attachTarget, out var list))
            {
                list = new List<WorldspaceUIBase>();
                m_target2Uis[attachTarget] = list;
            }

            list.Add(ui);
            return ui;
        }

        private List<WorldspaceUIBase> m_toRemoveUiList = new List<WorldspaceUIBase>();

        public void CloseUI<T>(Transform target) where T : WorldspaceUIBase
        {
            if (target != null && m_target2Uis.TryGetValue(target, out var list))
            {
                foreach (var ui in list)
                    if (ui is T)
                        m_toRemoveUiList.Add(ui);

                for (int i = 0; i < m_toRemoveUiList.Count; i++)
                {
                    m_target2Uis[target].Remove(m_toRemoveUiList[i]);
                    if (m_toRemoveUiList[i] != null)
                        Destroy(m_toRemoveUiList[i].gameObject);
                }
            }
            m_toRemoveUiList.Clear();
        }

        public void CloseAllUIOnTarget(Transform target)
        {
            if (target != null && m_target2Uis.TryGetValue(target, out var list))
            {
                foreach (var ui in list)
                    if (ui != null)
                        Destroy(ui.gameObject);
                list.Clear();
            }
        }


        private GameObject LoadPrefab(string assetName)
        {
            if (m_uiPrefabs.ContainsKey(assetName))
                return m_uiPrefabs[assetName];
            GameObject prefab = GameModule.Resource.LoadAsset<GameObject>($"{assetName}");
            prefab.hideFlags = HideFlags.HideInHierarchy;
            if (prefab == null)
                return null;
            m_uiPrefabs[assetName] = prefab;
            return prefab;
        }

        public void ClearAll()
        {
            foreach (var kv in m_target2Uis)
            {
                var list = kv.Value;
                foreach (var ui in list)
                {
                    if (ui != null)
                    {
                        Destroy(ui.gameObject);
                    }
                }
            }

            m_target2Uis.Clear();
        }
    }
}