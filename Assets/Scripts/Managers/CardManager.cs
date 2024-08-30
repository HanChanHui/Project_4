using Consts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;

namespace HornSpirit {
    public class CardManager : Singleton<CardManager> {

        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private DeckData playersDeck;
        [SerializeField] private InputManager input;
        [SerializeField] private LevelGrid levelGrid;
        [SerializeField] private GridSystemVisual gridSV;

        public UnityAction<CardData, Vector3, List<GridPosition>, int> OnCardUsed;
        public event Action OnDrawFinish;

        [Header("UI Elements")]
        [SerializeField] private RectTransform backupCardTransform;
        [SerializeField] private RectTransform cardSpawnPanel;
        [SerializeField] private RectTransform cardsPanel;

        [SerializeField] private UICard[] cards;
        private bool cardIsActive = false;
        private bool IsPlaceable = false;
        private GameObject previewHolder;
        private GameObject newPlaceable;
        [SerializeField] private IPlaceable  dataToSpawn;
        private List<GridPosition> placeableGridPositionList = new List<GridPosition>();
        private Vector3 resultPlaceableGridPos = new Vector3();
        private int drawCount = 0;

        private void Awake() {
            previewHolder = new GameObject("PreviewHolder");
            cards = new UICard[4];
        }

        // 덱 Addressable에서 Load
        public void LoadDeck() {
            DeckLoader newDeckLoaderComp = gameObject.AddComponent<DeckLoader>();
            newDeckLoaderComp.OnDeckLoaded += DeckLoaded;
            newDeckLoaderComp.LoadDeck(playersDeck);
        }

        // 덱 펼치기
        private void DeckLoaded() {
            Debug.Log("Player's deck loaded");

            //setup initial cards
            StartCoroutine(AddCardToDeck(.1f));
            for (int i = 0; i < cards.Length; i++) {
                StartCoroutine(PromoteCardFromDeck(i, 0.4f + i));
                StartCoroutine(AddCardToDeck(0.8f + i));
            }
        }

        // 덱에서 대시보드로 카드를 이동
        private IEnumerator PromoteCardFromDeck(int position, float delay = 0f) {
            yield return new WaitForSeconds(delay);

            backupCardTransform.SetParent(cardSpawnPanel, true);
            //위치로 이동하고 스케일 조정
            backupCardTransform.DOAnchorPos(new Vector2(-310f + (200f * position), 0f),
                                            0.2f + (0.05f * position)).SetEase(Ease.OutQuad);
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
            drawCount++;
            if(drawCount == 5)
            {
                OnDrawFinish?.Invoke();
            }
        }

        // 카드가 탭 될 때 호출
        private void CardTapped(int cardId) {
            cards[cardId].GetComponent<RectTransform>().SetAsLastSibling();
        }

        // 카드가 드래그 될 때 호출
        private void CardDragged(int cardId, Vector2 dragAmount) {
            cards[cardId].transform.Translate(dragAmount);

            dataToSpawn = cards[cardId].cardData.placeableData;
            if (input.GetPosition(out Vector3 position)) {
                if (!cardIsActive) {
                    cardIsActive = true;
                    previewHolder.transform.position = position;
                    cards[cardId].ChangeActiveState(true); // 카드 숨기기

                    // CardData에서 배열을 가져옵니다.
                    Vector3 offsets = cards[cardId].cardData.relativeOffsets;
                    placeableGridPositionList = new List<GridPosition>();

                    // 슬로우 시간
                    GameManager.Instance.Pause(0.2f);
                    gridSV.UpdateGridVisual(true, dataToSpawn.gridRangeType);

                    // 미리보기 PlaceableTower를 생성하고 cardPreview에 부모로 설정
                    newPlaceable = GameObject.Instantiate<GameObject>(dataToSpawn.IconPrefab,
                                                                      position + offsets,
                                                                      Quaternion.identity,
                                                                      previewHolder.transform);
                    
                    if (dataToSpawn is PlaceableTowerData towerData) 
                    {
                        gridSV.UpdateGridVisual(true, dataToSpawn.gridRangeType, towerData.towerType);
                        newPlaceable.GetComponent<TowerVisualGrid>().Init();
                    } else if (dataToSpawn is PlaceableBlockData) {
                        gridSV.UpdateGridVisual(true, dataToSpawn.gridRangeType);
                        newPlaceable.GetComponent<BlockSprite>().Init();
                    }
                } else {
                    // 임시 복사본이 생성되었으며 커서와 함께 이동합니다.
                    UpdateMousePosition(previewHolder);
                    HandleMouseHoldAction(dataToSpawn, position);
                }
            } 
            else 
            {
                if (cardIsActive) {
                    IsPlaceable = false;
                    cardIsActive = false;
                    cards[cardId].ChangeActiveState(false); // 카드 표시

                    ClearPreviewObjects();
                }
            }
        }

        // 카드를 놓을 때 호출
        private void CardReleased(int cardId) {
            // 플레이 필드에 카드가 있는지 확인하기 위해 레이캐스트를 수행

            if (input.GetPosition(out Vector3 position) && IsPlaceable
                && (int)GameManager.Instance.NatureAmount() >= dataToSpawn.placeableCost) {
                cardIsActive = false;
                if (OnCardUsed != null) {
                    //GameManager가 실제 Placeable을 생성하도록 요청
                    OnCardUsed(cards[cardId].cardData, resultPlaceableGridPos, placeableGridPositionList, dataToSpawn.placeableCost);
                }

                gridSV.UpdateGridVisual(false, dataToSpawn.gridRangeType);
                if (dataToSpawn is PlaceableBlockData) {
                    GameManager.Instance.Resume();
                    AstarPath.active.Scan();
                }

                ClearPreviewObjects();
                Destroy(cards[cardId].gameObject); // 카드를 제거

                StartCoroutine(PromoteCardFromDeck(cardId, .2f));
                StartCoroutine(AddCardToDeck(.6f));
            } else {
                GameManager.Instance.Resume();
                gridSV.UpdateGridVisual(false, dataToSpawn.gridRangeType);
                cardIsActive = false;
                ClearPreviewObjects();
                cards[cardId].GetComponent<RectTransform>().DOAnchorPos(new Vector2(-310f + (200f * cardId), 0f),
                                                                        0.2f).SetEase(Ease.OutQuad);
                cards[cardId].ChangeActiveState(false);
            }
        }

        private void HandleMouseHoldAction(IPlaceable placeableData, Vector3 position) 
        {
            Vector3 pos = position;
            if (levelGrid.HasAnyBlockTypeOnWorldPosition(pos)) {
                pos.z = 2f;
            }
            GridPosition gridPosition = levelGrid.GetGridPosition(pos);
            Vector2 gridTr = levelGrid.GetWorldPosition(gridPosition);
            placeableGridPositionList.Clear();

            if (placeableData is PlaceableTowerData towerData) {
                if (TryGetPlaceableGridPositions(gridPosition, towerData, out List<GridPosition> gridPositions)) {
                    newPlaceable.GetComponent<TowerVisualGrid>().ShowAllGridPosition();
                    placeableGridPositionList = gridPositions;
                    resultPlaceableGridPos = gridTr;
                    IsPlaceable = true;
                } else {
                    newPlaceable.GetComponent<TowerVisualGrid>().HideAllGridPosition();
                    IsPlaceable = false;
                    return;
                }
            } else if (placeableData is PlaceableBlockData blockData) {
                // Block에 대한 위치 처리 로직
                if (TryGetPlaceableGridPositions(gridPosition, blockData, out List<GridPosition> gridPositions)) {
                    placeableGridPositionList = gridPositions;
                    resultPlaceableGridPos = gridTr;
                    IsPlaceable = true;
                } else {
                    IsPlaceable = false;
                    return;
                }
            }

        }

        public void UpdateMousePosition(GameObject towerGhost) {
            Vector3 targetPosition = GetMouseWorldSnappedPosition();
            towerGhost.transform.position = targetPosition;
        }

        private Vector3 GetMouseWorldSnappedPosition() {
            Vector3 mousePosition = InputManager.Instance.GetMouseWorldPosition();
            if (levelGrid.HasAnyBlockTypeOnWorldPosition(mousePosition)) {
                mousePosition.z = 2f;
                return levelGrid.GetWorldPosition(levelGrid.GetGridPosition(mousePosition));
            } else {
                return levelGrid.GetWorldPosition(levelGrid.GetCameraGridPosition(mousePosition));
            }
        }

        private bool TryGetPlaceableGridPositions(GridPosition gridPosition, IPlaceable placeableData, out List<GridPosition> gridPositions) {
            gridPositions = new List<GridPosition>();

            if (placeableData.gridRangeType == GridRangeType.Single) {
                if (OneAndTwoLayerDivision(gridPosition, placeableData)) {
                    gridPositions.Add(gridPosition);
                    return true;
                }
            } else if (placeableData.gridRangeType == GridRangeType.Range) {
                if (placeableData.GetGridPositionList(gridPosition, out gridPositions)) {
                    return true;
                }
            }

            return false;
        }

        private bool OneAndTwoLayerDivision(GridPosition gridPosition, IPlaceable placeableData)
        {
            if(placeableData is PlaceableTowerData towerData)
            {
                switch (towerData.towerType) 
                {
                    case TowerType.Dealer:
                        return towerData.GetSingleTwoLayerGridPosition(gridPosition);
                    case TowerType.Tanker:
                        return towerData.GetSingleOneLayerGridPosition(gridPosition);
                }
            }
            else if(placeableData is PlaceableBlockData blockData)
            {
                return blockData.GetSingleOneLayerGridPosition(gridPosition) &&
                        blockData.CanPlaceBlock(GameManager.Instance.SpawnerList,
                                                    GameManager.Instance.TargetList[0].position);
            }
            return false;
        }



        private void ClearPreviewObjects() {
            // 미리보기 PlaceableTower를 모두 제거
            for (int i = 0; i < previewHolder.transform.childCount; i++) {
                Destroy(previewHolder.transform.GetChild(i).gameObject);
            }

            if (newPlaceable != null) {
                newPlaceable = null;
            }

            if (dataToSpawn != null) {
                dataToSpawn = null;
            }
        }

    }
}

