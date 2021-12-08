using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int score, highscore, highlevel;
    [HideInInspector] public bool gameOver;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        gameOver = false;
        score = 0;
        highlevel = 0;
        highscore = PlayerPrefs.GetInt("HI");
    }
    private void Update()
    {
        if(highscore < score)
        {
            highscore = score;
            PlayerPrefs.SetInt("HI", highscore);
        }
    }
}
