using System;
using Consts;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour {
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private Transform gridSystemVisualFloorPrefab;
    [SerializeField] private Transform gridSystemVisualBlockPrefab;
    public Transform GridSystemVisualFloorPrefab { get {return gridSystemVisualFloorPrefab; } }
    public Transform GridSystemVisualBlockPrefab { get {return gridSystemVisualBlockPrefab; } }
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    
    [Serializable]
    public struct GridVisualTypeMaterial {
        public GridVisualType gridVisualType;
        public Material material;
    }

    private GridSystemVisualSingle[,] gridSystemVisualOneLayerArray;
    private GridSystemVisualSingle[,] gridSystemVisualTwoLayerArray;


    private void Awake() {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (levelGrid == null) 
        {
            levelGrid = GameObject.Find("LevelGrid").GetComponent<LevelGrid>();

            if (levelGrid == null) {
                Debug.LogError("LevelGrid 오브젝트를 찾을 수 없습니다. LevelGrid를 할당해주세요.");
            }
        }
    }

    private void Start() 
    {

        // 1층 그리드 시각화 초기화
        gridSystemVisualOneLayerArray = new GridSystemVisualSingle[
            levelGrid.GetWidth(),
            levelGrid.GetHeight()
        ];
        // 2층 그리드 시각화 초기화
        gridSystemVisualTwoLayerArray = new GridSystemVisualSingle[
            levelGrid.GetWidth(),
            levelGrid.GetHeight()
        ];

        for (int x = 0; x < levelGrid.GetWidth(); x++) 
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++) 
            {
                GridPosition gridPosition = new GridPosition(x, y);


                if (levelGrid.HasAnyBlockTypeOnGridPosition(gridPosition))
                {
                    GridPosition newGridPosition = new GridPosition(x, y, 2);
                    GridSystemVisualSelect(gridSystemVisualBlockPrefab, newGridPosition, x, y);
                }
                GridSystemVisualSelect(gridSystemVisualFloorPrefab, gridPosition, x, y);
            }
        }
    }

    private void GridSystemVisualSelect(Transform prefab, GridPosition gridPosition, int x, int y)
    {
        Vector3 worldPosition = levelGrid.GetWorldPosition(gridPosition);
        worldPosition.y -= 0.24f; // 임시
        Transform gridSystemVisualOneLayerTransform = Instantiate(prefab, worldPosition, Quaternion.identity);
        gridSystemVisualOneLayerTransform.transform.parent = transform;
        gridSystemVisualOneLayerArray[x, y] = gridSystemVisualOneLayerTransform.GetComponent<GridSystemVisualSingle>();
    }

    public void HideAllGridPosition() 
    {
        for (int x = 0; x < levelGrid.GetWidth(); x++) 
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++) 
            {
                gridSystemVisualOneLayerArray[x, y].Hide();
                gridSystemVisualTwoLayerArray[x, y].Hide();
            }
        }
    }

    public void ShowAllGridPosition() 
    {
        for (int x = 0; x < levelGrid.GetWidth(); x++) 
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++) 
            {
                gridSystemVisualOneLayerArray[x, y].Show(GetGridVisualMaterial());
                gridSystemVisualTwoLayerArray[x, y].Show(GetGridVisualMaterial());
            }
        }
    }

    public void DestroyGridPositionList() 
    {
        for (int x = 0; x < levelGrid.GetWidth(); x++) 
        {
            for (int y = 0; y < levelGrid.GetHeight(); y++) 
            {
                Destroy(gridSystemVisualOneLayerArray[x, y].gameObject);
            }
        }
    }

    public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) 
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList) 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) 
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

    public Material GetGridVisualMaterial() 
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList) 
        {
            return gridVisualTypeMaterial.material;
        }
        return null;
    }
}
