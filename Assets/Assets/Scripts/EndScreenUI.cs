using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public GameObject endScreen;
    public GameObject winText;
    public GameObject loseText;

    public static EndScreenUI instance;

    void Awake ()
    {
        instance = this;
    }

    public void SetEndScreen (bool win)
    {
        endScreen.SetActive(true);

        winText.SetActive(win);
        loseText.SetActive(!win);
    }

    public void OnPlayAgainButton ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}