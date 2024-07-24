using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMouseSystem : MonoBehaviour
{
    public static GridMouseSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;

    private InputActionAsset  inputActions;
    private InputAction mouseClickAction;

    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private TowerObject towerObject;

    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CharacterActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inputActions = Resources.Load<InputActionAsset>("InputSystem/PlayerInputActions");
        
        var playerActionMap = inputActions.FindActionMap("MouseClick");
        mouseClickAction = playerActionMap.FindAction("Mouse action");
    }

    private void OnEnable() 
    {
        mouseClickAction.performed += OnMouseClick;
        mouseClickAction.Enable();
    }

    // 마우스 좌클릭 이벤트
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        Vector2 position;
        bool isHit = GetPosition(out position);

        if(isHit && ResourceManager.Instance.SelectedPrefabIndex != -1)
        {
            GridPosition gridPosition = levelGrid.GetGridPosition(position);

            int index = ResourceManager.Instance.SelectedPrefabIndex;
            towerObject = ResourceManager.Instance.Prefabs[index];

            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);
            List<Vector2> gridPositions = towerObject.GetGridPositionList(gridTr);
            List<GridPosition> gridPositionList = new List<GridPosition>();
            foreach(Vector2 gridPos in gridPositions)
            {
                gridPositionList.Add(levelGrid.GetGridPosition(gridPos));
                if(HasAnyGridObject(gridPositionList[gridPositionList.Count - 1]))
                {
                    return;
                }
            }

            Unit unit = Instantiate(towerObject.prefab, gridTr, Quaternion.identity).GetComponentInChildren<Unit>();
            foreach(GridPosition gridPos in gridPositionList)
            {
                unit.GridPosition.Add(gridPos);
                levelGrid.AddUnitAtGridPosition(gridPos, unit);
            }
            int reset = -1;
            ResourceManager.Instance.SetSelectedPrefabIndex(reset);
        }
       
    }  

    public bool GetPosition(out Vector2 position)
    {
        // 클릭한 위치의 레이캐스트 검사
        Vector3 clickPosition = InputManager.Instance.GetMouseWorldPosition();
        RaycastHit2D raycastHit = Physics2D.Raycast(clickPosition, Vector2.zero, 0f, mousePlaneLayerMask);
        if (raycastHit.collider != null) 
        {
            position = raycastHit.point;
            return true;
        }

        position = Vector2.zero;
        return false;
    }

    public void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool HasAnyGridObject(GridPosition gridPosition)
    {
        return levelGrid.HasAnyUnitOnGridPosition(gridPosition);
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= OnMouseClick;
        mouseClickAction.Disable();
    }

}
