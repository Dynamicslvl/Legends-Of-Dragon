using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public List<Sprite> sprite;
    void Start()
    {
        transform.GetChild(5).GetComponent<Image>().sprite = sprite[GameManager.instance.highlevel];
        transform.GetChild(5).GetComponent<Image>().SetNativeSize();
    }
}
