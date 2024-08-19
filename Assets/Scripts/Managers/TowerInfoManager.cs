using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TowerInfoManager : Singleton<TowerInfoManager> 
{
    [Header("tower Elements")]
    [SerializeField] private RectTransform backuptowerTransform;


    private GameObject previewUIHolder;
    [SerializeField] GameObject newTowerSpriteUI;

    private void Start() 
    {
        previewUIHolder = new GameObject("previewUIHolder");
        previewUIHolder.AddComponent<RectTransform>();
        previewUIHolder.transform.SetParent(UIManager.Instance.GetCanvas().transform);
        previewUIHolder.transform.position = new Vector3(0, 0, 0);
    }


    public void PromoteTowerFromTowerUI(UITower uiTower) 
    {
        // Card 이벤트에 리스너 설정
        uiTower.OnTapDownAction += CardTapped;
        uiTower.OnDragAction += CardDragged;
        uiTower.OnTapReleaseAction += CardReleased;
    }

    //새 카드를 덱에 추가
    private void AddCardToDeck(Tower tower) //TODO: pass in the CardData dynamically
    {
        backuptowerTransform = Instantiate<GameObject>(newTowerSpriteUI, previewUIHolder.transform).GetComponent<RectTransform>();
        backuptowerTransform.localScale = Vector3.one * 1.5f;
        backuptowerTransform.GetComponent<Image>().sprite = tower.sprite.sprite;
        backuptowerTransform.anchoredPosition = tower.transform.position;
        backuptowerTransform.SetAsLastSibling();
    }

    private void CardTapped(Tower tower) 
    {
        UIManager.Instance.ShowTowerSellInfoUI();
        tower.gameObject.SetActive(false);
        AddCardToDeck(tower);
    }

    private void CardDragged() 
    {
        previewUIHolder.transform.position = InputManager.Instance.GetMouseWorldPosition();

        // if (InputManager.Instance.GetPosition(out Vector3 position)) 
        // {
        //     if (!cardIsActive) 
        //     {
        //         cardIsActive = true;
        //         previewHolder.transform.position = position;
        //         cards[cardId].ChangeActiveState(true); // 카드 숨기기

        //         // CardData에서 배열을 가져옵니다.
        //         Vector3 offsets = cards[cardId].cardData.relativeOffsets;
        //         towerGridPositionList = new List<GridPosition>();

        //         // 슬로우 시간
        //         GameManager.Instance.Pause(0.2f);
        //         GridSystemVisual.Instance.UpdateGridVisual(dataToSpawn.towerType, true);

        //         // 미리보기 PlaceableTower를 생성하고 cardPreview에 부모로 설정
        //         newPlaceable = GameObject.Instantiate<GameObject>(dataToSpawn.towerIconPrefab,
        //                                                                         position + offsets,
        //                                                                         Quaternion.identity,
        //                                                                         previewHolder.transform);
        //         newPlaceable.GetComponent<TowerVisualGrid>().Init(dataToSpawn);
        //     } 
        //     else 
        //     {
        //         // 임시 복사본이 생성되었으며 커서와 함께 이동합니다.
        //         UpdateMousePosition(previewHolder);
        //         HandleMouseHoldAction(dataToSpawn, position);
                
        //     }
        // } 
        // else 
        // {
        //     if (cardIsActive) 
        //     {
        //         IsPlaceable = false;
        //         cardIsActive = false;
        //         cards[cardId].ChangeActiveState(false); // 카드 표시

        //         ClearPreviewObjects();
        //     }
        // }
    }

    // 카드를 놓을 때 호출
    private void CardReleased() 
    {
        // // 플레이 필드에 카드가 있는지 확인하기 위해 레이캐스트를 수행
       

        // if (InputManager.Instance.GetPosition(out Vector3 position) && IsPlaceable 
        //     && (int)UIManager.Instance.NatureAmount >= dataToSpawn.towerCost) 
        // {
        //     cardIsActive = false;
        //     if (OnCardUsed != null)
        //     {
        //         //GameManager가 실제 Placeable을 생성하도록 요청
        //         OnCardUsed(cards[cardId].cardData, resultTowerGridPos, towerGridPositionList, dataToSpawn.towerCost); 
        //     }
            
        //     UIManager.Instance.ShowDirectionJoystickUI(resultTowerGridPos);
        //     GridSystemVisual.Instance.UpdateGridVisual(dataToSpawn.towerType, false);

        //     ClearPreviewObjects();
        //     Destroy(cards[cardId].gameObject); // 카드를 제거

        //     StartCoroutine(PromoteCardFromDeck(cardId, .2f));
        //     StartCoroutine(AddCardToDeck(.6f));
        // } 
        // else 
        // {
        //     GameManager.Instance.Resume();
        //     GridSystemVisual.Instance.UpdateGridVisual(dataToSpawn.towerType, false);
        //     cardIsActive = false;
        //     ClearPreviewObjects();
        //     cards[cardId].GetComponent<RectTransform>().DOAnchorPos(new Vector2(-310f + (200f * cardId), 0f),
        //                                                             0.2f).SetEase(Ease.OutQuad);
        //     cards[cardId].ChangeActiveState(false);
        // }
    }
}
