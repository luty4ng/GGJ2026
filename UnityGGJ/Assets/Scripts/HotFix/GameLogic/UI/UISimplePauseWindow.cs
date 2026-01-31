using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UGFExtensions.Await;
namespace GameLogic
{
    [Window(UILayer.UI, resetTransformOnLoad: true)]
    public class UISimplePauseWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_tmpPauseTitle;
        private TextMeshProUGUI m_tmpMusicVolumeTitle;
        private Slider m_sliderMusicVolume;
        private TextMeshProUGUI m_tmpSFXVolumeTitle;
        private Slider m_sliderSFXVolume;
        private Button m_btnRestart;
        private TextMeshProUGUI m_tmpRestart;
        private Button m_btnQuit;
        private TextMeshProUGUI m_tmpQuit;
        protected override void ScriptGenerator()
        {
            m_tmpPauseTitle = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_tmpPauseTitle");
            m_tmpMusicVolumeTitle = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_tmpMusicVolumeTitle");
            m_sliderMusicVolume = FindChildComponent<Slider>("CT_MainPanel/m_sliderMusicVolume");
            m_tmpSFXVolumeTitle = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_tmpSFXVolumeTitle");
            m_sliderSFXVolume = FindChildComponent<Slider>("CT_MainPanel/m_sliderSFXVolume");
            m_btnRestart = FindChildComponent<Button>("CT_MainPanel/m_btnRestart");
            m_tmpRestart = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnRestart/m_tmpRestart");
            m_btnQuit = FindChildComponent<Button>("CT_MainPanel/m_btnQuit");
            m_tmpQuit = FindChildComponent<TextMeshProUGUI>("CT_MainPanel/m_btnQuit/m_tmpQuit");
            m_sliderMusicVolume.onValueChanged.AddListener(OnSliderMusicVolumeChange);
            m_sliderSFXVolume.onValueChanged.AddListener(OnSliderSFXVolumeChange);
            m_btnRestart.onClick.AddListener(UniTask.UnityAction(OnClickRestartBtn));
            m_btnQuit.onClick.AddListener(UniTask.UnityAction(OnClickQuitBtn));
        }
        #endregion


        private AudioSource m_rhythmSource;
        protected override void OnCreate()
        {
            base.OnCreate();
            if (UserData is GameFlow flow)
            {
                m_rhythmSource = flow.RhythmController.audioCom;
                m_sliderMusicVolume.value = m_rhythmSource.volume;
            }
            m_sliderSFXVolume.value = GameModule.Sound.GetVolume("Sound");
        }

        #region 事件
        private void OnSliderMusicVolumeChange(float value)
        {
            if (m_rhythmSource != null)
                m_rhythmSource.volume = value;
        }
        private void OnSliderSFXVolumeChange(float value)
        {
            GameModule.Sound.SetVolume("Sound", value);
        }
        private async UniTaskVoid OnClickRestartBtn()
        {
            await UniTask.Yield();
            await GameModule.Scene.UnloadSceneAsync("GameTest");
            await GameModule.Scene.LoadSceneAsync("GameTest");
            GameModule.UI.CloseUI<UISimplePauseWindow>();
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
        #endregion

    }
}
