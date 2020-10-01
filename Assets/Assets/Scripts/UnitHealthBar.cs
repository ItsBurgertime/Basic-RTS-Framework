using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    public GameObject healthContainer;
    public RectTransform healthFill;

    private float maxSize;

    void Awake ()
    {
        maxSize = healthFill.sizeDelta.x;
        healthContainer.SetActive(false);
    }

    public void UpdateHealthBar (int curHp, int maxHp)
    {
        healthContainer.SetActive(true);
        float healthPercentage = (float)curHp / (float)maxHp;
        healthFill.sizeDelta = new Vector2(maxSize * healthPercentage, healthFill.sizeDelta.y);
    }
}