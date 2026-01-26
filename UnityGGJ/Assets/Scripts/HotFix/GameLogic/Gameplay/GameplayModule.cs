using Cysharp.Threading.Tasks;
using UnityEngine;
using GameConfig;

namespace GameLogic
{
    public class GameplayModule : MonoBehaviour
    {
        public static GameTickerManager GameTicker { get; private set; }
        private static GameObject s_GameplayRoot;
        private static bool _isInitialized = false;


        public static async UniTask InitAsync()
        {
            if (_isInitialized)
                return;

            if (s_GameplayRoot == null)
            {
                s_GameplayRoot = new GameObject("GameplayModule");
                s_GameplayRoot.GetOrAddComponent<GameplayModule>();
                DontDestroyOnLoad(s_GameplayRoot);
            }

            GameTicker = new GameTickerManager();
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;
            GameTicker.Tick(Time.deltaTime, Time.unscaledDeltaTime);
        }

        void OnDestroy()
        {
            
        }
    }
}