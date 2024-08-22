using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public event Action<float, float> OnUseNature;
    public event Action<float, float> OnFullNature;


    [Header("Parameter")]
    private float natureAmount;
    [SerializeField] private float natureAmountMax;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject gameVictoryUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject towerAttackDirectionJoystickUI;
    [SerializeField] private GameObject towerInfoUI;
    [SerializeField] private GameObject towerSellInfoUI;

    [Header("Class")]
    [SerializeField] private UIBattlePanel uiBattlePanel;

    public float NatureAmount { get { return natureAmount; } }

    public void Init(int TargetCount, int EnemyDeathCount)
    {
        uiBattlePanel.Init(this, TargetCount, EnemyDeathCount);
    }

    public void NatureBarInit(float amount)
    {
        natureAmountMax = amount;
        natureAmount = amount;
    }

    public void UseNature(int amount)
    {
        natureAmount -= amount;
        OnUseNature?.Invoke(GetNatureNormalized(), natureAmount);

        if(natureAmount < 0)
        {
            natureAmount = 0;
        }
    }

    public void FullNature(float amount)
    {
        natureAmount += amount;
        OnFullNature?.Invoke(GetNatureNormalized(), natureAmount);

        if(natureAmount > natureAmountMax)
        {
            natureAmount = natureAmountMax;
        }
    }

    public float GetNatureNormalized()
    {
        return (float)natureAmount / natureAmountMax;
    }


    public void ShowGameVictoryUI()
    {
        gameVictoryUI.SetActive(true);
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    public void ShowDirectionJoystickUI(Vector3 towerTr)
    {
        towerAttackDirectionJoystickUI.GetComponentInChildren<JoystickController>().towerTr = towerTr;
        towerAttackDirectionJoystickUI.SetActive(true);
    }

    //public void TogglePauseLabel

    public void HideDirectionJoystickUI()
    {
        towerAttackDirectionJoystickUI.SetActive(false);
    }

    public void ShowTowerInfoUI()
    {
        towerInfoUI.SetActive(true);
        GameManager.Instance.Pause(0f);
    }

    public void HideTowerInfoUI()
    {
        towerInfoUI.SetActive(false);
        GameManager.Instance.Resume();
    }

    public void ShowTowerSellInfoUI()
    {
        towerSellInfoUI.SetActive(true);
        GameManager.Instance.Pause(0f);
    }

    public void HideTowerSellInfoUI()
    {
        towerSellInfoUI.SetActive(false);
        GameManager.Instance.Resume();
    }

    public GameObject GetJoystickPanel() => towerAttackDirectionJoystickUI;
    public Canvas GetCanvas() => canvas;

}
