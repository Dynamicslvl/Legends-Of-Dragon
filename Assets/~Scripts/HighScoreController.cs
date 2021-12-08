using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreController : MonoBehaviour
{
    public string text;
    TextMeshProUGUI highscore;
    void Start()
    {
        highscore = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        highscore.text = text + GameManager.instance.highscore.ToString();
    }
}
