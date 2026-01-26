using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UGFExtensions.Await;
namespace GameLogic
{
	[Window(UILayer.UI)]
	public class UITitleBtnWidget : UIWidget
	{
		#region 脚本工具生成的代码
		private Image m_imgBkg;
		private TextMeshProUGUI m_tmpTitle;
		private Image m_imgFrontMask;
		protected override void ScriptGenerator()
		{
			m_imgBkg = FindChildComponent<Image>("m_imgBkg");
			m_tmpTitle = FindChildComponent<TextMeshProUGUI>("m_tmpTitle");
			m_imgFrontMask = FindChildComponent<Image>("m_imgFrontMask");
		}
		#endregion

		#region 事件
		#endregion

	}
}
