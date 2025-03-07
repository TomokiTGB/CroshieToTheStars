using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour
{
    public GameManager gM;
    public Enemy enemy;
    public Projectile projectile;
    public Rigidbody rb;
    public SpeedType speedType;
    public Player player;

    public MainStats mainStats;
    public bool dazed;
    public ShooterType currentShooterType;

    public SpriteRenderer sR;

    public int projectileAmount = 2;
    private float spawnIntervals = 1.5f;
    private float timer;

    [SerializeField] int slowIndex;
    [SerializeField] int normalIndex;
    [SerializeField] int fastIndex;
    [SerializeField] int unliIndex;
    [SerializeField] int limitIndex;
    Sprite projectileSprite;

    List<EnemySprites> accessListSprites;
    int levelIndex;

    // put whatever renderer were using
    private void Start()
    {
        gM = GameManager.Instance;
        player = gM.player;
        rb = GetComponent<Rigidbody>();
        sR = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        accessListSprites = SpawnManager.Instance.enemySprites;
        levelIndex = (GameManager.Instance.levelVisually - 1);
        if (GameManager.Instance.gameStates == GameStates.Play)
        {
            dazed = false;
            if (projectile != null)
            {
                enemy = null;
            }
            if (enemy != null)
            {
                slowIndex = Random.Range(0, accessListSprites[levelIndex].slow_speed.Count);
                normalIndex = Random.Range(0, accessListSprites[levelIndex].normal_seed.Count);
                fastIndex = Random.Range(0, accessListSprites[levelIndex].fast_speed.Count);
                limitIndex = Random.Range(0, accessListSprites[levelIndex].limit_projectile.Count);
                unliIndex = Random.Range(0, accessListSprites[levelIndex].unli_projectile.Count);
                int enemyIndex = -1;
                while (enemyIndex == -1)
                {
                    int tempIndex = Random.Range(0, 5);
                    switch (tempIndex)
                    {
                        case 0:
                            if (accessListSprites[levelIndex].slow_speed.Count > 0)
                            {
                                //Debug.Log("Level " + (levelIndex+1) + " " + accessListSprites[levelIndex].slow_speed[slowIndex].name + ": === Slow");
                                enemyIndex = tempIndex;
                            }
                            break;
                        case 1:
                            if (accessListSprites[levelIndex].normal_seed.Count > 0)
                            {
                                //Debug.Log("Level " + (levelIndex + 1) + " " + accessListSprites[levelIndex].normal_seed[normalIndex].name + ": === Normal");
                                enemyIndex = tempIndex;
                            }
                            break;
                        case 2:
                            if (accessListSprites[levelIndex].fast_speed.Count > 0)
                            {
                                //Debug.Log("Level " + (levelIndex + 1) + " " + accessListSprites[levelIndex].fast_speed[fastIndex].name + ": === Fast");
                                enemyIndex = tempIndex;
                            }
                            break;
                        case 3:
                            if (accessListSprites[levelIndex].limit_projectile.Count > 0)
                            {
                                //Debug.Log("Level " + (levelIndex + 1) + " " + accessListSprites[levelIndex].limit_projectile[limitIndex].enemySprite.name + ": === Limit projectile");
                                enemyIndex = tempIndex;
                            }
                            break;
                        case 4:
                            if (accessListSprites[levelIndex].unli_projectile.Count > 0)
                            {
                                //Debug.Log("Level " + (levelIndex + 1) + " " + accessListSprites[levelIndex].unli_projectile[unliIndex].enemySprite.name + ": === Unli projectile");
                                enemyIndex = tempIndex;
                            }
                            break;
                    }
                    //if (SpawnManager.Instance.enemySprites[GameManager.Instance.level] == )
                }
                enemy = SpawnManager.Instance.enemyType[enemyIndex];
                mainStats.health = enemy.mStats.health;
                currentShooterType = enemy.pStats.shooterType;
                speedType = enemy.speedType;
            }
            else if (projectile != null)
            {
                mainStats.health = projectile.mStats.health;
                speedType = projectile.speedType;
            }
            Invoke("SetSprite", 0.001f);
        }       
    }

    public void Update()
    {
        if (gM.gameStates == GameStates.Play)
        {
            if (enemy != null)
            {
                GetComponent<SpriteRenderer>().color = dazed ? Color.green : Color.white;

                mainStats.speed = (int)speedType;
                MoveToTarget(player.transform, mainStats.speed);
                //Movement(enemy.speedType, mainStats.speed, player.transform);
                if (mainStats.health <= 0)
                {
                    PointsAdd();
                    gameObject.SetActive(false);
                }
                if (timer < spawnIntervals)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0f;
                }
                if (timer == 0)
                {
                    //Instantiate(enemy.pStats.projectilePrefab, transform.position, Quaternion.identity);
                    //SpawnManager.Instance.GetSpawn(enemy.pStats.projectilePrefab.name, enemy.pStats.projectilePrefab, transform.position);


                    if (enemy.pStats.shooterType != ShooterType.None)
                    {
                        if (projectileAmount > 0)
                        {
                            //Vector3 pos = gameObject.transform.position;
                            GameObject projectile = SpawnManager.Instance.Spawn(enemy.pStats.projectilePrefab, transform.position);
                            //GameObject projectile = SpawnManager.Instance.GetSpawn(enemy.pStats.projectilePrefab.name, enemy.pStats.projectilePrefab, transform.position);
                            Threat projThreat = projectile.GetComponent<Threat>();
                            projThreat.limitIndex = limitIndex;
                            projThreat.unliIndex = unliIndex;
                            projThreat.currentShooterType = currentShooterType;
                            projThreat.projectileSprite = projectileSprite;
                            SpawnManager.Instance.fireProjectile.Play();
                            if (currentShooterType == ShooterType.Limit)
                            {
                                projectileAmount--;
                            }
                        }
                        else
                        {
                            currentShooterType = ShooterType.None;
                            sR.sprite = accessListSprites[levelIndex].limit_projectile[limitIndex].noAmmoSprite;
                        }
                    }
                }
            }
            else if (projectile != null)
            {
                mainStats.speed = (int)speedType;
                MoveToTarget(player.transform, mainStats.speed);
                //Movement(projectile.speedType, mainStats.speed, player.transform);
                if (mainStats.health <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
            Vector3 rotation = transform.localEulerAngles;
            sR.flipY = Mathf.Abs((rotation.z > 180) ? (rotation.z - 360) : (rotation.z)) > 90;
            //Debug.Log(transform.localEulerAngles.z);

            float targetRotation = GameManager.Instance.AngleOfTwo(transform.position, GameManager.Instance.bubble.transform.position);
            transform.eulerAngles = new Vector3(0, 0, targetRotation);
        }
      
    }


    void PointsAdd() 
    {
        int points = 0;
        switch (enemy.speedType)
        {

            case SpeedType.Low:
                points += 1;
                break;
            case SpeedType.Mid:
                points += 3;
                break;
            case SpeedType.Fast:
                points += 5;
                break;

        }
        if (enemy.pStats.shooterType != ShooterType.None)
        {
            points *= 2;
        }
        gM.kills += points;
    }

    void MoveToTarget(Transform target,float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    void Movement(SpeedType speedType, float speed,Transform target)
    {
        speed = (int)speedType;
        MoveToTarget(target,speed);

    }

    public void SetSprite()
    {
        //Debug.Log(gameObject.name + ": -" + speedType.ToString());
        if (enemy != null)
        {
            switch (currentShooterType)
            {
                case ShooterType.None:
                    switch (speedType)
                    {
                        case SpeedType.Low:
                            sR.sprite = accessListSprites[levelIndex].slow_speed[slowIndex];
                            break;
                        case SpeedType.Mid:
                            sR.sprite = accessListSprites[levelIndex].normal_seed[normalIndex];
                            break;
                        case SpeedType.Fast:
                            sR.sprite = accessListSprites[levelIndex].fast_speed[fastIndex];
                            break;
                    }
                    break;
                case ShooterType.Has:
                    {
                        projectileAmount = 2;
                        sR.sprite = accessListSprites[levelIndex].unli_projectile[unliIndex].enemySprite;
                        projectileSprite = accessListSprites[levelIndex].unli_projectile[unliIndex].projectile;
                        /*enemy.pStats.projectilePrefab.GetComponent<Threat>().GetComponent<SpriteRenderer>().sprite 
                            = accessListSprites[index].unli_projectile[unliIndex].projectile;*/
                        break;
                    }

                case ShooterType.Limit:
                    {
                        projectileAmount = 2;
                        sR.sprite = accessListSprites[levelIndex].limit_projectile[limitIndex].enemySprite;
                        projectileSprite = accessListSprites[levelIndex].limit_projectile[limitIndex].projectile;
                        /*if (SpawnManager.Instance.enemySprites[GameManager.Instance.levelVisually - 1].limit_projectile[limitIndex].projectile != null)
                        {
                            enemy.pStats.projectilePrefab.GetComponent<Threat>().GetComponent<SpriteRenderer>().sprite
                                = SpawnManager.Instance.enemySprites[GameManager.Instance.levelVisually - 1].limit_projectile[limitIndex].projectile;
                        }
                        else
                        {

                        }*/
                        break;
                    }
            }

        }
        if (projectile != null)
        {
            //sR.sprite = SpawnManager.Instance.enemySprites[GameManager.Instance.level - 1].;
            /*switch (currentShooterType)
            {
                case ShooterType.None:
                    break;
                case ShooterType.Has:
                    sR.sprite = accessListSprites[levelIndex].unli_projectile[unliIndex].projectile;
                    break;
                case ShooterType.Limit:
                    sR.sprite = accessListSprites[levelIndex].limit_projectile[limitIndex].projectile;
                    break;
            }*/
            sR.sprite = projectileSprite;
        }
    }
}



