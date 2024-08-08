using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public event Action<float, float> OnUseNature;

    [Header("Parameter")]
    [SerializeField] private float natureAmount;
    [SerializeField] private float natureAmountMax;

    public float NatureAmount { get { return natureAmount; } }


    public void Init(int natureAmount) 
    {
        NatureBarInit(natureAmount);
    }


    private void NatureBarInit(int amount)
    {
        natureAmountMax = amount;
        natureAmount = 0;
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
        if(natureAmount > natureAmountMax)
        {
            natureAmount = natureAmountMax;
        }
    }

    public float GetNatureNormalized()
    {
        return (float)natureAmount / natureAmountMax;
    }

}
