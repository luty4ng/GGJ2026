using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace UnityGameFramework.Editor
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        private static readonly string SceneMain = "GameLauncher";

        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            EditorApplication.playModeStateChanged -= OnQuitPlaying;
            EditorApplication.playModeStateChanged += OnQuitPlaying;
        }

        static readonly string ButtonStyleName = "Tab middle";
        static GUIStyle _buttonGuiStyle;
        private static bool _canUnloadFirstScene = false;
        static void OnToolbarGUI()
        {
            _buttonGuiStyle ??= new GUIStyle(ButtonStyleName)
            {
                padding = new RectOffset(2, 8, 2, 2),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(
                    new GUIContent("Launch", EditorGUIUtility.FindTexture("PlayButton"), $"Start Scene Launcher"),
                    _buttonGuiStyle))
            {
                SceneHelper.StartScene(SceneMain);
            }

            // GUILayout.Space(10);
            // if (GUILayout.Button(
            //         new GUIContent("Run", EditorGUIUtility.FindTexture("PlayButton"), $"Start Current With Scene Launcher"),
            //         _buttonGuiStyle))
            // {
            //     SceneHelper.StartSceneWithMain(SceneMain);
            // }
        }

        static void OnQuitPlaying(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode && _canUnloadFirstScene)
            {
                var scene = EditorSceneManager.GetSceneAt(0);
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }

    public static class SceneHelper
    {
        static string _openSceneName;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;

            _openSceneName = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (_openSceneName == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string[] guids = AssetDatabase.FindAssets("t:scene " + _openSceneName, null);
                if (guids.Length > 0)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    EditorApplication.EnterPlaymode();
                }
            }

            _openSceneName = null;
        }


        public static void StartSceneWithMain(string mainSceneName)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
            
            _openSceneName = mainSceneName;
            _resortSceneOrder = true;
            EditorApplication.update += OnUpdateAdditive;
        }

        private static bool _resortSceneOrder = false;
        static void OnUpdateAdditive()
        {
            if (_openSceneName == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            
            EditorApplication.update -= OnUpdateAdditive;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string[] guids = AssetDatabase.FindAssets("t:scene " + _openSceneName, null);
                if (guids.Length > 0)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    if (_resortSceneOrder)
                        ResortScene(scene);
                    EditorApplication.EnterPlaymode();
                }
            }
            
            _openSceneName = null;
            _resortSceneOrder = false;
        }
        
        static void ResortScene(Scene mainScene)
        {
            var allScenes = new List<Scene>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                allScenes.Add(EditorSceneManager.GetSceneAt(i));
            allScenes.Remove(mainScene);
            allScenes.Insert(0, mainScene);

            for (int i = 0; i < allScenes.Count; i++)
            {
                if (i == 0)
                    continue;
                EditorSceneManager.MoveSceneAfter(allScenes[i], allScenes[i - 1]);
            }
        }
    }
}