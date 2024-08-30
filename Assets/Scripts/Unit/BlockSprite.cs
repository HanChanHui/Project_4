using Pathfinding;
using UnityEngine;

namespace HornSpirit {
    public class BlockSprite : MonoBehaviour 
    {
        private DynamicGridObstacle dynamicGridObstacle;

        public void Init() {
            dynamicGridObstacle = gameObject.AddComponent<DynamicGridObstacle>();
            dynamicGridObstacle.updateError = 1f;  // 위치 변경 감도 설정
            dynamicGridObstacle.checkTime = 0.01f;  // 경로 업데이트 간격 설정
        }
    }
}
