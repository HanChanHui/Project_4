using Consts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HornSpirit {
    public class HealerTower : Tower {
        [SerializeField] protected List<Tower> towersInRange = new List<Tower>();
        private Tower targetTower;
        private List<Tower> currentTowersInRange = new List<Tower>();
        protected AttackDirection atkDirection;
        protected JoystickController joystickController;
        protected List<GridPosition> atkRangeGridList = new List<GridPosition>();
        private bool isHealing = false;


        protected override void MyInit() {
            base.MyInit();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
        }

        protected override IEnumerator CoCheckDistance() {
            while (true) 
            {
                if (towersInRange.Count > 0 && !isHealing) {
                    FindTower();
                    CoHealTowers();
                    yield return new WaitForSeconds(attackSpeed);
                }
                else
                {
                    FindTower();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void CoHealTowers() {
            isHealing = true;
            //Debug.Log("힐 탐색");
            if (towersInRange.Count > 0) {
                
                towersInRange.Sort((t1, t2) => 
                ((float)t1.Health / t1.MaxHealth).CompareTo((float)t2.Health / t2.MaxHealth));
                
                targetTower = towersInRange[0];

                if (targetTower != null && targetTower.Health < targetTower.MaxHealth) {
                    targetTower.SetHealth((int)attackDamage);
                }
            }

            isHealing = false;
        }
        protected void FindTower() {
            currentTowersInRange.Clear();
    
            foreach (GridPosition gridPos in atkRangeGridList) {
                Tower tower = LevelGrid.Instance.GetTowerAtGridPosition(gridPos);
                if (tower != null && tower.Health < tower.MaxHealth) 
                {
                    if (!towersInRange.Contains(tower)) {
                        towersInRange.Add(tower);
                    }
                    currentTowersInRange.Add(tower);
                }
            }

            towersInRange.RemoveAll(tower => !currentTowersInRange.Contains(tower));
        }

        protected virtual void OnAttackDirectionSelected(Vector2 direction) {
            direction.Normalize();

            if (direction.x > 0 && direction.y < 0) {
                atkDirection = AttackDirection.Right;
            } else if (direction.x < 0 && direction.y > 0) {
                atkDirection = AttackDirection.Left;
            } else if (direction.x < 0 && direction.y < 0) {
                atkDirection = AttackDirection.Down;
            } else if (direction.x > 0 && direction.y > 0) {
                atkDirection = AttackDirection.Up;
            }
        }

        public void GenerateAttackPattern(AttackDirection direction, GridPosition gridPosition, List<Vector2Int> basePatternArray) {
            atkRangeGridList.Clear();
            towersInRange.Clear();

            List<Vector2Int> directionVectors = patternData.GetDirectionVector(basePatternArray, direction);

            foreach (Vector2Int directionVector in directionVectors) {
                GridPosition attackGridPosition = gridPosition + new GridPosition(directionVector.x, directionVector.y);
                atkRangeGridList.Add(attackGridPosition);
            }

            FilterInvalidGridPositions(atkRangeGridList);
        }

        protected void FilterInvalidGridPositions(List<GridPosition> atkRangeGridList) {
            atkRangeGridList.RemoveAll(gridPos =>
                !LevelGrid.Instance.IsValidGridPosition(gridPos) ||
                gridPos == gridPositionList[0]
            );
        }

    }
}
