using System;
using System.Collections.Generic;
using UGFExtensions.Texture;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 游戏模块
/// </summary>
public class GameModule : MonoBehaviour
{
    #region BaseComponents
    /// <summary>
    /// 获取游戏基础组件。
    /// </summary>
    private static BaseComponent _base;
    public static BaseComponent Base => _base ??= Get<BaseComponent>();

    /// <summary>
    /// 获取调试组件。
    /// </summary>
    private static DebuggerComponent _debugger;
    public static DebuggerComponent Debugger => _debugger ??= Get<DebuggerComponent>();

    /// <summary>
    /// 获取下载组件。
    /// </summary>
    private static DownloadComponent _download;
    public static DownloadComponent Download => _download ??= Get<DownloadComponent>();

    /// <summary>
    /// 获取实体组件。
    /// </summary>
    private static EntityComponent _entity;
    public static EntityComponent Entity => _entity ??= Get<EntityComponent>();

    /// <summary>
    /// 获取事件组件。
    /// </summary>
    private static EventComponent _event;
    public static EventComponent Event => _event ??= Get<EventComponent>();

    /// <summary>
    /// 获取文件系统组件。
    /// </summary>
    private static FileSystemComponent _fileSystem;
    public static FileSystemComponent FileSystem => _fileSystem ??= Get<FileSystemComponent>();

    /// <summary>
    /// 获取有限状态机组件。
    /// </summary>
    private static FsmComponent _fsm;
    public static FsmComponent Fsm => _fsm ??= Get<FsmComponent>();

    /// <summary>
    /// 获取本地化组件。
    /// </summary>
    private static LocalizationComponent _localization;
    public static LocalizationComponent Localization => _localization ??= Get<LocalizationComponent>();

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    private static NetworkComponent _network;
    public static NetworkComponent Network => _network ??= Get<NetworkComponent>();

    /// <summary>
    /// 获取对象池组件。
    /// </summary>
    private static ObjectPoolComponent _objectPool;
    public static ObjectPoolComponent ObjectPool => _objectPool ??= Get<ObjectPoolComponent>();

    /// <summary>
    /// 获取流程组件。
    /// </summary>
    private static ProcedureComponent _procedure;
    public static ProcedureComponent Procedure => _procedure ??= Get<ProcedureComponent>();

    /// <summary>
    /// 获取资源组件。
    /// </summary>
    private static ResourceComponent _resource;
    public static ResourceComponent Resource => _resource ??= Get<ResourceComponent>();

    /// <summary>
    /// 获取场景组件。
    /// </summary>
    private static SceneComponent _scene;
    public static SceneComponent Scene => _scene ??= Get<SceneComponent>();

    /// <summary>
    /// 获取配置组件。
    /// </summary>
    private static SettingComponent _setting;
    public static SettingComponent Setting => _setting ??= Get<SettingComponent>();

    /// <summary>
    /// 获取声音组件。
    /// </summary>
    private static SoundComponent _sound;
    public static SoundComponent Sound => _sound ??= Get<SoundComponent>();

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    private static WebRequestComponent _webRequest;
    public static WebRequestComponent WebRequest => _webRequest ??= Get<WebRequestComponent>();
    
    /// <summary>
    /// 获取时间组件。
    /// </summary>
    private static TimerComponent _timer;
    public static TimerComponent Timer => _timer ??= Get<TimerComponent>();
    
    /// <summary>
    /// 获取设置Texture组件。
    /// </summary>
    private static TextureSetComponent _textureSet;
    public static TextureSetComponent TextureSet => _textureSet ??= Get<TextureSetComponent>();
    
    /// <summary>
    /// 资源组件拓展。
    /// </summary>
    private static ResourceExtComponent _resourceExt;
    public static ResourceExtComponent ResourceExt => _resourceExt ??= Get<ResourceExtComponent>();
    
    /// <summary>
    /// UI组件
    /// </summary>
    private static UISystem _ui;
    public static UISystem UI => _ui ??= Get<UISystem>();

    #endregion

    /// <summary>
    /// 初始化系统框架模块
    /// </summary>
    private static readonly Dictionary<Type, GameFrameworkComponent> s_Components = new Dictionary<Type, GameFrameworkComponent>();
    public static T Get<T>() where T : GameFrameworkComponent
    {
        Type type = typeof(T);
        
        if (s_Components.TryGetValue(type, out GameFrameworkComponent component))
        {
            return (T)component;
        }
        
        component = UnityGameFramework.Runtime.GameSystem.GetComponent<T>();
        
        Log.Assert(condition:component != null,$"{typeof(T)} is null");
        
        s_Components.Add(type,component);

        return (T)component;
    }
    
    public static void QuitApplication()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");

#endif
        Application.Quit();
    }
}