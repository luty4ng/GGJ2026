using UnityEngine;
namespace GameLogic
{
    public static class GameplayEventId
    {
        public const string OnPlayerInteractStateChange = "GameplayEventId.OnPlayerInteractStateChange";
        public const string OnPlayerStatusChange = "GameplayEventId.OnPlayerStatusChange";
        public const string OnGameSelectionChange = "GameplayEventId.OnGameSelectionChange";
        public const string OnGameIncidentGenerate = "GameplayEventId.OnGameIncidentGenerate";
        public const string OnPlanetKilled = "GameplayEventId.OnCheckIfBattleEnded";
        public const string OnGameOver = "GameplayEventId.OnGameOver";

        public static class UI
        {
            public const string OnInventoryItemDragBegin = "GameplayEventId.UI.OnInventoryItemDragBegin";
            public const string OnInventoryItemDrag = "GameplayEventId.UI.OnInventoryItemDrag";
            public const string OnInventoryItemDragRelease = "GameplayEventId.UI.OnInventoryItemDragRelease";
        }
    }
}
