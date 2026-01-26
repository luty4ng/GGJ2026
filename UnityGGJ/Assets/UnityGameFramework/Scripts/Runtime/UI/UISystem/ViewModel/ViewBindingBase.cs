using System;

namespace UnityGameFramework.Runtime
{
    public struct ViewBindingInitParam
    {
        public IViewModelBase ViewModel { get; private set; }
        public Type BindingType { get; private set; }
        public ViewBindingInitParam(Type bindingType, IViewModelBase viewModel)
        {
            ViewModel = viewModel;
            BindingType = bindingType;
        }
    }
    
    public abstract class ViewBindingBase
    {
        public IViewModelBase ViewModel { get; private set; }
        public IViewBase View { get; private set; }
        private bool m_isBindCompleted = false;
        public void BindView(IViewBase view)
        {
            View = view;
            if (!m_isBindCompleted && ViewModel != null && View != null)
            {
                OnBindComplete();
                m_isBindCompleted = true;
            }
        }
        public void BindViewModel(IViewModelBase viewModel)
        {
            ViewModel = viewModel;
            if (!m_isBindCompleted && ViewModel != null && View != null)
            {
                OnBindComplete();
                m_isBindCompleted = true;
            }
        }
        public void UnBind()
        {
            ViewModel = null;
            View = null;
            OnBindRelease();
        }
        protected abstract void OnBindComplete();
        protected abstract void OnBindRelease();
    }
}