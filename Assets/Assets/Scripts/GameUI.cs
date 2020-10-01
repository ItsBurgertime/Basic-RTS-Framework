using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI unitCountText;
    public TextMeshProUGUI foodText;

    public static GameUI instance;

    void Awake ()
    {
        instance = this;
    }

    public void UpdateUnitCountText (int value)
    {
        unitCountText.text = value.ToString();
    }

    public void UpdateFoodText (int value)
    {
        foodText.text = value.ToString();
    }
}