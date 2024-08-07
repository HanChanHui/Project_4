using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITowerSelectPanel : MonoBehaviour
{
    [Header("Parameter")]

    [SerializeField] private float fullBarSpeed = 0.5f; // 채워지는 속도
    [SerializeField] private float natureBarSpeed = 10f; // 채워지는 속도
    [SerializeField] private float natureFillInterval = 0.01f; // 채워지는 간격

    [Header("UI")]
    [SerializeField] private Image barImage;
    [SerializeField]private Image fullBarIamge;
    [SerializeField]private TextMeshProUGUI natureCountText;

    private int previousNatureSegment = 0;
    private int currentNatureSegment = 0;
    private bool isFilling = false;

    private void Start()
    {
        StartCoroutine(FillNature());
    }
    

    private IEnumerator FillNature()
    {
        while (!isFilling)
        {
            UIManager.Instance.FullNature(fullBarSpeed * natureFillInterval);
            float normalize = UIManager.Instance.GetNatureNormalized();
            SetNature(normalize);

            currentNatureSegment = Mathf.FloorToInt(UIManager.Instance.GetNatureNormalized() * 10);

            if (currentNatureSegment > previousNatureSegment)
            {
                previousNatureSegment = currentNatureSegment;
                StartCoroutine(SmoothFillNatureBar());
                int natureCnt = (int)UIManager.Instance.NatureAmount;
                natureCountText.text = natureCnt.ToString();
                //yield return new WaitForSeconds(0.2f); // 10단위 도달 시 멈춤
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
        StartCoroutine(FillNature());
    }

    private void SetNature(float natureNormalized)
    {
        fullBarIamge.fillAmount = natureNormalized;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    

}
