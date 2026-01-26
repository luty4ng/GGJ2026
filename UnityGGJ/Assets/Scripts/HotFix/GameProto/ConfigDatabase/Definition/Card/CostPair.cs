using System;
using Sirenix.OdinInspector;

namespace GameConfig
{
    [Serializable]
    public struct CostPair
    {
        [LabelText("消耗资源")]
        public string costResource;
        [LabelText("消耗值")]
        public int costValue;
    }
}