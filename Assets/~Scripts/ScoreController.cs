using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    TextMeshProUGUI UI;
    float score;
    void Start()
    {
        score = 0;
        UI = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        float diff = (float) GameManager.instance.score - score;
        score += (diff / 4)*Time.deltaTime*60;
        if (diff <= 0.1f) score = GameManager.instance.score;
        UI.text = ((int) score).ToString();
    }
}
