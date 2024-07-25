using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGhost : MonoBehaviour
{
    // private Transform prefabs;
    // private TowerObject towerObject;

    // private void Start() 
    // {
    //     //InputHandlerSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    // }

    // private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
    //     RefreshVisual();
    // }

    // private void LateUpdate() {
    //     Vector3 targetPosition = GetMouseWorldSnappedPosition();
    //     transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
    // }

    // private void RefreshVisual() {
    //     if (prefabs != null) {
    //         Destroy(prefabs.gameObject);
    //         prefabs = null;
    //     }

    //     int count = ResourceManager.Instance.SelectedPrefabIndex;
    //     towerObject = ResourceManager.Instance.Prefabs[count];

    //     if (towerObject != null) 
    //     {
    //         prefabs = Instantiate(towerObject.prefab, Vector3.zero, Quaternion.identity);
    //         prefabs.parent = transform;
    //         prefabs.localPosition = Vector3.zero;
    //         prefabs.localEulerAngles = Vector3.zero;
    //     }
    // }

    // public Vector3 GetMouseWorldSnappedPosition() 
    // {
    //     Vector3 mousePosition = InputManager.Instance.GetMouseWorldPosition();
    //     GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);

    //     if (towerObject != null) 
    //     {
    //         Vector3 placedObjectWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    //         return placedObjectWorldPosition;
    //     } 
    //     else 
    //     {
    //         return mousePosition;
    //     }
    // }
}
