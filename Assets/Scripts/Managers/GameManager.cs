using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [Header("Parameter")]
    [SerializeField] private float natureAmount;
    [SerializeField] private float natureAmountMax;



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

    private void Start() 
    {
        NatureBarInit(100);
    }
    


    private void NatureBarInit(int amount)
    {
        natureAmountMax = amount;
        natureAmount = 0;
    }

    public void UseNature(int amount)
    {
        natureAmount -= amount;
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
