using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;


public class UITower : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public UnityAction<Vector2> OnDragAction;
    public UnityAction<Tower> OnTapDownAction;
    public UnityAction OnTapReleaseAction;
    private Tower towerData;
    private Image image;

    

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void InitialiseWithData(Tower tData) 
    {
        towerData = tData;
        image.sprite = towerData.sprite.sprite;
    }


    public void OnPointerDown(PointerEventData pointerEvent)
    {
        if (OnTapDownAction != null)
        {
            OnTapDownAction(towerData);
        }
    }

    public void OnDrag(PointerEventData pointerEvent)
    {
        if (OnDragAction != null)
        {
            OnDragAction(pointerEvent.delta);
        }
    }

    public void OnPointerUp(PointerEventData pointerEvent) 
    {
        if (OnTapReleaseAction != null)
        {
            OnTapReleaseAction();
        }
    }

}
