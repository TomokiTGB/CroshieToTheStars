using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "Entity/Level")]
public class Level : ScriptableObject 
{
    public List<GameObject> enemies;
    public List<Sprite> bg;

}
