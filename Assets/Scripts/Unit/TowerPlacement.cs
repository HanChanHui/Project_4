using Consts;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private InputManager inputManager;
    private List<GridPosition> towerGridPositionList = new List<GridPosition>();
    private GridPosition previousGridPosition = new GridPosition();
    private Vector3 resultTowerGridPos = new Vector3();

    private bool isDisposition = false;
    private Transform towerGhostPrefab;
    private TowerObject towerObject;
    private TowerType towerType;

    private bool isOutside = false;

    private void Awake() 
    {
        if (levelGrid == null) 
        {
            levelGrid = GameObject.Find("LevelGrid").GetComponent<LevelGrid>();

            if (levelGrid == null) {
                Debug.LogError("LevelGrid 오브젝트를 찾을 수 없습니다. LevelGrid를 할당해주세요.");
            }
        }
    }

    public void OnMouseUp()
    {
        if (ResourceManager.Instance.SelectedPrefabIndex != -1)
        {
            RefreshVisual();
            ResourceManager.Instance.SetSelectedPrefabIndex(-1);
            if (isDisposition)
            {
                PlaceTower(resultTowerGridPos);
            }
        }
    }

    public void HandleMouseHoldAction()
    {

        if (inputManager.GetPosition(out Vector2 position))
        {
            isOutside = true;
            Vector3 pos = position;
            if(levelGrid.HasAnyBlockTypeOnWorldPosition(pos))
            {
                pos.z = 2f;
            }

            GridPosition gridPosition = levelGrid.GetGridPosition(pos);
            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);
            if (!gridPosition.Equals(previousGridPosition))
            {
                previousGridPosition = gridPosition;
                towerGridPositionList.Clear();

                OnTowerTypeVisualGrid(gridPosition);

                if (TryGetTowerGridPositions(gridPosition, out List<GridPosition> gridPositions))
                {
                    towerGridPositionList = gridPositions;
                    resultTowerGridPos = gridTr;
                    isDisposition = true;
                }
                else
                {
                    previousGridPosition = new GridPosition();
                    isDisposition = false;
                    return;
                }
            }
        }
        else
        {
            isDisposition = false;
            if(isOutside)
            {
                isOutside = false;
                towerGhostPrefab.GetComponent<TowerVisualGrid>().OutSideGridPositionList();
                previousGridPosition = new GridPosition();
            }
        }
        
    }

    private bool TryGetTowerGridPositions(GridPosition gridPosition, out List<GridPosition> gridPositions)
    {
        gridPositions = new List<GridPosition>();

        if (towerType == TowerType.Dealer)
        {
            if (towerObject.GetSingleGridPosition(gridPosition))
            {
                gridPositions.Add(gridPosition);
                return true;
            }
        }
        else if (towerType == TowerType.Tanker)
        {
            if (towerObject.GetGridPositionList(gridPosition, out gridPositions))
            {
                return true;
            }
        }

        return false;
    }

    private void PlaceTower(Vector3 gridTr)
    {
        Tower tower = Instantiate(towerObject.prefab, gridTr, Quaternion.identity).GetComponentInChildren<Tower>();
        foreach (GridPosition gridPos in towerGridPositionList)
        {
            tower.GridPositionList.Add(gridPos);
            levelGrid.AddTowerAtGridPosition(gridPos, tower);
        }
    }

    public void UpdateMousePosition()
    {
        Vector3 targetPosition = GetMouseWorldSnappedPosition();
        towerGhostPrefab.position = targetPosition;
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
        if (count >= 0)
        {
            towerObject = ResourceManager.Instance.Prefabs[count];
        }
        else
        {
            towerObject = null;
        }

        if (towerObject != null)
        {
            isOutside = false;
            towerGhostPrefab = Instantiate(towerObject.icon, inputManager.GetMouseWorldPosition(), Quaternion.identity).transform;
        }
    }

    private Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = inputManager.GetMouseWorldPosition();
        if (levelGrid.HasAnyBlockTypeOnWorldPosition(mousePosition))
        {
            mousePosition.z = 2f;
            return levelGrid.GetWorldPosition(levelGrid.GetGridPosition(mousePosition));
        }

        GridPosition gridPosition = levelGrid.GetCameraGridPosition(mousePosition);
        if(levelGrid.IsValidGridPosition(gridPosition))
        {
            return levelGrid.GetWorldPosition(gridPosition);
        }

        return mousePosition;
    }

    private void OnTowerTypeVisualGrid(GridPosition gridPosition)
    {
        towerType = towerObject.towerType;
        switch(towerType)
        {
            case TowerType.Dealer:
                towerGhostPrefab.GetComponent<TowerVisualGrid>().ShowSingleTowerGridPositionRange(gridPosition);
                break;
             case TowerType.Tanker:
                towerGhostPrefab.GetComponent<TowerVisualGrid>().ShowRangeTowerGridPositionRange(gridPosition);
                break;
        }
    }
}
