using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITowerSelectPanel : MonoBehaviour
{
    [Header("Parameter")]
    //[SerializeField] private float natureAmount;
    //[SerializeField] private float natureAmountMax;

    [SerializeField] private float fullBarSpeed; // 채워지는 속도
    [SerializeField] private float natureBarSpeed; // 채워지는 속도
    [SerializeField] private float natureFillInterval; // 채워지는 간격

    [Header("UI")]
    [SerializeField] private Image barImage;
    [SerializeField]private Image fullBarIamge;

    private int previousNatureSegment = 0;
    private int currentNatureSegment = 0;
    private bool isFilling = false;

    private void Start()
    {
        StartCoroutine(FillNature());
        //StartCoroutine(CheckNatureSegment());
    }



    private IEnumerator CheckNatureSegment()
    {
        while (true)
        {
            currentNatureSegment = Mathf.FloorToInt(GameManager.Instance.GetNatureNormalized() * 10);

            if (currentNatureSegment > previousNatureSegment && !isFilling)
            {
                previousNatureSegment = currentNatureSegment;
                StartCoroutine(SmoothFillNatureBar());
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FillNature()
    {
        while (true)
        {
            GameManager.Instance.FullNature(fullBarSpeed * natureFillInterval);
            SetNature(GameManager.Instance.GetNatureNormalized());

            currentNatureSegment = Mathf.FloorToInt(GameManager.Instance.GetNatureNormalized() * 10);

            if (currentNatureSegment > previousNatureSegment)
            {
                previousNatureSegment = currentNatureSegment;
                StartCoroutine(SmoothFillNatureBar());
                yield return new WaitForSeconds(0.2f); // 10단위 도달 시 멈춤
            }

            yield return new WaitForSeconds(natureFillInterval);
        }
    }

    private IEnumerator SmoothFillNatureBar()
    {
        isFilling = true;
        float targetFillAmount = previousNatureSegment * 0.1f;

        while (barImage.fillAmount < targetFillAmount)
        {
            barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, targetFillAmount, Time.deltaTime * natureBarSpeed);
            if (Mathf.Abs(barImage.fillAmount - targetFillAmount) < 0.01f)
            {
                barImage.fillAmount = targetFillAmount;
            }
            yield return null;
        }

        barImage.fillAmount = targetFillAmount; // 정확하게 목표량에 맞추기
        isFilling = false;
    }

    private void SetNature(float natureNormalized)
    {
        fullBarIamge.fillAmount = natureNormalized;
    }

    

}
