using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    Default,
    Falling,
    Selecting,
    Merging,
}

public class BoardController : MonoBehaviour
{
    public List<GameObject> effect;
    [HideInInspector] public float peekTime;
    [HideInInspector] public float mergeTime;
    [HideInInspector] public int dragonLevel;
    [HideInInspector] public BoardState state;
    [SerializeField] List<GameObject> Grass;
    [HideInInspector] public readonly SlotController[,] SLOT = new SlotController[5, 5];
    static Vector3 pivot;
    int addScore = 0;
    void Start()
    {
        dragonLevel = 3;
        state = BoardState.Falling;
        peekTime = 0.15f;
        #region SET PIVOT
        pivot = transform.position + new Vector3(-4, +4, 0);
        #endregion
        #region SET SLOTS
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                SLOT[i, j] = Instantiate(Grass[1 - (i + j) % 2], Grid2Real(i, j), Quaternion.identity).GetComponent<SlotController>();
                SLOT[i, j].Board = this;
            }
        }
        #endregion
        SetDragons();
        if (IsGameOver())
        {
            GameManager.instance.gameOver = true;
        }
        GameManager.instance.highlevel = Mathf.Max(GameManager.instance.highlevel, dragonLevel-1);
    }

    #region FUNCTIONS
    public static Vector2 Grid2Real(int i, int j)
    {
        return (Vector2)(pivot + new Vector3(j * 2, -i * 2, 0));
    }
    public static Vector2Int Real2Grid(Vector2 position)
    {
        position -= (Vector2)pivot;
        return new Vector2Int((int) Mathf.Round(position.y / -2), (int)Mathf.Round(position.x / 2));
    }
    public int RandomValue()
    {
        /*
        int omega = dragonLevel * (dragonLevel - 1) / 2;
        int alpha = Random.Range(0, omega);
        int beta = dragonLevel;
        int value = 0;
        for(; value < dragonLevel; value++)
        {
            if(alpha < beta)
            {
                break;
            } else
            {
                beta += dragonLevel - value;
            }
        }
        */
        return Random.Range(0, dragonLevel);
    }
    bool IsInside(Vector2Int gridPositon)
    {
        if (gridPositon.x < 0 || gridPositon.x >= 5 || gridPositon.y < 0 || gridPositon.y >= 5)
        {
            return false;
        }
        return true;
    }
    bool IsAvainable(Vector2Int gridPositon, int value, bool select)
    {
        if (!IsInside(gridPositon))
        {
            return false;
        }
        SlotController slot = SLOT[gridPositon.x, gridPositon.y];
        if (slot.value != value || slot.select != select)
        {
            return false;
        }
        return true;
    }
    readonly int[] d1 = { 0, 0, -1, 1 };
    readonly int[] d2 = { -1, 1, 0, 0 };
    #endregion
    #region BFS SELECT
    public void BFS_Select(Vector2Int gridPositon)
    {
        int value = SLOT[gridPositon.x, gridPositon.y].value;
        int depth = 0;
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(gridPositon); 
        SLOT[gridPositon.x, gridPositon.y].ChangeSelectStates();
        while (queue.Count != 0)
        {
            Vector2Int u = queue.Dequeue();
            for(int i = 0; i<4; i++)
            {
                Vector2Int v = new Vector2Int(u.x + d1[i], u.y + d2[i]);
                if (IsAvainable(v, value, false))
                {
                    SLOT[v.x, v.y].ChangeSelectStates();
                    queue.Enqueue(v);
                }
            }
            if(queue.Count == 0 && depth == 0)
            {
                state = BoardState.Default;
                SLOT[gridPositon.x, gridPositon.y].ChangeSelectStates();
                return;
            }
            depth++;
        }
    }
    #endregion
    #region BFS MERGE
    public void BFS_Merge(Vector2Int gridPositon)
    {
        addScore = 1;
        int value = SLOT[gridPositon.x, gridPositon.y].value;
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(new Vector3Int(gridPositon.x, gridPositon.y, 0));
        List<Vector3Int> tmp = new List<Vector3Int> { new Vector3Int(gridPositon.x, gridPositon.y, 0)};
        SLOT[gridPositon.x, gridPositon.y].ChangeSelectStates();
        BFS.Clear();
        BFS.Add(tmp);
        while (queue.Count != 0)
        {
            Vector3Int u = queue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                Vector3Int v = new Vector3Int(u.x + d1[i], u.y + d2[i], u.z + 1);
                if (IsAvainable((Vector2Int) v, value, true))
                {
                    SLOT[v.x, v.y].ChangeSelectStates();
                    queue.Enqueue(v);
                    PREV[v.x, v.y] = (Vector2Int) u;
                    if(BFS.Count <= v.z)
                    {
                        BFS.Add(new List<Vector3Int>());
                    }
                    BFS[v.z].Add(v);
                    addScore++;
                }
            }
        }
        addScore *= (value + 1);
        mergeTime = 0.1f;
        foreach(List<Vector3Int> uList in BFS)
        {
            foreach(Vector2Int u in uList)
            {
                SLOT[u.x, u.y].ChangeSelectStates();
            }
        }
    }
    #endregion
    #region SET DRAGONS
    public void SetDragons()
    {
        for(int j = 0; j<5; j++)
        {
            int order = 0; //Ordering the new dragons
            for(int i = 4; i>=0; i--)
            {
                if(SLOT[i, j].select)
                {
                    if (SLOT[i, j].dragon != null) SLOT[i, j].DestroyDragon();
                    SLOT[i, j].ChangeSelectStates();
                }
                if(SLOT[i, j].dragon == null)
                {
                    int h;
                    bool needCreateNew = true;
                    for (h = 1; i - h >= 0; h++)
                    {
                        if(SLOT[i - h, j].dragon != null && !SLOT[i-h, j].select)
                        {
                            needCreateNew = false;
                            break;
                        }
                    }
                    if (needCreateNew)
                    {
                        order++;
                        SLOT[i, j].CreateDragon(i + order);
                    } else
                    {
                        SLOT[i, j].dragon = SLOT[i - h, j].dragon;
                        SLOT[i, j].value = SLOT[i - h, j].value;
                        SLOT[i, j].dragon.transform.SetParent(SLOT[i, j].transform);
                        SLOT[i - h, j].dragon = null;
                    }
                }
            }
        }
    }
    #endregion

    [HideInInspector] readonly Vector2Int[,] PREV = new Vector2Int[5, 5];
    [HideInInspector] readonly List<List<Vector3Int>> BFS = new List<List<Vector3Int>>();

    float alarm = 0;
    bool isEnding = false;
    public void CancelSelect()
    {
        for(int i = 0; i<5; i++)
        {
            for(int j = 0; j<5; j++)
            {
                if (SLOT[i, j].select)
                {
                    SLOT[i, j].ChangeSelectStates();
                }
            }
        }
    }
    public bool IsGameOver()
    {
        for(int i = 0; i<5; i++)
        {
            for(int j = 0; j<5; j++)
            {
                bool ok = false;
                for(int k = 0; k<4; k++)
                {
                    Vector2Int u = new Vector2Int(i + d1[k], j + d2[k]);
                    if (IsInside(u))
                    {
                        if(SLOT[i,j].value == SLOT[u.x, u.y].value)
                        {
                            ok = true;
                            break;
                        }
                    }
                }
                if (ok)
                {
                    return false;
                }
            }
        }
        FindObjectOfType<AudioManager>().Play("GameOver");
        return true;
    }
    float fallTime = 0.5f;
    void Update()
    {
        #region DRAGON FADING
        if (state == BoardState.Merging)
        {
            if(BFS.Count > 1)
            {
                if (alarm == 0)
                {
                    alarm = mergeTime;
                    List<Vector3Int> tmp = BFS[BFS.Count - 1];
                    foreach(Vector2Int u in tmp)
                    {
                        SLOT[u.x, u.y].autoDestroyDragon = true;
                        SLOT[u.x, u.y].fadePosition = PREV[u.x, u.y];
                        SLOT[u.x, u.y].tDestroy = mergeTime;
                    }
                    BFS.RemoveAt(BFS.Count - 1);
                    mergeTime = Mathf.Max(0.000001f, mergeTime * 0.7f);
                }
            } else
            {
                if(alarm == 0)
                {
                    if (isEnding)
                    {
                        alarm = 0;
                        state = BoardState.Falling;
                        SLOT[BFS[0][0].x, BFS[0][0].y].ChangeSelectStates();
                        SetDragons();
                        isEnding = false;
                        GameManager.instance.score += addScore;
                        FindObjectOfType<ScoreController>().GetComponent<Animator>().SetTrigger("Increase");
                        if (IsGameOver())
                        {
                            GameManager.instance.gameOver = true;
                        }
                    }
                    else
                    {
                        alarm = peekTime;
                        SLOT[BFS[0][0].x, BFS[0][0].y].EvolveDragon();
                        isEnding = true;
                    }
                }
            }
            alarm -= Time.deltaTime;
            if (alarm <= 0) alarm = 0;
        }
        #endregion
        #region Timing Falling
        if(state == BoardState.Falling)
        {
            fallTime = Mathf.Max(0, fallTime - Time.deltaTime);
            if(fallTime == 0)
            {
                fallTime = 0.5f;
                state = BoardState.Default;
            }
        }
        #endregion
    }
}
