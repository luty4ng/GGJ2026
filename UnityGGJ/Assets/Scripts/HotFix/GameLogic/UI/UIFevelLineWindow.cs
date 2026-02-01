using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UGFExtensions.Await;

namespace GameLogic
{
    [Window(UILayer.UI, resetTransformOnLoad: false)]
    public class UIFevelLineWindow : UIWindow
    {
        #region 脚本工具生成的代码

        private Image m_imgSlide;

        protected override void ScriptGenerator()
        {
            m_imgSlide = FindChildComponent<Image>("m_imgSlide");
        }

        #endregion

        private GameFlow m_gameFlow;

        protected override void OnCreate()
        {
            base.OnCreate();
            if (UserData is GameFlow gameFlow)
            {
                m_gameFlow = gameFlow;
                m_imgSlide.fillAmount = (float)m_gameFlow.CurrentScore / (float)m_gameFlow.WinScore;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (m_gameFlow != null)
            {
                m_imgSlide.fillAmount = (float)m_gameFlow.CurrentScore / (float)m_gameFlow.WinScore;
            }
        }
    }
}