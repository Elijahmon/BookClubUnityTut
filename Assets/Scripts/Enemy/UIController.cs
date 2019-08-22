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
    Sprite singleFiremodeSprite;
    [SerializeField]
    Sprite burstFiremodeSprite;
    [SerializeField]
    Sprite rapidFiremodeSprite;

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

    private void Start()
    {
        instance = this;
    }
}
