using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;


public class CardManager : Singleton<CardManager> 
{
    public Camera mainCamera; 
    public LayerMask playingFieldMask;
    public GameObject cardPrefab;
    public DeckData playersDeck;
    //public MeshRenderer forbiddenAreaRenderer;

    public UnityAction<CardData, Vector3> OnCardUsed;

    [Header("UI Elements")]
    public RectTransform backupCardTransform; 
    public RectTransform cardSpawnPanel; 
    public RectTransform cardsPanel; 

    private UICard[] cards;
    private bool cardIsActive = false;
    private GameObject previewHolder;
    private Vector3 inputCreationOffset = new Vector3(0f, 0f, 1f); 

    private void Awake() {
        previewHolder = new GameObject("PreviewHolder");
        cards = new UICard[4];
    }

    public void LoadDeck() 
    {
        DeckLoader newDeckLoaderComp = gameObject.AddComponent<DeckLoader>();
        newDeckLoaderComp.OnDeckLoaded += DeckLoaded;
        newDeckLoaderComp.LoadDeck(playersDeck);
    }


    private void DeckLoaded() {
        Debug.Log("Player's deck loaded");

        //setup initial cards
        StartCoroutine(AddCardToDeck(.1f));
        for (int i = 0; i < cards.Length; i++) {
            StartCoroutine(PromoteCardFromDeck(i, .4f + i));
            StartCoroutine(AddCardToDeck(.8f + i));
        }
    }

    // 덱에서 대시보드로 카드를 이동
    private IEnumerator PromoteCardFromDeck(int position, float delay = 0f) {
        yield return new WaitForSeconds(delay);

        backupCardTransform.SetParent(cardSpawnPanel, true);
        //위치로 이동하고 스케일 조정
        backupCardTransform.DOAnchorPos(new Vector2(-310f + (200f * position), 0f),
                                        .2f + (.05f * position)).SetEase(Ease.OutQuad);
        backupCardTransform.localScale = Vector3.one;

        //Card 컴포넌트를 배열에 저장
        UICard cardScript = backupCardTransform.GetComponent<UICard>();
        cardScript.cardId = position;
        cards[position] = cardScript;

        // Card 이벤트에 리스너 설정
        cardScript.OnTapDownAction += CardTapped;
        cardScript.OnDragAction += CardDragged;
        cardScript.OnTapReleaseAction += CardReleased;
    }

    //새 카드를 덱에 추가
    private IEnumerator AddCardToDeck(float delay = 0f) //TODO: pass in the CardData dynamically
    {
        yield return new WaitForSeconds(delay);

        //새 카드 생성
        backupCardTransform = Instantiate<GameObject>(cardPrefab, cardsPanel).GetComponent<RectTransform>();
        backupCardTransform.localScale = Vector3.one * 0.6f;

        //왼쪽 아래 모서리로 보냅니다.
        backupCardTransform.anchoredPosition = new Vector2(0f, -300f);
        backupCardTransform.DOAnchorPos(new Vector2(0f, -50f), .2f).SetEase(Ease.OutQuad);

        //Card 스크립트에 CardData 설정
        UICard cardScript = backupCardTransform.GetComponent<UICard>();
        cardScript.InitialiseWithData(playersDeck.GetNextCardFromDeck());
    }
    // 카드가 탭 될 때 호출
    private void CardTapped(int cardId) 
    {
        cards[cardId].GetComponent<RectTransform>().SetAsLastSibling();
        //forbiddenAreaRenderer.enabled = true;
    }

    // 카드가 드래그 될 때 호출
    private void CardDragged(int cardId, Vector2 dragAmount) {
        cards[cardId].transform.Translate(dragAmount);

        // 플레이 필드에 카드가 있는지 확인하기 위해 레이캐스트를 수행
        //RaycastHit hit;
        Vector3 pos = InputManager.Instance.GetMouseWorldPosition() ;

        bool planeHit = Physics2D.Raycast(pos, Vector2.zero, 0f, playingFieldMask);

        if (planeHit) {
            if (!cardIsActive) {
                cardIsActive = true;
                previewHolder.transform.position = pos;
                cards[cardId].ChangeActiveState(true); // 카드 숨기기

                // CardData에서 배열을 가져옵니다.
                PlaceableTowerData[] dataToSpawn = cards[cardId].cardData.towerData;
                Vector3[] offsets = cards[cardId].cardData.relativeOffsets;

                // 미리보기 PlaceableTower를 생성하고 cardPreview에 부모로 설정
                for (int i = 0; i < dataToSpawn.Length; i++) 
                {
                    GameObject newPlaceable = GameObject.Instantiate<GameObject>(dataToSpawn[i].towerIconPrefab,
                                                                                pos + offsets[i] + inputCreationOffset,
                                                                                Quaternion.identity,
                                                                                previewHolder.transform);
                }
            } 
            else 
            {
                // 임시 복사본이 생성되었으며 커서와 함께 이동합니다.
                previewHolder.transform.position = pos;
            }
        } 
        else 
        {
            if (cardIsActive) 
            {
                cardIsActive = false;
                cards[cardId].ChangeActiveState(false); // 카드 표시

                ClearPreviewObjects();
            }
        }
    }

    // 카드를 놓을 때 호출
    private void CardReleased(int cardId) 
    {
        // 플레이 필드에 카드가 있는지 확인하기 위해 레이캐스트를 수행
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, playingFieldMask)) 
        {
            if (OnCardUsed != null)
            {
                //GameManager가 실제 Placeable을 생성하도록 요청
                OnCardUsed(cards[cardId].cardData, hit.point + inputCreationOffset); 
            }

            ClearPreviewObjects();
            Destroy(cards[cardId].gameObject); // 카드를 제거

            StartCoroutine(PromoteCardFromDeck(cardId, .2f));
            StartCoroutine(AddCardToDeck(.6f));
        } 
        else 
        {
            cards[cardId].GetComponent<RectTransform>().DOAnchorPos(new Vector2(-310f + (200f * cardId), 0f),
                                                                    .2f).SetEase(Ease.OutQuad);
        }

        //forbiddenAreaRenderer.enabled = false;
    }

    
    private void ClearPreviewObjects() 
    {
        // 미리보기 PlaceableTower를 모두 제거
        for (int i = 0; i < previewHolder.transform.childCount; i++) {
            Destroy(previewHolder.transform.GetChild(i).gameObject);
        }
    }
}

