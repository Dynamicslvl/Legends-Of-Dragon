using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [HideInInspector] public BoardController Board;
    [HideInInspector] public bool select = false;
    [HideInInspector] public bool autoDestroyDragon = false;
    [HideInInspector] public int id;
    [HideInInspector] public int value;
    public DragonStates state;
    public GameObject prefabsDragon;
    [HideInInspector] public DragonController dragon;

    void Start()
    {
        targetPosition = transform.position;
        startPosition = transform.position;
    }
    public void CreateDragon(int distance)
    {
        dragon = Instantiate(prefabsDragon, transform.position + new Vector3(0, distance*2, 0), Quaternion.identity).GetComponent<DragonController>();
        value = Board.RandomValue();
        dragon.image.GetComponent<SpriteRenderer>().sprite = state.Dragons[value].sprite[0];
        dragon.transform.SetParent(transform);
        dragon.value = value;
    }
    public void EvolveDragon()
    {
        if (value == 13) return;
        value++;
        dragon.spriteIndex = 0;
        dragon.image.GetComponent<SpriteRenderer>().sprite = state.Dragons[value].sprite[0];
        dragon.value = value;
        Board.dragonLevel = Mathf.Max(value/2, Mathf.Min(Mathf.Max(value, Board.dragonLevel), 4));
        GameManager.instance.highlevel = Mathf.Max(GameManager.instance.highlevel, value);
        if(value == 7 || value == 8 || value == 9)
        {
            Instantiate(Board.effect[1], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("EggBreak");
        }
        if(value == 5 || value == 6)
        {
            Instantiate(Board.effect[0], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("KiraKira");
        }
        if(value < 5)
        {
            dragon.animator.SetTrigger("Jump");
            Instantiate(Board.effect[1], transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            FindObjectOfType<AudioManager>().Play("Tap");
        }
    }
    public void DestroyDragon()
    {
        if(dragon != null)
        {
            DestroyImmediate(dragon, true);
        }
    }
    void OnMouseDown()
    {
        if (Board.state == BoardState.Merging || Board.state == BoardState.Falling) return;
        if (!select)
        {
            FindObjectOfType<AudioManager>().Play("Select");
            if (Board.state != BoardState.Selecting)
            {
                Board.state = BoardState.Selecting;
                Board.BFS_Select(BoardController.Real2Grid(transform.position));
            } else
            {
                Board.state = BoardState.Default;
                Board.CancelSelect();
            }
        } else
        {
            //Board.state == BoardState.Selecting for sure
            FindObjectOfType<AudioManager>().Play("Scored");
            Board.BFS_Merge(BoardController.Real2Grid(transform.position));
            Board.state = BoardState.Merging;
        }
    }

    Vector3 targetPosition, startPosition;
    public void ChangeSelectStates()
    {
        select = !select;
        if (select)
        {
            targetPosition += new Vector3(0, 0.2f, 0);
        }
        else
        {
            targetPosition += new Vector3(0, -0.2f, 0);
        }
    }

    [HideInInspector] public float tDestroy;
    [HideInInspector] public Vector2Int fadePosition;
    void Update()
    {
        #region DRAGON FALLING
        if (Board.state == BoardState.Falling)
        {
            if(dragon != null)
            {
                Vector3 diff = transform.position - dragon.transform.position;
                dragon.transform.Translate(diff * Time.deltaTime * 14f);
            }
        }
        #endregion
        #region AUTO DESTROY
        if (autoDestroyDragon)
        {
            //Timing to destroy
            tDestroy = Mathf.Max(0, tDestroy - Time.deltaTime);
            Fading();
            if(tDestroy <= 0)
            {
                ChangeSelectStates();
                tDestroy = Board.mergeTime;
                autoDestroyDragon = false;
                DestroyDragon();
            }
        }
        #endregion
        #region PEEKING
        float distance = Mathf.Abs( transform.position.y - targetPosition.y);
        if (distance < 0.01f) distance = 0;
        if(distance != 0)
        {
            transform.Translate((targetPosition - transform.position) * Time.deltaTime / Board.peekTime);
        }
        GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.cyan, Mathf.Abs(transform.position.y - startPosition.y) / 0.2f);
        #endregion
    }

    void Fading()
    {
        Vector3 diff = Board.SLOT[fadePosition.x, fadePosition.y].transform.position - dragon.transform.position;
        dragon.transform.Translate(diff * Time.deltaTime / Board.mergeTime);
        dragon.image.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, tDestroy / Board.mergeTime);
    }
}
