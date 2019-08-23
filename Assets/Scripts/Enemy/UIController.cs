using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{

    public static UIController instance;

    [SerializeField]
    Camera _cam;

    [SerializeField]
    Image firemodeIcon;
    [SerializeField]
    TextMeshProUGUI firemodeText;
    [SerializeField]
    GameObject menu;
    [SerializeField]
    TextMeshProUGUI gameOver;

    [SerializeField]
    Sprite singleFiremodeSprite;
    [SerializeField]
    Sprite burstFiremodeSprite;
    [SerializeField]
    Sprite rapidFiremodeSprite;

    /// <summary>
    /// Updates UI image and text for fire mode
    /// </summary>
    /// <param name="mode">New Fire Mode</param>
    public void UpdateFireMode(PlayerController.FireMode mode)
    {
        switch(mode)
        {
            case PlayerController.FireMode.SINGLE:
                firemodeText.text = "Single";
                firemodeIcon.sprite = singleFiremodeSprite;
                break;
            case PlayerController.FireMode.BURST:
                firemodeText.text = "Burst";
                firemodeIcon.sprite = burstFiremodeSprite;
                break;
            case PlayerController.FireMode.RAPID:
                firemodeText.text = "Auto";
                firemodeIcon.sprite = rapidFiremodeSprite;
                break;
        }
    }

    public void Init()
    {
        instance = this;
        firemodeIcon.gameObject.SetActive(false);
        firemodeText.gameObject.SetActive(false);
        menu.SetActive(true);
        gameOver.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        firemodeIcon.gameObject.SetActive(true);
        firemodeText.gameObject.SetActive(true);
        menu.SetActive(false);
        gameOver.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        firemodeIcon.gameObject.SetActive(false);
        firemodeText.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(true);
    }

    public void OnStartGamePressed()
    {
        GameStateManager.instance.StartGame();
    }
}
