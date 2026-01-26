using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UGFExtensions.Await;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class ProcedureStartGame : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        private bool m_InitGameplayComplete = false;
        private string m_StartScene = "GameMain";
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            UILoadMgr.HideAll();
            AwaitableExtensions.SubscribeEvent();
            InitGameplay().Forget();
        }

        public async UniTask InitGameplay()
        {
            await GameplayModule.InitAsync();
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(RedirectScene))
            {
                m_StartScene = RedirectScene;
                await StartGameRunDirectlyAsync();
                return;
            }

            async UniTask StartGameRunDirectlyAsync()
            {
                await GameModule.Scene.LoadSceneAsync(m_StartScene);
                // await GameFlow.Instance.StartGameRunNew();
            }
#endif 
            // GameModule.UI.ShowUIAsync<UIStartGameWindow>(GameFlow.Instance);
            m_InitGameplayComplete = true;
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            if (!m_InitGameplayComplete)
                return;
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            Log.Info($"======= Init GameplayModule Complete =======");
        }

#if UNITY_EDITOR
        public static string RedirectScene
        {
            get => UnityEditor.EditorPrefs.GetString("Cosmos_GameApp_RedirectScene", string.Empty);
            set => UnityEditor.EditorPrefs.SetString("Cosmos_GameApp_RedirectScene", value);
        }

        public static bool IgnoreStartLogic
        {
            get => UnityEditor.EditorPrefs.GetInt("Cosmos_GameApp_IgnoreStartLogic", 0) == 1;
            set => UnityEditor.EditorPrefs.SetInt("Cosmos_GameApp_IgnoreStartLogic", value ? 1 : 0);
        }
#endif
    }
}