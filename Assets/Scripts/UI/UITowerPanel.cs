using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITowerPanel : MonoBehaviour
{
    public int prefabIndex;

   public void OnButtonPress()
    {
        ResourceManager.Instance.SetSelectedPrefabIndex(prefabIndex);
    }

}
