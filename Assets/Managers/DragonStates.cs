using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDragonStates", menuName = "Scriptable Objects/DragonStates", order = 0)]
public class DragonStates : ScriptableObject
{
    public List<Dragon> Dragons;
}

[System.Serializable]
public class Dragon
{
    public Sprite[] sprite;
}
