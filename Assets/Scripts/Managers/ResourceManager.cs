using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;


    [SerializeField] private List<TowerObject> prefabs;
    public List<TowerObject> Prefabs{get{return prefabs;}}
    [SerializeField] private Transform enemyTarget;
    public Transform EnemyTarget{get { return enemyTarget;}}
    public GameObject enemyPrefab;

    [SerializeField] private Transform gridSystemVisualSingPrefab;
    [SerializeField] private Transform gridSystemVisualSingPrefab2;
    public Transform GridSystemVisualSingPrefab { get {return gridSystemVisualSingPrefab; } }
    public Transform GridSystemVisualSingPrefab2 { get {return gridSystemVisualSingPrefab2; } }

    private int selectedPrefabIndex = -1; // 선택된 프리팹 인덱스
    public int SelectedPrefabIndex {get { return selectedPrefabIndex; } set{ selectedPrefabIndex = value; }}


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetSelectedPrefabIndex(int index)
    {
        if (index > prefabs.Count - 1)
        {
            Debug.LogError("Invalid index");
            return;
        }
        selectedPrefabIndex = index;
    }
}
