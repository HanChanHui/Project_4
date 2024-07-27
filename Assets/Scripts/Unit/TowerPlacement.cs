using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    private List<GridPosition> towerGridPositionList = new List<GridPosition>();
    private GridPosition previousGridPosition = new GridPosition();
    private Vector2 resultTowerGridPos = new Vector2();

    private bool isDisposition = false;
    private Transform towerGhostPrefab;
    private TowerObject towerObject;

    public void OnMouseUp()
    {
        if (ResourceManager.Instance.SelectedPrefabIndex != -1)
        {
            GridSystemVisual.Instance.ShowBeforeTowerGridVisual();
            RefreshVisual();
            if (isDisposition)
            {
                PlaceTower(resultTowerGridPos);
            }
        }
    }

    public void HandleMouseHoldAction()
    {
        if (GetPosition(out Vector2 position))
        {
            GridPosition gridPosition = levelGrid.GetGridPosition(position);
            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);

            if (!gridPosition.Equals(previousGridPosition))
            {
                previousGridPosition = gridPosition;
                towerGridPositionList.Clear();

                //GridSystemVisual.Instance.ShowTowerGridPositionRange(gridPosition, towerObject.width, towerObject.height);


                List<Vector2> gridPositionList = towerObject.GetGridPositionList(gridTr);
                foreach (Vector2 gridPos in gridPositionList)
                {
                    GridPosition towerPos = levelGrid.GetGridPosition(gridPos);
                    towerGridPositionList.Add(towerPos);
                    if (HasAnyGridObject(towerPos))
                    {
                        isDisposition = false;
                        return;
                    }
                }
                resultTowerGridPos = gridTr;
                isDisposition = true;
            }
        }
        else
        {
            if(isDisposition)
            {
                GridSystemVisual.Instance.ShowBeforeTowerGridVisual();
                previousGridPosition = new GridPosition();
            }
            isDisposition = false;
        }
    }

    private void PlaceTower(Vector2 gridTr)
    {
        Tower tower = Instantiate(towerObject.prefab, gridTr, Quaternion.identity).GetComponentInChildren<Tower>();
        foreach (GridPosition gridPos in towerGridPositionList)
        {
            tower.GridPosition.Add(gridPos);
            levelGrid.AddUnitAtGridPosition(gridPos, tower);
        }
        ResourceManager.Instance.SetSelectedPrefabIndex(-1);
    }

    public void UpdateMousePosition()
    {
        Vector3 targetPosition = GetMouseWorldSnappedPosition();
        towerGhostPrefab.position = Vector3.Lerp(towerGhostPrefab.position, targetPosition, Time.deltaTime * 50f);
    }

    public void RefreshVisual()
    {
        if (towerGhostPrefab != null)
        {
            Destroy(towerGhostPrefab.gameObject);
            towerGhostPrefab = null;
            return;
        }

        int count = ResourceManager.Instance.SelectedPrefabIndex;
        if (count >= 0 && count < ResourceManager.Instance.Prefabs.Count)
        {
            towerObject = ResourceManager.Instance.Prefabs[count];
        }
        else
        {
            towerObject = null;
        }

        if (towerObject != null)
        {
            towerGhostPrefab = Instantiate(towerObject.prefab, GetMousePosition(), Quaternion.identity).transform;
        }
    }

    private Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = GetMousePosition();
        GridPosition gridPosition = levelGrid.GetGridPosition(mousePosition);

        if (gridPosition != null) {
            // **1층 그리드 위치**
            Vector3 worldPosition1 = levelGrid.GetWorldPosition(gridPosition);

            //// **2층 그리드 위치**
            //var gridSystem2 = levelGrid.GetGridSystem2();
            //if (gridSystem2 != null) {
            //    foreach (var gridPosition2 in levelGrid.GetLayer2GridPosition()) {
            //        Vector3 worldPosition2 = gridSystem2.GetWorldPosition(gridPosition2);

            //        // **2층 그리드가 유효한 경우**
            //        if (gridSystem2.IsValidGridPosition(gridPosition2) && !levelGrid.HasAnyUnitOnGridPosition(gridPosition2)) {
            //            if (Vector2.Distance(mousePosition, worldPosition2) < gridSystem2.GetCellSize() / 2) {
            //                return worldPosition2;
            //            }
            //        }
            //    }
            //}

            return worldPosition1;
        }

        return mousePosition;
    }

    private bool GetPosition(out Vector2 position)
    {
        Vector3 clickPosition = GetMousePosition();
        RaycastHit2D raycastHit = Physics2D.Raycast(clickPosition, Vector2.zero, 0f, mousePlaneLayerMask);
        if (raycastHit.collider != null)
        {
            position = raycastHit.point;
            return true;
        }

        position = Vector2.zero;
        return false;
    }

    private Vector3 GetMousePosition()
    {
        return InputManager.Instance.GetMouseWorldPosition();
    }

    private bool HasAnyGridObject(GridPosition gridPosition)
    {
        return levelGrid.HasAnyUnitOnGridPosition(gridPosition);
    }
}
