using Consts;
using System.Collections.Generic;
using UnityEngine;

public class TowerVisualGrid : MonoBehaviour
{

    [SerializeField] private TowerObject towerObject;

    private GridPosition gridPosition;
    public GridPosition GridPosition { get{ return gridPosition; } set{ gridPosition = value; }}

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;


    private void Start() 
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            towerObject.width,
            towerObject.height
        ];
        
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        Material material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Green);
        if (towerObject.width > 1 && towerObject.height > 1) 
        {
            for (int x = 0; x < towerObject.width; x++) 
            {
                for (int y = 0; y < towerObject.height; y++) 
                {
                    GridPosition testgridPosition = new GridPosition(x, y);
                    Vector3 worldPosition = transform.position + new Vector3(x, y);

                    Transform gridSystemVisualSingleTransform = Instantiate(GridSystemVisual.Instance.GridSystemVisualSingPrefab, worldPosition, Quaternion.identity);
                    gridSystemVisualSingleTransform.transform.parent = transform;
                    gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                    gridSystemVisualSingleArray[x, y].Show(material);
                    gridSystemVisualSingleArray[x, y].GridLayerChange("PlaceGrid");
                }
            }
        }
        else
        {
            GridPosition testgridPosition = new GridPosition(0, 0) + gridPosition;
            Transform gridSystemVisualSingleTransform = Instantiate(GridSystemVisual.Instance.GridSystemVisualSingPrefab2, transform.position, Quaternion.identity);
            gridSystemVisualSingleTransform.transform.parent = transform;
            gridSystemVisualSingleArray[0, 0] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            gridSystemVisualSingleArray[0, 0].Show(material);
            gridSystemVisualSingleArray[0, 0].GridLayerChange("PlaceGrid");
        }
        
    }

    public void ShowSingleTowerGridPositionRange(GridPosition gridPosition) 
    {
        Material material;

        if (!LevelGrid.Instance.IsValidGridPosition(gridPosition) ||
            LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition)) 
        {
            material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Red);
            gridSystemVisualSingleArray[0, 0].Show(material);
            return;
        }

        material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Green);
        gridSystemVisualSingleArray[0, 0].Show(material);
    }

    public void ShowRangeTowerGridPositionRange(GridPosition gridPosition) 
    {
        Material material;
        for (int x = 0; x < towerObject.width; x++) 
        {
            for (int y = 0; y < towerObject.height; y++) 
            {
                GridPosition testGridPosition = new GridPosition(x, y) + gridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) 
                {
                    material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Red);
                    gridSystemVisualSingleArray[x, y].Show(material);
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition) ||
                    LevelGrid.Instance.HasAnyBlockOnGridPosition(testGridPosition)) 
                {
                    material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Red);
                    gridSystemVisualSingleArray[x, y].Show(material);
                    continue;
                }
                
                material = GridSystemVisual.Instance.GetGridVisualTypeMaterial(GridVisualType.Green);
                gridSystemVisualSingleArray[x, y].Show(material);
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
