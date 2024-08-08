using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public event Action<int> OnEnemyDeath;

    private CardManager cardManager;
    private UIManager uiManager;

    [SerializeField] private int natureAmount;
    [SerializeField] private int enemyWave;
    [SerializeField] private int enemyDeathCount;
    [SerializeField] private int enemyMaxDeathCount;

    private List<Tower> towerList = new List<Tower>();
    private List<BaseEnemy> enemyList = new List<BaseEnemy>();
    

    public int EnemyMaxDeathCount {get{return enemyMaxDeathCount;} set{enemyMaxDeathCount = value;}}

    


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
        uiManager.Init(natureAmount);
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

    public void TwoSpeedTime()
    {
        Time.timeScale = 2;
    }

    public void UseCard(CardData cardData, Vector3 position, List<GridPosition> towerGridPositionList, int towerCost) 
    {
        PlaceableTowerData pDataRef = cardData.towerData;
        GameObject prefabToSpawn = pDataRef.towerPrefab;
        Tower newPlaceableGO = Instantiate(prefabToSpawn, position, Quaternion.identity).GetComponentInChildren<Tower>();
        uiManager.UseNature(towerCost);
        foreach (GridPosition gridPosition in towerGridPositionList) 
        {
            newPlaceableGO.GridPositionList.Add(gridPosition);
        }

        AddPlaceableTowerList(newPlaceableGO);
        //SetupPlaceable(newPlaceableGO, pDataRef);

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

    public void AddPlaceableTowerList(Tower tower) 
    {
        if(tower != null)
        {
            towerList.Add(tower);
        }
    }

    public void AddPlaceableEnemyList(BaseEnemy enemy) 
    {
        if(enemy != null)
        {
            enemyList.Add(enemy);
        }
    }

    public void RemovePlaceableTowerList(Tower tower) 
    {
        if(tower != null)
        {
            towerList.Remove(tower);
        }
    }

    public void RemovePlaceableEnemyList(BaseEnemy enemy) 
    {
        if(enemy != null)
        {
            enemyList.Remove(enemy);
            enemyDeathCount++;
            OnEnemyDeath?.Invoke(enemyDeathCount);
        }
    }

    private void DestroyObject()
    {
        if(towerList != null)
        {
            foreach (Tower tower in towerList) {
                Destroy(tower);
            }
        }

        if(enemyList != null)
        {
            foreach (BaseEnemy enemy in enemyList) {
                Destroy(enemy);
            }
        }

        towerList = null;
        enemyList = null;
    }
    

    // public void OnEndGameCutsceneOver() {
    //     UIManager.ShowGameOverUI();
    // }

    private void OnDisable() 
    {
        DestroyObject();
    }


}
