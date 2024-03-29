using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelCollection", menuName = "ScriptableObjects/LevelCollection", order = 0)]

public class LevelCollection : ScriptableObject
{
    public List<Level> levels;

    public Level this[int i]
    {
        get { return levels[i]; }
        set { levels[i] = value; }
    }

    public int Count { get { return levels.Count; } }
}
