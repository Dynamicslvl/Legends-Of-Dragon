using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float timeDestroy;

    void Update()
    {
        timeDestroy = Mathf.Max(0, timeDestroy - Time.deltaTime);
        if(timeDestroy == 0)
        {
            Destroy(gameObject);
        }
    }
}
