using System.IO;
using System.Text;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Editor
{
    public static class SceneMenu
    {
        #region BuildSceneList

        [MenuItem("Tools/Scene Navigate/ScenesBuild-All")]
        static void BuildScenesAll()
        {
            BuildScenes(SceneNavigateSetting.AssetsRoot);
        }

        [MenuItem("Tools/Scene Navigate/ScenesBuild-Main")]
        static void BuildScenesMain()
        {
            BuildScenes(SceneNavigateSetting.GameMainRoot);
        }

        private static void BuildScenes(string root)
        {
            string[] files = AssetDatabase.FindAssets("t:Scene", new [] { root });
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[files.Length];
            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = AssetDatabase.GUIDToAssetPath(files[i]);
                scenes[i] = new EditorBuildSettingsScene(files[i], true);
                if (i > 0 && Path.GetFileNameWithoutExtension(files[i]) == "GameLauncher")
                {
                    var temp = scenes[0];
                    scenes[0] = scenes[i];
                    scenes[i] = temp;
                }
            }
            EditorBuildSettings.scenes = scenes;
        }

        #endregion
        
        
        #region UpdateSceneList

        [MenuItem("Tools/Scene Navigate/ScenesUpdate-All")]
        public static void UpdateScenesAll()
        {
            UpdateFastAccessScenes(SceneNavigateSetting.AssetsRoot);
        }
        
        [MenuItem("Tools/Scene Navigate/ScenesUpdate-Main")]
        public static void UpdateScenesMain()
        {
            UpdateFastAccessScenes(SceneNavigateSetting.GameMainRoot);
        }

        private static void UpdateFastAccessScenes(string root)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using UnityEditor;");
            stringBuilder.AppendLine("namespace Client.Editor\n{");
            stringBuilder.AppendLine("\tpublic static class ScenesList");
            stringBuilder.AppendLine("\t{");

            foreach (string sceneGuid in AssetDatabase.FindAssets("t:Scene", new [] { root }))
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                string methodName = scenePath.Replace('/', '_').Replace('\\', '_').Replace('.', '_').Replace('-', '_').Replace(' ', '_');
                stringBuilder.AppendLine(string.Format("\t\t[MenuItem(\"Scenes/{0}\")]", sceneName));
                stringBuilder.AppendLine(string.Format("\t\tpublic static void {0}() {{ SceneMenu.OpenScene(\"{1}\"); }}", methodName, scenePath));
            }
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");

            CreateOrWriteSceneList(SceneNavigateSetting.ScenesListPath, stringBuilder.ToString());
            AssetDatabase.Refresh();
        }

        private static void CreateOrWriteSceneList(string assetPath, string content)
        {
            string[] pathParts = assetPath.Split("/");
            string directoryPath = pathParts.Take(pathParts.Length - 1).Aggregate((a, b) => a + "/" + b);
            if(!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!File.Exists(assetPath))
            {
                using (var stream = File.Create(assetPath))
                {
                    File.WriteAllText(assetPath, content);
                    stream.Flush();
                }
            }
            else
            {
                File.WriteAllText(assetPath, content);
            }
        }
        #endregion
        
        public static void OpenScene(string filename)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Open Scene: " + filename);
                if (SceneManager.sceneCount > 0)
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                    }
                }

                EditorSceneManager.OpenScene(SceneNavigateSetting.LauncherPath, OpenSceneMode.Single);
                EditorSceneManager.OpenScene(filename, OpenSceneMode.Additive);
            }
        }
    }
}