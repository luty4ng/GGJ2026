using System.Linq;
using UnityEditor;

namespace Client.Editor
{
    public static class SceneNavigateSetting
    {
        public const string AssetsRoot = "Assets/";
        public const string GameMainRoot = "Assets/GameMain/";
        public static string ScenesListPath
        {
            get
            {
                string[] files = AssetDatabase.FindAssets("ScenesList");
                string assetPath = AssetDatabase.GUIDToAssetPath(files.First());
                return assetPath;
            }
        }
        
        public static string LauncherPath
        {
            get
            {
                string[] files = AssetDatabase.FindAssets("GameLauncher");
                string assetPath = AssetDatabase.GUIDToAssetPath(files.First());
                return assetPath;
            }
        }
    }
}
