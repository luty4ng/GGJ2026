using System;
using YooAsset;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;

namespace UnityGameFramework.Runtime
{
    public class FallbackResourceManager : Singleton<FallbackResourceManager>
    {
        protected override void Init()
        {
            base.Init();
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new ResourceLogger());
            }
            YooAssets.SetOperationSystemMaxTimeSlice(30);
            InitEditorDefaultPackage();
        }

        private void InitEditorDefaultPackage()
        {
            string packageName = "DefaultPackage";
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }
            
            InitializationOperation initializationOperation = null;
            var createParameters = new EditorSimulateModeParameters();
            createParameters.CacheBootVerifyLevel = EVerifyLevel.Middle;
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, packageName);
            initializationOperation = package.InitializeAsync(createParameters);
            Log.Info($"Init resource package version : {initializationOperation?.PackageVersion}");
        }

        public async void LoadAssetAsync<T>(string location, Type assetType, Action<T> onComplete = null, Action onFailed = null) where T : UnityEngine.Object
        {
            AssetHandle handle = YooAssets.LoadAssetAsync(location, assetType);
            await handle.ToUniTask();

            if (handle.AssetObject == null || handle.Status == EOperationStatus.Failed)
            {
                onFailed?.Invoke();
                Log.Error("Can not load asset '{0}'.", location);
            }
            else
            {
                onComplete?.Invoke(handle.AssetObject as T);
            }
        }
        
        public async void LoadAssetAsync(string location, Type assetType, Action<UnityEngine.Object> onComplete = null, Action onFailed = null)
        {
            LoadAssetAsync<UnityEngine.Object>(location, assetType, onComplete, onFailed);
        }
        
        public T LoadAsset<T>(string location) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Asset name is invalid.");
                return null;
            }
            AssetHandle handle = YooAssets.LoadAssetAsync<T>(location);
            T ret = handle.AssetObject as T;
            return ret;
        }

        public UnityEngine.Object LoadAsset(string location)
        {
            return LoadAsset<UnityEngine.Object>(location);
        }
        
    }
}