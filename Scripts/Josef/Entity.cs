using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Entity/Entity")]
public class Entity : ScriptableObject
{
    public MainStats mStats;
    public ConditionStats cStats;
}