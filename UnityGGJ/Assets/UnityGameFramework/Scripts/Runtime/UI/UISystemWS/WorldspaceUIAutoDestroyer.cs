using System;
using dnlib.DotNet;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public class WorldspaceUIAutoDestroyer : MonoBehaviour
    {
        void OnDestroy()
        {
            GameModule.WorldspaceUI.CloseAllUIOnTarget(transform);
        }
    }
}