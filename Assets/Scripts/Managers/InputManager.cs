using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private LayerMask mousePlaneLayerMask;
    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorldPosition;
    }

    public bool GetPosition(out Vector2 position)
    {
        Vector3 clickPosition = GetMouseWorldPosition();
        RaycastHit2D raycastHit = Physics2D.Raycast(clickPosition, Vector2.zero, 0f, mousePlaneLayerMask);
        if (raycastHit.collider != null)
        {
            position = raycastHit.point;
            return true;
        }

        position = Vector2.zero;
        return false;
    }
}
