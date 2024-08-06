using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITowerSelectPanel : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private float natureAmount;
    [SerializeField] private float natureAmountMax;

    [SerializeField] private float fillSpeed = 1; // 채워지는 속도
    [SerializeField] private float natureFillInterval = 0.0001f; // 채워지는 간격

    [Header("UI")]
    [SerializeField] private Image barImage;
    [SerializeField]private Image fullBarIamge;

    private int previousNatureSegment = 0;

    private void Start()
    {
        NatureBarInit(100);

        SetNature(GetNatureNormalized());

        StartCoroutine(FillNature());
    }

    private void Update() {

        int currentNatureSegment = Mathf.FloorToInt(GetNatureNormalized() * 10);

        if (currentNatureSegment > previousNatureSegment)
        {
            previousNatureSegment = currentNatureSegment;
            StartCoroutine(FillBarImage());
        }
    }

    private void NatureBarInit(int amount)
    {
        natureAmountMax = amount;
        natureAmount = 0;
    }

    private IEnumerator FillNature()
    {
        while (true)
        {
            FullNature(fillSpeed * natureFillInterval);
            SetNature(GetNatureNormalized());
            yield return new WaitForSeconds(natureFillInterval);
        }
    }

    private IEnumerator FillBarImage()
    {
        int fullSpeed = 1;
        while (barImage.fillAmount < previousNatureSegment * 0.1f)
        {
            barImage.fillAmount += fullSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void SetNature(float natureNormalized)
    {
        fullBarIamge.fillAmount = natureNormalized;
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
