using System.Diagnostics;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ControlPanel : OdinEditorWindow
{
    #region Constants
    private const string k_TabGroup = "Tabs";
    private const string k_TabBuild = "构建流程";
    private const string k_TabScenes = "场景管理";
    private const string k_DataBuildGroup = k_TabGroup + "/" + k_TabBuild + "/DataBuildGroup";
    private const string k_ScenesLaunchGroup = k_TabGroup + "/" + k_TabScenes + "/ScenesLaunchGroup";
    private const string k_ScenesAccessGroup = k_TabGroup + "/" + k_TabScenes + "/ScenesAccessGroup";
    private const string k_GameLauncherScene = "GameLauncher";

    private static readonly Color k_GreenColor = new Color(0.7f, 0.9f, 0.7f);
    private static readonly Color k_BlueColor = new Color(0.6f, 0.8f, 1f);
    private static readonly Color k_OrangeColor = new Color(1f, 0.85f, 0.6f);
    private static readonly Color k_PurpleColor = new Color(0.8f, 0.8f, 1f);
    #endregion

    [TabGroup(k_TabGroup, k_TabBuild)]
    [BoxGroup(k_DataBuildGroup, ShowLabel = true, CenterLabel = true, LabelText = "数据打表")]
    [LabelText("数据表脚本"), LabelWidth(70)]
    [ValueDropdown(nameof(GetConfigBatFiles))]
    public string buildDataBatFile;

    [TabGroup(k_TabGroup, k_TabScenes)]
    [BoxGroup(k_ScenesLaunchGroup, ShowLabel = true, CenterLabel = true, LabelText = "场景启动")]
    [LabelText("重定向场景"), LabelWidth(70)]
    [ValueDropdown(nameof(GetRedirectScenes)), OnValueChanged(nameof(ApplyRedirectScene))]
    public string redirectSceneName;

    [MenuItem("Window/控制面板")]
    public static void ShowWindow()
    {
        var window = GetWindow<ControlPanel>(false, "控制面板", true);
        window.minSize = new Vector2(360, 120);
    }

    [TabGroup(k_TabGroup, k_TabBuild)]
    [BoxGroup(k_DataBuildGroup)]
    [Button("打表", ButtonSizes.Large), GUIColor(nameof(k_GreenColor))]
    private void RunSelectedBatch()
    {
        RunSingleBatch(buildDataBatFile);
    }

    [TabGroup(k_TabGroup, k_TabScenes)]
    [BoxGroup(k_ScenesLaunchGroup)]
    [Button("智能启动", ButtonSizes.Large), GUIColor(nameof(k_BlueColor))]
    private void Launch()
    {
        var count = EditorSceneManager.sceneCount;
        bool hasLauncher = false;
        bool hasOther = false;
        for (int i = 0; i < count; i++)
        {
            var s = EditorSceneManager.GetSceneAt(i);
            if (!s.IsValid()) 
                continue;
            var name = string.IsNullOrEmpty(s.path) ? s.name : Path.GetFileNameWithoutExtension(s.path);
            if (name == k_GameLauncherScene) 
                hasLauncher = true; 
            else 
                hasOther = true;
        }

        if (hasLauncher && !hasOther)
        {
            GameLogic.ProcedureStartGame.IgnoreStartLogic = false;
            EditorApplication.isPlaying = true;
            return;
        }

        if (hasLauncher && hasOther)
        {
            GameLogic.ProcedureStartGame.IgnoreStartLogic = true;
            EditorApplication.isPlaying = true;
            return;
        }

        if (!hasLauncher && hasOther)
        {
            GameLogic.ProcedureStartGame.IgnoreStartLogic = true;
            LoadOrUnloadLauncher();
            EditorApplication.isPlaying = true;
            return;
        }

        GameLogic.ProcedureStartGame.IgnoreStartLogic = false;
        EditorApplication.isPlaying = true;
    }

    // [TabGroup(k_TabGroup, k_TabScenes)]
    // [BoxGroup(k_ScenesLaunchGroup)]
    // [Button("准备启动器", ButtonSizes.Medium), GUIColor(nameof(k_PurpleColor))]
    // [DisableIf("@EditorApplication.isPlaying")]
    // private void ToggleGameLauncher()
    // {
    //     LoadOrUnloadLauncher();
    // }

    [Space]
    [TabGroup(k_TabGroup, k_TabScenes)]
    [BoxGroup(k_ScenesAccessGroup, ShowLabel = true, CenterLabel = true, LabelText = "场景访问")]
    [ShowInInspector]
    [ListDrawerSettings(ShowPaging = true, ShowIndexLabels = false, HideAddButton = true,
         HideRemoveButton = true, DraggableItems = false, ShowFoldout = false)]
    [DisableContextMenu]
    private List<SceneInfo> m_SceneList = new List<SceneInfo>();

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshSceneList();
    }

    protected override void OnImGUI()
    {
        var style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 16;
        GUILayout.Space(6);
        GUILayout.Label("控制面板", style, GUILayout.Height(24));
        GUILayout.Space(4);

        base.OnImGUI();
    }

    [System.Serializable]
    [HideLabel]
    public struct SceneInfo
    {
        [HideInInspector]
        public string sceneName;

        [HideInInspector]
        public string scenePath;

        [Button("@sceneName", ButtonSizes.Medium), GUIColor("@ControlPanel.k_GreenColor")]
        [ShowInInspector]
        public void OpenScene()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("提示", "运行时无法切换场景", "确定");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
        }
    }

    [BoxGroup(k_ScenesAccessGroup)]
    [Button("刷新场景列表", ButtonSizes.Medium), GUIColor(nameof(k_PurpleColor))]
    private void RefreshSceneList()
    {
        m_SceneList.Clear();

        var guids = AssetDatabase.FindAssets("t:scene", new[] { "Assets/Scenes" });
        if (guids != null && guids.Length > 0)
        {
            foreach (var guid in guids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    m_SceneList.Add(new SceneInfo
                    {
                        sceneName = sceneName,
                        scenePath = scenePath
                    });
                }
            }
        }

        // 按名称排序
        m_SceneList = m_SceneList.OrderBy(s => s.sceneName).ToList();
    }

    private static System.Collections.Generic.IEnumerable<ValueDropdownItem<string>> GetConfigBatFiles()
    {
        var assetsPath = Application.dataPath;
        var configDir = Path.GetFullPath(Path.Combine(assetsPath, "..", "..", "Configs", "GameConfig"));
        if (!Directory.Exists(configDir))
            return System.Array.Empty<ValueDropdownItem<string>>();
        return Directory
            .GetFiles(configDir, "*.bat", SearchOption.TopDirectoryOnly)
            .Select(p => new ValueDropdownItem<string>(Path.GetFileNameWithoutExtension(p), p));
    }

    private static System.Collections.Generic.IEnumerable<ValueDropdownItem<string>> GetRedirectScenes()
    {
        var items = new System.Collections.Generic.List<ValueDropdownItem<string>>();
        items.Add(new ValueDropdownItem<string>("不重定向", string.Empty));
        var guids = AssetDatabase.FindAssets("t:scene", new[] { "Assets/Scenes" });
        items.AddRange(guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(p => Path.GetFileNameWithoutExtension(p))
            .Where(name => name != k_GameLauncherScene)
            .Select(name => new ValueDropdownItem<string>(name, name)));
        return items;
    }

    private void ApplyRedirectScene()
    {
        GameLogic.ProcedureStartGame.RedirectScene = redirectSceneName;
    }

    private static void RunSingleBatch(string batPath)
    {
        if (string.IsNullOrEmpty(batPath))
            return;
        var workDir = Path.GetDirectoryName(batPath);
        var psi = new ProcessStartInfo
        {
            FileName = Path.GetFileName(batPath),
            WorkingDirectory = workDir,
            UseShellExecute = true,
            CreateNoWindow = true
        };
        try
        {
            using var p = Process.Start(psi);
            if (p != null)
                p.WaitForExit();
        }
        catch
        {
            UnityEngine.Debug.LogError($"运行打表脚本失败: {batPath}");
        }
    }

    private static void LoadOrUnloadLauncher()
    {
        if (EditorApplication.isPlaying)
            return;

        var guids = AssetDatabase.FindAssets($"t:scene {k_GameLauncherScene}", new[] { "Assets/Scenes" });
        if (guids == null || guids.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到GameLauncher场景", "确定");
            return;
        }

        var launcherPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        if (string.IsNullOrEmpty(launcherPath))
            return;

        if (IsSceneOpen(launcherPath))
        {
            var scene = EditorSceneManager.GetSceneByPath(launcherPath);
            if (scene.IsValid())
                EditorSceneManager.CloseScene(scene, true);
        }
        else
        {
            var opened = EditorSceneManager.OpenScene(launcherPath, OpenSceneMode.Additive);
            if (opened.IsValid())
            {
                ResortSceneToFront(opened);
                DeactiveMainCameraInLauncher(opened);
            }
        }
    }


    private static void DeactiveMainCameraInLauncher(Scene launcherScene)
    {
        var mainCamera = launcherScene.GetRootGameObjects().FirstOrDefault(g => g.name == "Main Camera");
        if (mainCamera != null)
            mainCamera.SetActive(false);
    }

    private static bool IsSceneOpen(string scenePath)
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var s = EditorSceneManager.GetSceneAt(i);
            if (s.path == scenePath)
                return true;
        }
        return false;
    }

    private static void ResortSceneToFront(Scene mainScene)
    {
        var count = EditorSceneManager.sceneCount;
        var list = new System.Collections.Generic.List<Scene>(count);
        for (int i = 0; i < count; i++)
            list.Add(EditorSceneManager.GetSceneAt(i));
        list.Remove(mainScene);
        list.Insert(0, mainScene);
        for (int i = 1; i < list.Count; i++)
            EditorSceneManager.MoveSceneAfter(list[i], list[i - 1]);
    }
}


