using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITowerPanel : MonoBehaviour
{

   public void OnButtonDownEvent(int index)
    {
        ResourceManager.Instance.SetSelectedPrefabIndex(index);
        InputHandlerSystem.Instance.HandleMouseDown();
    }

}
