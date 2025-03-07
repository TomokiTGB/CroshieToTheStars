using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpeedType { Slow = 5, Low = 10, Mid = 20, Fast = 30, Stun = 0 }
public enum ShooterType { None, Limit, Has }
[CreateAssetMenu(fileName = "Enemy", menuName = "Entity/Enemy")]
public class Enemy : Entity
{
    public SpeedType speedType;
    public ProjectileStats pStats;
}