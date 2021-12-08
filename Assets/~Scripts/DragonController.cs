using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    bool isAppear = false;
    [HideInInspector] public int value;
    public GameObject image;
    float animationTime;
    [HideInInspector] public Animator animator;
    [HideInInspector] public int spriteIndex;
    void Start()
    {
        spriteIndex = 0;
        transform.localScale = new Vector3(1, 1, 1);
        image.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        animationTime = Random.Range(0f, 3f);
        animator = image.GetComponent<Animator>();
    }
    float t = 0.2f;
    void SpriteChanging()
    {
        Sprite[] tmp = transform.parent.GetComponent<SlotController>().state.Dragons[value].sprite;
        int limit = tmp.Length;
        t = Mathf.Max(0, t - Time.deltaTime);
        if(t == 0)
        {
            t = 0.2f;
            spriteIndex++;
            if (spriteIndex == limit)
            {
                if (value == 7)
                {
                    spriteIndex--;
                } else
                {
                    spriteIndex = 0;
                }
            }
            image.GetComponent<SpriteRenderer>().sprite = tmp[spriteIndex];
        }

    }
    void Update()
    {
        //
        SpriteChanging();
        //Appearance
        if (transform.position.y < 2.8f && !isAppear)
        {
            isAppear = true;
            image.GetComponent<SpriteRenderer>().color = Color.white;
        }
        //Animation
        if (value <= 0) return;
        if(value >= 5 && value <= 6)
        {
            animator.SetBool("IsFloating", true);
        } else
        {
            animator.SetBool("IsFloating", false);
        }
        animationTime = Mathf.Max(animationTime - Time.deltaTime, 0);
        if (animationTime == 0)
        {
            animationTime = Random.Range(3f, 6f);
            if (transform.parent.GetComponent<SlotController>().Board.state != BoardState.Falling)
            {
                if(value < 8)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }
    }
}
