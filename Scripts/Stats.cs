using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MainStats
{
    public float health;
    public float speed;
    public List<List<Sprite>> animations;//replace this if not sprite
}
[System.Serializable]
public class ConditionStats 
{
    public bool invincible;

    IEnumerator Wait()
    {
        invincible = true;
        yield return new WaitForSeconds(3);
        invincible = false;
    }
}
[System.Serializable]
public class ProjectileStats
{
    public ShooterType shooterType;
    public GameObject projectilePrefab;
}

