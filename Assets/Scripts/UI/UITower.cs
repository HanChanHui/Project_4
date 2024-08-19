using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class UITower : MonoBehaviour
{
    public UnityAction OnDragAction, OnTapReleaseAction;
    public UnityAction<Tower> OnTapDownAction;
    public Tower tower;



    private void Start() 
    {
        tower = GetComponent<Tower>();
    }


    public void OnMouseDown() {
        if (OnTapDownAction != null)
            OnTapDownAction(tower);
    }

    public void OnMouseDrag() {
        if (OnDragAction != null)
            OnDragAction();
    }

    public void OnMouseUp() {
        if (OnTapReleaseAction != null)
            OnTapReleaseAction();
    }

}
