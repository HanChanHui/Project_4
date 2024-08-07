using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private CardManager cardManager;
    private InputManager inputManager;
    private UIManager uiManager;



    bool timePaused = false;
    Tower tower;

    public Tower Tower { get { return tower; } }

    private void Awake() 
    {
        uiManager = UIManager.Instance;
        cardManager = CardManager.Instance;

        cardManager.OnCardUsed += UseCard;
    }

    private void Start() 
    {
        uiManager.Init();
        cardManager.LoadDeck();
    }
 
    public bool Pause() {
        if (timePaused) {
            return true;
        }

        Time.timeScale = 0;
        timePaused = true;
        return false;
    }

    public void Resume() {
        if (timePaused == false) {
            return;
        }

        Time.timeScale = 1;
        timePaused = false;
    }

    public void UseCard(CardData cardData, Vector3 position) 
    {
        for (int pNum = 0; pNum < cardData.towerData.Length; pNum++) {
            PlaceableTowerData pDataRef = cardData.towerData[pNum];
            //Quaternion rot = (pFaction == Placeable.Faction.Player) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
            //Prefab to spawn is the associatedPrefab if it's the Player faction, otherwise it's alternatePrefab. But if alternatePrefab is null, then first one is taken
            GameObject prefabToSpawn = pDataRef.towerPrefab;
            GameObject newPlaceableGO = Instantiate<GameObject>(prefabToSpawn, position + cardData.relativeOffsets[pNum], Quaternion.identity);

            //SetupPlaceable(newPlaceableGO, pDataRef);

        }

    }


    // private void SetupPlaceable(GameObject go, PlaceableTowerData pDataRef) {
    //     //Add the appropriate script
    //     switch (pDataRef.pType) {
    //         case Placeable.PlaceableType.Unit:
    //             Unit uScript = go.GetComponent<Unit>();
    //             uScript.Activate(pFaction, pDataRef); //enables NavMeshAgent
    //             uScript.OnDealDamage += OnPlaceableDealtDamage;
    //             uScript.OnProjectileFired += OnProjectileFired;
    //             AddPlaceableToList(uScript); //add the Unit to the appropriate list
    //             UIManager.AddHealthUI(uScript);
    //             break;

    //         case Placeable.PlaceableType.Building:
    //         case Placeable.PlaceableType.Castle:
    //             Building bScript = go.GetComponent<Building>();
    //             bScript.Activate(pFaction, pDataRef);
    //             bScript.OnDealDamage += OnPlaceableDealtDamage;
    //             bScript.OnProjectileFired += OnProjectileFired;
    //             AddPlaceableToList(bScript); //add the Building to the appropriate list
    //             UIManager.AddHealthUI(bScript);

    //             //special case for castles
    //             if (pDataRef.pType == Placeable.PlaceableType.Castle) {
    //                 bScript.OnDie += OnCastleDead;
    //             }

    //             navMesh.BuildNavMesh(); //rebake the Navmesh
    //             break;

    //         case Placeable.PlaceableType.Obstacle:
    //             Obstacle oScript = go.GetComponent<Obstacle>();
    //             oScript.Activate(pDataRef);
    //             navMesh.BuildNavMesh(); //rebake the Navmesh
    //             break;

    //         case Placeable.PlaceableType.Spell:
    //             //Spell sScript = newPlaceable.AddComponent<Spell>();
    //             //sScript.Activate(pFaction, cardData.hitPoints);
    //             //TODO: activate the spell and… ?
    //             break;
    //     }

    //     go.GetComponent<Placeable>().OnDie += OnPlaceableDead;
    // }

    // private void AddPlaceableToList(ThinkingPlaceable p) {
    //     allThinkingPlaceables.Add(p);

    //     if (p.faction == Placeable.Faction.Player) {
    //         allPlayers.Add(p);

    //         if (p.pType == Placeable.PlaceableType.Unit)
    //             playerUnits.add(p);
    //         else
    //             playerBuildings.Add(p);
    //     } else if (p.faction == Placeable.Faction.Opponent) {
    //         allOpponents.Add(p);

    //         if (p.pType == Placeable.PlaceableType.Unit)
    //             opponentUnits.Add(p);
    //         else
    //             opponentBuildings.Add(p);
    //     } else {
    //         Debug.LogError("플레이서블을 플레이어/상대 리스트에 추가하는 중 오류 발생");
    //     }
    // }
    // private void RemovePlaceableFromList(ThinkingPlaceable p) {
    //     allThinkingPlaceables.Remove(p);

    //     if (p.faction == Placeable.Faction.Player) {
    //         allPlayers.Remove(p);

    //         if (p.pType == Placeable.PlaceableType.Unit)
    //             playerUnits.Remove(p);
    //         else
    //             playerBuildings.Remove(p);
    //     } else if (p.faction == Placeable.Faction.Opponent) {
    //         allOpponents.Remove(p);

    //         if (p.pType == Placeable.PlaceableType.Unit)
    //             opponentUnits.Remove(p);
    //         else
    //             opponentBuildings.Remove(p);
    //     } else {
    //         Debug.LogError("플레이서블을 플레이어/상대 리스트에서 제거하는 중 오류 발생");
    //     }
    // }

    // public void OnEndGameCutsceneOver() {
    //     UIManager.ShowGameOverUI();
    // }



}
