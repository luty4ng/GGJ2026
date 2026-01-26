using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityWeld.Binding.Internal;
using UnityWeld.Binding;
using static UnityWeld.Binding.ControlBinding;

namespace UnityWeld_Editor
{
    [CustomEditor(typeof(ViewBinding))]
    public class ViewBindingEditor : ControlBindingEditor
    {
        private ViewBinding m_targetScript;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_targetScript = (ViewBinding)target;
        }
    }
}
