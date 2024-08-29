using Consts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    public class Block : LivingEntity {
        protected List<GridPosition> gridPositionList = new List<GridPosition>();
        public List<GridPosition> GridPositionList { get { return gridPositionList; } set { gridPositionList = value; } }
        [SerializeField] private HealthLabel healthBar;
        [SerializeField] private BlockType blockType;
        public BlockType BlockType { get { return blockType; } }

        private void Awake() {
            if (blockType == BlockType.TargetBlock) {
                InitHealth(5);
                GameManager.Instance.AddPlaceableTargetList(transform);
            }
        }

        private void OnEnable() 
        {
            gridPositionList.Add(LevelGrid.Instance.GetGridPosition(transform.position));
            LevelGrid.Instance.AddBlockAtGridPosition(gridPositionList[0], this);
        }

        void Start() {
            if (healthBar != null) {
                healthBar.Init();
            }
        }

        public override void TakeDamage(float damage, int obstacleDamage = 1, bool showLabel = false) {
            base.TakeDamage(damage, obstacleDamage, showLabel);
            healthBar.Show();
            healthBar.UpdateHealth(health, maxHealth);
            int count = GameManager.Instance.TargetDeathCount + 1;
            GameManager.Instance.TargetDeathCount = count;
            if (health <= 0) {
                DestroyTarget();
            }
        }


        private void DestroyTarget() {
            GameManager.Instance.RemovePlaceableTargetList(transform);
            Destroy(gameObject);
        }

    }
}
