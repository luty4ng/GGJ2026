using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UGFExtensions.Await;

namespace GameLogic
{
    [Window(UILayer.UI, resetTransformOnLoad: false)]
    public class UISimpleResultWindow : UIWindow
    {
        #region 脚本工具生成的代码

        private TextMeshProUGUI m_tmpTitle;
        private TextMeshProUGUI m_tmpHint;

        protected override void ScriptGenerator()
        {
            m_tmpTitle = FindChildComponent<TextMeshProUGUI>("m_tmpTitle");
            m_tmpHint = FindChildComponent<TextMeshProUGUI>("m_tmpHint");
        }

        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            m_tmpTitle.text = "恭喜你又活了一天";
            m_tmpTitle.color = Color.green;
            m_tmpHint.text = "按空格键重开";
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReloadScene().Forget();
            }
        }

        private async UniTask ReloadScene()
        {
            GameModule.UI.CloseUI<UIFevelLineWindow>();
            await GameModule.Scene.UnloadSceneAsync("GameTest");
            await GameModule.Scene.LoadSceneAsync("GameTest");
            GameModule.UI.CloseUI<UISimpleResultWindow>();
        }
    }
}