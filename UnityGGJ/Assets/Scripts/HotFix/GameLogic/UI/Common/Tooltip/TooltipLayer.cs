using UnityEngine;

namespace GameLogic
{
    public class TooltipLayer : MonoBehaviour
    {
        public RectTransform RectTransform
        {
            get { return (RectTransform)transform; }
        }
    }

}
