using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    [CreateAssetMenu(fileName = "NewCard", menuName = "Unity Royale/Card Data")]
    public class CardData : ScriptableObject {
        [Header("Card graphics")]
        public Sprite cardImage;
        public Sprite cardPropertyImage;
        public Sprite cardBackgroundImage;

        [Header("List of Placeables")]
        public PlaceableTowerData towerData;  // 타워 데이터를 위한 필드
        public PlaceableBlockData blockData;  // 블록 데이터를 위한 필드
        public Vector3 relativeOffsets;

        public IPlaceable placeableData {
            get {
                if (towerData != null) {
                    return towerData;
                } else if (blockData != null) {
                    return blockData;
                } else {
                    return null;
                }
            }
        }
    }
}
