using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Enemies")]
    public List<GameObject> enemies;
    public List<Transform> spawnPoints;

    public List<GameObject> spawnGroups;
    private Dictionary<string, GameObject> spawnGroupName = new Dictionary<string, GameObject>();


    public int maxSpawns = 20;
    bool spawnEnemies;

    int lastSpawned=-1; //old spawner

    public List<Enemy> enemyType;
    public List<EnemySprites> enemySprites;

    public AudioSource fireProjectile;

    [Header("PowerUps")]
    public List<Sprite> powerUpSprites;
    public GameObject powerUpPrefab;
    bool spawnPowerUps;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerUp());
    }

    private void Update()
    {
        spawnEnemies = GameManager.Instance.gameStates == GameStates.Play;
        spawnPowerUps = GameManager.Instance.gameStates == GameStates.Play;
    }
    IEnumerator SpawnEnemies() 
    {
        while (true) 
        {
            while (spawnEnemies) 
            {
                GameObject enemy = enemies[Random.Range(0, enemies.Count)];
                Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
                //GameObject newEnemy = GetSpawn(enemy.name, enemy, spawn.position);
                GameObject newEnemy = Spawn(enemy, spawn.position);
                Threat threat = newEnemy.GetComponent<Threat>();
                threat.mainStats.health = threat.enemy.mStats.health;
                yield return new WaitForSeconds(5f - (1+ ((GameManager.Instance.levelVisually - 1) / 3f) * 2f));
                //Debug.Log(5f - (1 + ((GameManager.Instance.levelVisually - 1) / 3f) * 2f));
            }
            yield return null;
        }
    }
    IEnumerator SpawnPowerUp() 
    {
        while (true) 
        {
            while (spawnPowerUps) 
            {
                GameObject newPowerUp = Spawn(powerUpPrefab, Vector3.zero - (Vector3.up*50f) + (Vector3.left*Random.Range(-30f,30f)));
                PowerUpGet powerUp = newPowerUp.GetComponent<PowerUpGet>();
                powerUp.powerUpType = (PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpType)).Length);
                SpriteRenderer power_sR = newPowerUp.GetComponent<SpriteRenderer>();
                switch (powerUp.powerUpType)
                {
                    case PowerUpType.Shield:
                        power_sR.sprite = powerUpSprites[2];
                        break;
                    case PowerUpType.Daze:
                        power_sR.sprite = powerUpSprites[1];
                        break;
                    case PowerUpType.Boost:
                        power_sR.sprite = powerUpSprites[0];
                        break;
                    case PowerUpType.Hp:
                        power_sR.sprite = powerUpSprites[3];
                        break;
                }
                yield return new WaitForSeconds(20 - (GameManager.Instance.levelVisually * 3));
            }
            yield return null;
        }
    }
    #region OldSpawners
    //Creates a group Obj if there's none;
    private GameObject SpawnGroup(string spawnType)
    {
        if (!spawnGroupName.ContainsKey(spawnType))
        {
            GameObject newGroup = new GameObject(spawnType);
            newGroup.transform.parent = this.transform;

            spawnGroupName[spawnType] = newGroup;
            spawnGroups.Add(newGroup);
        }

        return spawnGroupName[spawnType];
    }

    public GameObject GetSpawn(string spawnType, GameObject prefab, Vector3 spawnPos)
    {
        GameObject group = SpawnGroup(spawnType);
        if (group.transform.childCount < maxSpawns)
        {
            //
            foreach (Transform spawn in group.transform)
            {
                if (!spawn.gameObject.activeInHierarchy)
                {
                    if (spawn.GetComponent<Threat>() != null)
                    {
                        //recycles
                        Threat threat = spawn.GetComponent<Threat>();
                        //SetSprite(threat);
                        if (threat.enemy != null)
                        {
                            threat.mainStats.health = threat.enemy.mStats.health;
                        }
                        if (threat.projectile != null)
                        {
                            threat.mainStats.health = threat.projectile.mStats.health;
                        }
                    }
                    spawn.transform.position = spawnPos;
                    spawn.gameObject.SetActive(true);
                    return spawn.gameObject;
                }
            }
            GameObject newSpawn = Instantiate(prefab, spawnPos, Quaternion.identity);
            newSpawn.transform.parent = group.transform;
            //newSpawn.transform.localPosition = new Vector3(spawnPos.x, spawnPos.y, 0);
            //SetSprite(newSpawn.GetComponent<Threat>());
            return newSpawn;
        }
        else
        {
            for (int i = 0; i<group.transform.childCount; i++)
            {
                // Calculate the next spawn index cyclically
                lastSpawned = (lastSpawned + 1) % group.transform.childCount;

                Transform spawn = group.transform.GetChild(lastSpawned);
                if (!spawn.gameObject.activeInHierarchy)
                {
                    spawn.gameObject.SetActive(true);
                    //SetSprite(spawn.GetComponent<Threat>());
                    return spawn.gameObject;
                }
            }
            Transform oldSpawn = group.transform.GetChild(lastSpawned);
            oldSpawn.transform.position = spawnPos;
            oldSpawn.gameObject.SetActive(true);

            return oldSpawn.gameObject;
        }
    }

    public void ReturnSpawn(string spawnType, GameObject spawn)
    {
        GameObject group = SpawnGroup(spawnType);


        if (group.transform.childCount >= maxSpawns)
        {
            Destroy(group.transform.GetChild(0).gameObject);
        }

        spawn.SetActive(false);
        spawn.transform.parent = group.transform;
    }
    #endregion
    public GameObject Spawn(GameObject @object, Vector3 spawnPos)
    {
        //search for the parent
        foreach (Transform parent in gameObject.transform)
        {
            if (parent.name == "P_" + @object.name)
            {
                // if there are any inactive gameobjects => use that
                foreach (Transform child in parent)
                {
                    if (!child.gameObject.activeSelf)
                    {
                        child.transform.SetAsLastSibling();
                        child.transform.position = spawnPos;
                        child.gameObject.SetActive(true);
                        return child.gameObject;
                    }
                }
                // if there aren't => instantiate one
                return Instantiate(@object, spawnPos, Quaternion.identity, parent.transform);
            }
        }
        //if there isn't one
        #region Creates a new Parent and Instantiate a child in it.
        //Creates a new parent
        GameObject newParent = new GameObject();
        newParent.transform.parent = gameObject.transform;
        newParent.name = "P_" + @object.name;
        //summons the object
        return Instantiate(@object, spawnPos, Quaternion.identity, newParent.transform); 
        #endregion
    }

    public void PutAfterTheInactives(GameObject currentObject)
    {
        int inactiveChildren = 0;
        Transform parentTransform = currentObject.transform.parent;

        //for counting how many inactive child there is in the parent
        foreach (Transform child in parentTransform)
        {
            //if child is inactive => adds to the count
            if (!child.gameObject.activeSelf)
            {
                inactiveChildren++;
            }
        }
        //move it after the last inactive child
        currentObject.transform.SetSiblingIndex(inactiveChildren); 
    }
}

[System.Serializable]
public class EnemySprites
{
    public int levelNo;
    [Space]
    public List<Sprite> slow_speed;
    public List<Sprite> normal_seed;
    public List<Sprite> fast_speed;
    [Space]
    public List<EnemyWithProjectileSprites> limit_projectile;
    public List<EnemyWithProjectileSprites> unli_projectile;
}
[System.Serializable]
public class EnemyWithProjectileSprites
{
    public Sprite enemySprite;
    public Sprite projectile;
    public Sprite noAmmoSprite;
}