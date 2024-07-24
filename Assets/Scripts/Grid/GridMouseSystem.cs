using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMouseSystem : MonoBehaviour
{
    private InputActionAsset  inputActions;
    private InputAction mouseClickAction;

    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake() {
        inputActions = Resources.Load<InputActionAsset>("InputSystem/PlayerInputActions");
        
        // Player 액션 맵에서 MouseClick 액션을 가져옵니다.
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
            Unit unit = Instantiate(ResourceManager.Instance.Prefabs[index]).GetComponentInChildren<Unit>();

            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);
            List<GridPosition> gridPositionList = unit.GetGridPositionList(gridTr);
            foreach(GridPosition gridPos in gridPositionList)
            {
                if(HasAnyGridObject(gridPos))
                {
                    Debug.Log("유닛이 존재 합니다");
                    return;
                }
            }

            //AddressableManager.Instance.InstantiatePrefab(gridTr, Quaternion.identity);
            foreach(GridPosition gridPos in gridPositionList)
            {
                LevelGrid.Instance.AddUnitAtGridPosition(gridPos, unit);
            }
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
