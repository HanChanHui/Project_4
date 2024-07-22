using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static MouseManager instance;


    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private GameObject prefabs;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 position;
            bool isHit = GetPosition(out position);

            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(position);
            Vector2 gridTr = LevelGrid.Instance.GetWorldPosition2(gridPosition);

            if (isHit) {
                GameObject tower = (GameObject)Instantiate(prefabs, gridTr, Quaternion.identity);
                tower.GetComponent<SpriteRenderer>().sortingOrder = -gridPosition.y;
            }
        }
    }

    public bool GetPosition(out Vector2 position)
    {
        // 클릭한 위치의 레이캐스트 검사
        Vector3 clickPosition = GetMouseWorldPosition();
        RaycastHit2D raycastHit = Physics2D.Raycast(clickPosition, Vector2.zero, 0f, mousePlaneLayerMask);
        if (raycastHit.collider != null) 
        {
            position = raycastHit.point;
            return true;
        }

        position = clickPosition; // 레이캐스트 실패 시 클릭 위치 반환
        return false;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // 2D이므로 z 축을 0으로 설정
        return mouseWorldPosition;
    }

}
