using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TowerInfoManager : Singleton<TowerInfoManager> 
{
    [Header("tower Elements")]
    [SerializeField] private RectTransform backuptowerTransform;


    private RectTransform previewUIHolder;
    [SerializeField] private GameObject newTowerSpriteUI;
    [SerializeField] private Tower towerData;
    private UITower towerScript;
    private Vector2 originTransform;

    private Coroutine dragCoroutine;

    private void Start() 
    {
        GameObject previewUIObject = new GameObject("previewUIHolder");
        previewUIHolder = previewUIObject.AddComponent<RectTransform>();
        previewUIHolder.transform.SetParent(UIManager.Instance.GetCanvas().transform);
    }


    public void PromoteTowerFromTowerUI(Tower tData) 
    {
        tData.OnClickAction += AddCardToDeck;
    }

    //새 카드를 덱에 추가
    private void AddCardToDeck(Tower tower) //TODO: pass in the CardData dynamically
    {
        backuptowerTransform = Instantiate<GameObject>(newTowerSpriteUI, previewUIHolder.transform).GetComponent<RectTransform>();
        backuptowerTransform.SetAsLastSibling();
        backuptowerTransform.GetComponent<Image>().SetNativeSize();

        towerScript = backuptowerTransform.GetComponent<UITower>();
        towerScript.InitialiseWithData(tower);
        Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            UIManager.Instance.GetCanvas().transform as RectTransform,
            Camera.main.WorldToScreenPoint(tower.transform.position),
            UIManager.Instance.GetCanvas().worldCamera,
            out uiPosition);
        originTransform = uiPosition;
        previewUIHolder.anchoredPosition = uiPosition;

        towerScript.OnTapDownAction += TowerTapped; 
        towerScript.OnDragAction += TowerDragged;
        towerScript.OnTapReleaseAction += TowerReleased;

        TowerTapped(tower);

        dragCoroutine = StartCoroutine(DragCoroutine());
    }

    private void TowerTapped(Tower tower) 
    {
        towerData = tower;
        towerData.gameObject.SetActive(false);
        UIManager.Instance.ShowTowerSellInfoUI();
        
    }

    private void TowerDragged(Vector2 dragAmount) 
    {
        RectTransformUI(out Vector2 mousePosition);
        previewUIHolder.anchoredPosition = mousePosition;
    }

    // 카드를 놓을 때 호출
    private void TowerReleased() 
    {
        previewUIHolder.anchoredPosition = originTransform;

        if (dragCoroutine != null)
        {
            StopCoroutine(dragCoroutine);
            dragCoroutine = null;
        }
    }

    public void CloneTowerInfoUI()
    {
        towerData.gameObject.SetActive(true);
        UIManager.Instance.HideTowerSellInfoUI();
        ClearPreviewObjects();
    }

    private IEnumerator DragCoroutine()
    {
        while (true)
        {
            RectTransformUI(out Vector2 mousePosition);
            previewUIHolder.anchoredPosition = mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                TowerReleased();
                yield break; // 코루틴 종료
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    private void RectTransformUI(out Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIManager.Instance.GetCanvas().transform as RectTransform,
                Input.mousePosition,
                UIManager.Instance.GetCanvas().worldCamera,
                out mousePosition);
    }

    private void ClearPreviewObjects() 
    {
        towerScript.OnTapDownAction -= TowerTapped; 
        towerScript.OnDragAction -= TowerDragged;
        towerScript.OnTapReleaseAction -= TowerReleased;
        // 미리보기 PlaceableTower를 모두 제거
        for (int i = 0; i < previewUIHolder.transform.childCount; i++) 
        {
            Destroy(previewUIHolder.transform.GetChild(i).gameObject);
        }
    }
}
