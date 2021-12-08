using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Gameplay");
        FindObjectOfType<AudioManager>().Play("Notification");
    }
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("BGAudio");
    }
}
