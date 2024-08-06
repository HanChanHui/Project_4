using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINatureBar : MonoBehaviour
{
    
    // private Image barImage;
    // private Image fullBarIamge;
    // UITowerSelectPanel natureSystem;

    // [SerializeField] private float fillSpeed = 1; // 채워지는 속도
    // [SerializeField] private float natureFillInterval = 0.0001f; // 채워지는 간격

    // private int previousNatureSegment = 0;
    // private void Awake()
    // {
    //     barImage = transform.Find("NaturaBar").GetComponent<Image>();
    //     fullBarIamge = transform.Find("FullBarIamge").GetComponent<Image>();
    // }

    // private void Start() {
    //     natureSystem = new UITowerSelectPanel(100);
    //     SetNature(natureSystem.GetNatureNormalized());

    //     natureSystem.OnFullNature += UITowerSelectPanel_OnFullNature;
    //     natureSystem.OnUseNature += UITowerSelectPanel_OnUseNature;

    //     StartCoroutine(FillNature());
    // }

    // private void Update() {

    //     int currentNatureSegment = Mathf.FloorToInt(natureSystem.GetNatureNormalized() * 10);

    //     if (currentNatureSegment > previousNatureSegment)
    //     {
    //         previousNatureSegment = currentNatureSegment;
    //         StartCoroutine(FillBarImage());
    //     }
    // }

    // private IEnumerator FillNature()
    // {
    //     while (true)
    //     {
    //         natureSystem.FullNature(fillSpeed * natureFillInterval);
    //         SetNature(natureSystem.GetNatureNormalized());
    //         yield return new WaitForSeconds(natureFillInterval);
    //     }
    // }

    // private IEnumerator FillBarImage()
    // {
    //     int fullSpeed = 1;
    //     while (barImage.fillAmount < previousNatureSegment * 0.1f)
    //     {
    //         barImage.fillAmount += fullSpeed * Time.deltaTime;
    //         yield return null;
    //     }
    // }

    // private void UITowerSelectPanel_OnFullNature(object sender, System.EventArgs e)
    // {
    //     SetNature(natureSystem.GetNatureNormalized());
    // }

    // private void UITowerSelectPanel_OnUseNature(object sender, System.EventArgs e)
    // {
    //     SetNature(natureSystem.GetNatureNormalized());
    //     barImage.fillAmount = fullBarIamge.fillAmount;
    // }

    // private void SetNature(float natureNormalized)
    // {
    //     fullBarIamge.fillAmount = natureNormalized;
    // }
}
