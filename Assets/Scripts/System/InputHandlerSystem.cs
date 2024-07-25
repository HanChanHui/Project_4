using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandlerSystem : MonoBehaviour
{
    public static InputHandlerSystem Instance { get; private set; }

    private InputActionAsset  inputActions;
    private InputAction mouseUpAction;

    [SerializeField] private LevelGrid levelGrid;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private TowerObject towerObject;
    private List<GridPosition> towerGridPositionList = new List<GridPosition>();
    private Vector2 resultTowerGridPos = new Vector2();
    private bool isDisposition = false;

    private Transform towerGhostPrefabs;

    private Coroutine mouseHoldCoroutine;

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
        mouseUpAction = playerActionMap.FindAction("MouseUp");
    }

    private void OnEnable() 
    {
        mouseUpAction.performed += HandleMouseUp;

        mouseUpAction.Enable();
    }

    public void HandleMouseDown()
    {
        Debug.Log("Mouse Down Event");
        if (mouseHoldCoroutine == null)
        {
            RefreshVisual();
            mouseHoldCoroutine = StartCoroutine(MouseHoldRoutine());
        }
    }

    private void HandleMouseUp(InputAction.CallbackContext context)
    {
        Debug.Log("Mouse Up Event");
        if(ResourceManager.Instance.SelectedPrefabIndex != -1)
        {
            if (mouseHoldCoroutine != null) {
                StopCoroutine(mouseHoldCoroutine);
                mouseHoldCoroutine = null;
            }

            RefreshVisual();
            if (isDisposition) 
            {
                HandleMouseDownAction(resultTowerGridPos);
            }
        }
    }

    private IEnumerator MouseHoldRoutine()
    {
        while (true)
        {
            MousePositionUpdate();
            if(HandleMouseHoldAction())
            {
                isDisposition = true;
            }
            else
            {
                isDisposition = false;
            }

            yield return new WaitForSeconds(0.01f); // 한 프레임 기다림
        }
    }

    // 
    private bool HandleMouseHoldAction()
    {
        Vector2 position;
        bool isHit = GetPosition(out position);

        if(isHit)
        {
            if(towerGridPositionList != null)
            {
                towerGridPositionList.Clear();
            }
            GridPosition gridPosition = levelGrid.GetGridPosition(position);
            GridSystemVisual.Instance.ShowGridPositionRange(gridPosition, towerObject.width, towerObject.height);

            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);
            List<Vector2> gridPositions = towerObject.GetGridPositionList(gridTr);
            foreach(Vector2 gridPos in gridPositions)
            {
                towerGridPositionList.Add(levelGrid.GetGridPosition(gridPos));
                if(HasAnyGridObject(towerGridPositionList[towerGridPositionList.Count - 1]))
                {
                    return false;
                }
            }
            resultTowerGridPos = gridTr;
            return true;
        }
        return false;
    }  

    private void HandleMouseDownAction(Vector2 gridTr)
    {
        Unit unit = Instantiate(towerObject.prefab, gridTr, Quaternion.identity).GetComponentInChildren<Unit>();
        foreach (GridPosition gridPos in towerGridPositionList) 
        {
            unit.GridPosition.Add(gridPos);
            levelGrid.AddUnitAtGridPosition(gridPos, unit);
        }
        int reset = -1;
        ResourceManager.Instance.SetSelectedPrefabIndex(reset);
    }

    private void MousePositionUpdate() 
    {
        Vector3 targetPosition = GetMouseWorldSnappedPosition();
        towerGhostPrefabs.position = Vector3.Lerp(towerGhostPrefabs.position, targetPosition, Time.deltaTime * 50f);
    }

    private void RefreshVisual() {
        if (towerGhostPrefabs != null) 
        {
            Destroy(towerGhostPrefabs.gameObject);
            towerGhostPrefabs = null;
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
            towerGhostPrefabs = Instantiate(towerObject.prefab, GetMousePosition(), Quaternion.identity);
            //towerGhostPrefabs.SetParent(transform);
            //towerGhostPrefabs.localPosition = Vector3.zero;
            //towerGhostPrefabs.localEulerAngles = Vector3.zero;
        }
    }

    public Vector3 GetMouseWorldSnappedPosition() 
    {
        Vector3 mousePosition = GetMousePosition();
        GridPosition gridPosition = levelGrid.GetGridPosition(mousePosition);

        if (towerObject != null) 
        {
            Vector3 placedObjectWorldPosition = levelGrid.GetWorldPosition(gridPosition);
            return placedObjectWorldPosition;
        } 
        else 
        {
            return mousePosition;
        }
    }

    public bool GetPosition(out Vector2 position)
    {
        // 클릭한 위치의 레이캐스트 검사
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

    public Vector3 GetMousePosition()
    {
        return InputManager.Instance.GetMouseWorldPosition();
    }

    private bool HasAnyGridObject(GridPosition gridPosition)
    {
        return levelGrid.HasAnyUnitOnGridPosition(gridPosition);
    }

    private void OnDisable()
    {
        mouseUpAction.performed -= HandleMouseUp;

        mouseUpAction.Disable();
    }

}
