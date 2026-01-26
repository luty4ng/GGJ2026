using System;
using dnlib.DotNet;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public enum WorldSpaceUIFaceMode
    {
        Billboard,
        YPositive,
        SurfaceNormal
    }
    public abstract class WorldspaceUIBase : MonoBehaviour, IViewBase
    {
        public Transform Target { get; private set; }
        public Camera RenderCamera { get; private set; }
        public WorldSpaceUIFaceMode FaceMode { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public ViewBindingBase ViewBinding { get; private set; }
        public object UserData { get; private set; }
        public void Initialize(Transform target, Camera renderCamera, WorldSpaceUIFaceMode faceMode, object userData = null)
        {
            Target = target;
            RenderCamera = renderCamera;
            FaceMode = faceMode;
            RectTransform = GetComponent<RectTransform>();
            UserData = userData;
            if(UserData is ViewBindingInitParam bindingParam)
            {
                ViewBinding = (ViewBindingBase)Activator.CreateInstance(bindingParam.BindingType);
                ViewBinding.BindView(this);
                ViewBinding.BindViewModel(bindingParam.ViewModel);
            }
            OnInitialize();
        }

        protected void LateUpdate()
        {
            if (Target == null || RenderCamera == null)
                return;

            transform.position = Target.position;
            if (FaceMode == WorldSpaceUIFaceMode.Billboard)
            {
                Vector3 toCamera = -RenderCamera.transform.forward;
                if (toCamera.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(-toCamera, RenderCamera.transform.up);
            }
            else if (FaceMode == WorldSpaceUIFaceMode.YPositive)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.up);
            }
            else if (FaceMode == WorldSpaceUIFaceMode.SurfaceNormal)
            {
                // todo: 由子类自己实现，有自己的Context看surfacenormal是多少
            }

            OnLateUpdate();
        }

        protected abstract void OnLateUpdate();
        protected abstract void OnInitialize();
    }
}