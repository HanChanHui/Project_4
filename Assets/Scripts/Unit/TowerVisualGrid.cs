using Consts;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisualGrid : MonoBehaviour
{

    [SerializeField] private TowerObject towerObject;

    private GridPosition gridPosition;
    public GridPosition GridPosition { get{ return gridPosition; } set{ gridPosition = value; }}

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
    private Material material;

    private void Start() 
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            towerObject.width,
            towerObject.height
        ];
        
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        Material material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Red);
        if(towerObject.towerType == TowerType.Dealer)
        {
            Transform gridSystemVisualSingleTransform = Instantiate(ResourceManager.Instance.GridSystemVisualSingPrefab2, transform.position, Quaternion.identity);
            gridSystemVisualSingleTransform.transform.parent = transform;
            gridSystemVisualSingleArray[0, 0] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            gridSystemVisualSingleArray[0, 0].Show(material);
            gridSystemVisualSingleArray[0, 0].GridLayerChange(LayerName.PlaceGrid.ToString());
        }
        else if(towerObject.towerType == TowerType.Tanker)
        {
            for (int x = 0; x < towerObject.width; x++) 
            {
                for (int y = 0; y < towerObject.height; y++) 
                {
                    Vector3 worldPosition = transform.position + new Vector3(x, y);

                    Transform gridSystemVisualSingleTransform = Instantiate(ResourceManager.Instance.GridSystemVisualSingPrefab, worldPosition, Quaternion.identity);
                    gridSystemVisualSingleTransform.transform.parent = transform;
                    gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                    gridSystemVisualSingleArray[x, y].Show(material);
                    gridSystemVisualSingleArray[x, y].GridLayerChange(LayerName.PlaceGrid.ToString());
                }
            }
        }
        
    }

    public void ShowSingleTowerGridPositionRange(GridPosition gridPosition) 
    {
        if (!LevelGrid.Instance.IsValidGridPosition(gridPosition)) 
        {
            GetMaterialGrid(0, 0, GridVisualType.Red);
            return;
        }

        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition)
            || !LevelGrid.Instance.HasAnyBlockOnGridPosition(gridPosition)) 
        {
            GetMaterialGrid(0, 0, GridVisualType.Red);
            return;
        }

        GetMaterialGrid(0, 0, GridVisualType.Green);
    }

    public void ShowRangeTowerGridPositionRange(GridPosition gridPosition) 
    {

        for (int x = 0; x < towerObject.width; x++) 
        {
            for (int y = 0; y < towerObject.height; y++) 
            {
                GridPosition testGridPosition = new GridPosition(x, y) + gridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) 
                {
                    GetMaterialGrid(x, y, GridVisualType.Red);
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition) ||
                    LevelGrid.Instance.HasAnyBlockOnGridPosition(testGridPosition)) 
                {
                    GetMaterialGrid(x, y, GridVisualType.Red);
                    continue;
                }
                
                GetMaterialGrid(x, y, GridVisualType.Green);
            }
        }
    }

    private void GetMaterialGrid(int x, int y, GridVisualType gridType) 
    {
        if (gridSystemVisualSingleArray != null) 
        {
            material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(gridType);
            gridSystemVisualSingleArray[x, y].Show(material);
        }
    }

    public void OutSideGridPositionList() 
    {
        for (int x = 0; x < towerObject.width; x++) {
            for (int y = 0; y < towerObject.height; y++) 
            {
                GetMaterialGrid(x, y, GridVisualType.Red);
            }
        }
    }

    public void DestroyGridPositionList() 
    {
        for (int x = 0; x < towerObject.width; x++) {
            for (int y = 0; y < towerObject.height; y++) {
                Destroy(gridSystemVisualSingleArray[x, y].gameObject);
            }
        }
    }

    private void OnDisable() 
    {
        DestroyGridPositionList();
    }

}
