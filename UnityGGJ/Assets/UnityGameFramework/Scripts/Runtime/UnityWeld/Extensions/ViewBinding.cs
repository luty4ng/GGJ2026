using UnityEngine;

namespace UnityWeld.Binding
{
    public class ViewBinding : ControlBinding
    {
        private ViewBase m_view;
        public override Component TargetControl
        {
            get 
            {
                if(m_view == null)
                {
                    m_view = GetComponent<ViewBase>();
                }
                return m_view; 
            }
        }
    }
}
