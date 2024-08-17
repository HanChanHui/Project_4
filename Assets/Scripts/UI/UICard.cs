using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
 {
    public UnityAction<int, Vector2> OnDragAction;
    public UnityAction<int> OnTapDownAction, OnTapReleaseAction;

    [HideInInspector] public int cardId;
    [HideInInspector] public CardData cardData;

    public Image portraitImage; //Inspector-set reference
    public Image towerPropertyImage;
    public TextMeshProUGUI towerCostText;
    private CanvasGroup canvasGroup;

    private bool isDragging = false;
    private float clickTime;
    private const float clickThreshold = 0.2f;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //called by CardManager, it feeds CardData so this card can display the placeable's portrait
    public void InitialiseWithData(CardData cData) 
    {
        cardData = cData;
        portraitImage.sprite = cardData.cardImage;
        towerPropertyImage.sprite = cardData.cardPropertyImage;
        towerCostText.text = cardData.towerData.towerCost.ToString();
    }

    public void OnPointerDown(PointerEventData pointerEvent) 
    {
        isDragging = false;
        clickTime = Time.time;

        if (OnTapDownAction != null)
        {
            OnTapDownAction(cardId);
        }
    }

    public void OnDrag(PointerEventData pointerEvent)
     {
        if (OnDragAction != null)
        {
            OnDragAction(cardId, pointerEvent.delta);
        }
    }

    public void OnPointerUp(PointerEventData pointerEvent) 
    {
        if (OnTapReleaseAction != null) 
        {
            // Ŭ���� �巡�� ���� ����
            if (!isDragging && (Time.time - clickTime) <= clickThreshold) {
                // ���� Ŭ������ ó��
                Debug.Log("Single click detected");
            } else {
                // �巡�� �� ����� ó��
                Debug.Log("Drag and drop detected");
            }

            OnTapReleaseAction(cardId);
        }
    }

    public void ChangeActiveState(bool isActive) 
    {
        canvasGroup.alpha = (isActive) ? .05f : 1f;
    }
}
