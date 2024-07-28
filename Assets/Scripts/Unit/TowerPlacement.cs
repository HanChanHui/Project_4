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

    private Vector3 GetMouseWorldSnappedPosition() 
    {
        Vector3 mousePosition = GetMousePosition();
        GridPosition gridPosition = levelGrid.GetGridPosition(mousePosition);

        if (gridPosition != null) {
            return levelGrid.GetAdjustedWorldPosition(gridPosition); // 수정된 부분: 높이 조정을 반영한 위치 반환
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
