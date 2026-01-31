using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UGFExtensions.Await;

namespace GameLogic
{
    [Window(UILayer.UI, resetTransformOnLoad: false)]
    public class UISimpleStartGameWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_tmpMainTitle;
        private Button m_btnStartGame;
        private TextMeshProUGUI m_tmpStart;
        private Button m_btnOption;
        private TextMeshProUGUI m_tmpOption;
        private Button m_btnCredits;
        private TextMeshProUGUI m_tmpCredits;
        private Button m_btnQuit;
        private TextMeshProUGUI m_tmpQuit;
        protected override void ScriptGenerator()
        {
            m_tmpMainTitle = FindChildComponent<TextMeshProUGUI>("CT_TopBar/m_tmpMainTitle");
            m_btnStartGame = FindChildComponent<Button>("CT_MainPanel/m_btnStartGame");
            m_tmpStart = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnStartGame/m_tmpStart");
            m_btnOption = FindChildComponent<Button>("CT_MainPanel/m_btnOption");
            m_tmpOption = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnOption/m_tmpOption");
            m_btnCredits = FindChildComponent<Button>("CT_MainPanel/m_btnCredits");
            m_tmpCredits = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnCredits/m_tmpCredits");
            m_btnQuit = FindChildComponent<Button>("CT_MainPanel/m_btnQuit");
            m_tmpQuit = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnQuit/m_tmpQuit");
            m_btnStartGame.onClick.AddListener(UniTask.UnityAction(OnClickStartGameBtn));
            m_btnOption.onClick.AddListener(UniTask.UnityAction(OnClickOptionBtn));
            m_btnCredits.onClick.AddListener(UniTask.UnityAction(OnClickCreditsBtn));
            m_btnQuit.onClick.AddListener(UniTask.UnityAction(OnClickQuitBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickStartGameBtn()
        {
            await StartGameRunDirectlyAsync();
            GameModule.UI.CloseUI<UISimpleStartGameWindow>();
        }
        private async UniTaskVoid OnClickOptionBtn()
        {
            await UniTask.Yield();
            Debug.Log("没做呢还");
        }
        private async UniTaskVoid OnClickCreditsBtn()
        {
            await UniTask.Yield();
            Debug.Log("没做呢还");
        }
        private async UniTaskVoid OnClickQuitBtn()
        {
            await UniTask.Yield();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        async UniTask StartGameRunDirectlyAsync()
        {
            await GameModule.Scene.LoadSceneAsync("GameTest");
        }
        #endregion

    }
}
