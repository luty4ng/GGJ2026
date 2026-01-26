using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class GameApp
    {
        private static GameApp _instance;
        public static GameApp Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new GameApp();
                    Log.Assert(_instance != null);
                }

                return _instance;
            }
        }

        private static List<Assembly> s_HotfixAssembly;

        /// <summary>
        /// 热更域App主入口。
        /// </summary>
        /// <param name="objects"></param>
        public static void Entrance(object[] objects)
        {
            s_HotfixAssembly = (List<Assembly>)objects[0];
            Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
            Log.Warning("======= Entrance GameApp =======");
            Log.Warning("======= Hotfix Assembly Count: {0} =======", s_HotfixAssembly.Count);
            TypesManager.Instance.Init(s_HotfixAssembly.ToArray());
            EventInterfaceHelper.Init();
            // Instance.StartGameLogic();
        }

        /// <summary>
        /// 开始游戏业务层逻辑。
        /// <remarks>显示UI、加载场景等。</remarks>
        /// </summary>
//         private void StartGameLogic()
//         {
// #if UNITY_EDITOR
//             if(IgnoreStartLogic)
//                 return;

//             if (!string.IsNullOrEmpty(RedirectScene))
//             {
//                 GameModule.Scene.LoadSceneAsync(RedirectScene).AsUniTask();
//                 return;
//             }
// #endif
//             GameModule.UI.ShowUIAsync<UIStartGameWindow>();
//         }

// #if UNITY_EDITOR
//         public static string RedirectScene  
//         {
//             get => UnityEditor.EditorPrefs.GetString("Cosmos_GameApp_RedirectScene", string.Empty);
//             set => UnityEditor.EditorPrefs.SetString("Cosmos_GameApp_RedirectScene", value);
//         }
//         public static bool IgnoreStartLogic
//         {
//             get => UnityEditor.EditorPrefs.GetInt("Cosmos_GameApp_IgnoreStartLogic", 0) == 1;
//             set => UnityEditor.EditorPrefs.SetInt("Cosmos_GameApp_IgnoreStartLogic", value ? 1 : 0);
//         }
// #endif
    }
}