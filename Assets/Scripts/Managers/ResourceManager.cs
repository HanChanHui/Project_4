using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;


    [SerializeField] private List<GameObject> prefabs;
    public List<GameObject> Prefabs{get{return prefabs;}}

    private int selectedPrefabIndex = -1; // 선택된 프리팹 인덱스
    public int SelectedPrefabIndex {get { return selectedPrefabIndex; } set{ selectedPrefabIndex = value; }}


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetSelectedPrefabIndex(int index)
    {
        if (index >= prefabs.Count - 1)
        {
            Debug.LogError("Invalid index");
            return;
        }
        selectedPrefabIndex = index;
    }
}
