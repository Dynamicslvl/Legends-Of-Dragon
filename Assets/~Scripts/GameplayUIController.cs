using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUIController : MonoBehaviour
{
    public GameObject GameOverCanvas;
    public void RestartButton()
    {
        FindObjectOfType<AudioManager>().Play("Notification");
        GameManager.instance.score = 0;
        GameManager.instance.highlevel = 0;
        GameManager.instance.gameOver = false;
        SceneManager.LoadScene("Gameplay");
    }
    public void HomeButton()
    {
        FindObjectOfType<AudioManager>().Play("Notification");
        GameManager.instance.score = 0;
        GameManager.instance.highlevel = 0;
        GameManager.instance.gameOver = false;
        SceneManager.LoadScene("Menu");
    }

    float timeOver;
    void Start()
    {
        timeOver = 1.5f;
    }
    void Update()
    {
        if (GameManager.instance.gameOver)
        {
            if(timeOver > 0)
            {
                timeOver = Mathf.Max(0, timeOver - Time.deltaTime);
            }
            transform.GetChild(4).GetComponent<Animator>().SetBool("IsNoMove", true);
            if(timeOver == 0)
            {
                timeOver = -1;
                var tmp = Instantiate(GameOverCanvas, transform.position, Quaternion.identity);
                tmp.GetComponent<Canvas>().worldCamera = Camera.main;
            }
        }
    }
}
