using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum SkillState { Shield,Daze,Boost }
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public AttackSkill attackStats;
    public bool bubbleDistanceDependent;

    public ShieldSkill shieldStats;
    public DazeSkill dazeStats;
    public BoostSkill boostStats;
    public int maxHp = 3;

    public float cd;
    public float shieldCD;
    public float dazeCD;
    public float dazeCD_two;
    public float boostCd;
    public Transform wandPos;
    public Transform bubblesParent;
    public bool hasShield;
    public bool hasDaze;
    public bool hasBoost;

    public SkillState state;
    [Header("UI")]
    public List<TextMeshProUGUI> txt_levelIndicator;
    public Image dazeEffect;
    [Header("Audios")]
    public AudioSource wandUse;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (PlayerPrefs.HasKey("Shield Level"))
        shieldStats.levelShield = PlayerPrefs.GetInt("Shield Level");
        if (PlayerPrefs.HasKey("Daze Level"))
        dazeStats.levelDaze = PlayerPrefs.GetInt("Daze Level");
        if (PlayerPrefs.HasKey("Boost Level"))
        boostStats.levelBoost = PlayerPrefs.GetInt("Boost Level");
        if (PlayerPrefs.HasKey("Health Level"))
            maxHp = PlayerPrefs.GetInt("Health Level") + 2;
    }
    private void Start()
    {
        cd = 0;
        dazeCD_two = 0;
        GameManager.Instance.player.stats.health = maxHp;
        StartCoroutine(Attack());
        StartCoroutine(Shield());
        StartCoroutine(Daze());
        StartCoroutine(Boost());
    }
    private void Update()
    {
        /*if (GameManager.Instance.levelChanged)
        {
            hasShield = true;
            hasDaze = true;
            hasBoost = true;
        }*/
        txt_levelIndicator[0].text = "- Level 0" + (maxHp-2);
        txt_levelIndicator[1].text = "- Level 0" + shieldStats.levelShield;
        txt_levelIndicator[2].text = "- Level 0" + dazeStats.levelDaze;
        txt_levelIndicator[3].text = "- Level 0" + boostStats.levelBoost;
    }
    IEnumerator Attack() 
    {
        while (true)
        {
            if (cd > 0)
            {
                cd--;
            }
            else 
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Q) && GameManager.Instance.gameStates == GameStates.Play);
                if (GameManager.Instance.gameStates.Equals(GameStates.Play))
                {
                    wandUse.Play();
                    SpawnManager.Instance.Spawn(attackStats.bubble, wandPos.position);
                    /*SpawnManager.Instance.GetSpawn(attackStats.bubble.name, attackStats.bubble, wandPos.position);
                    if (bubblesParent == null)
                    {
                        foreach (Transform t in SpawnManager.Instance.transform)
                        {
                            if (t.name == attackStats.bubble.name)
                            {
                                bubblesParent = t;
                            }
                        }
                    }*/
                    cd = attackStats.cd;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator Shield()
    {
        while (true)
        {
            if (GameManager.Instance.gameStates == GameStates.Play)
            {
                if (hasShield)
                {
                    shieldCD = shieldStats.duration;
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W) && GameManager.Instance.gameStates == GameStates.Play);
                    
                    GameObject shield = new GameObject();
                    switch (shieldStats.levelShield)
                    {
                        case 1:
                            shield = shieldStats.shield;
                            break;
                        case 2:
                            shield = shieldStats.shield2;
                            break;
                        case 3:
                            shield = shieldStats.shield3;
                            break;
                    }
                    hasShield = false;

                    shield.SetActive(true);
                    shieldCD = shieldStats.duration;
                    while (shieldCD > 0)
                    {
                        if (GameManager.Instance.gameStates == GameStates.Play)
                        {
                            shieldCD -= Time.deltaTime;
                        }
                        if (shieldCD < 3f && shieldCD > 1f)
                        {
                            foreach (Transform child in shield.transform)
                            {
                                child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, child.GetComponent<SpriteRenderer>().color.a == 1 ? 0.25f : 1);
                            }
                        }
                        else if (shieldCD < 1f)
                        {
                            foreach (Transform child in shield.transform)
                            {
                                child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, shieldCD);
                            }
                        }
                        yield return null;
                    }
                    shieldCD = 0;
                    //yield return new WaitForSeconds(shieldStats.duration);
                    shield.SetActive(false);
                    foreach (Transform child in shield.transform)
                    {
                        child.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                }
            }
            yield return null;
        }
    }
    IEnumerator Daze()
    {
        //Level 1 = Slows them down a little
        //Level 2 = Stops them for a while
        //Level 3 = Stops them for a while and minus hp
        while (true)
        {
            if (GameManager.Instance.gameStates == GameStates.Play)
            {
                if (hasDaze)
                {
                    dazeCD = (dazeStats.duration / 2f);
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) && GameManager.Instance.gameStates == GameStates.Play);
                    hasDaze = false;
                    StartCoroutine(FadeImage(true, dazeEffect, 0.5f));
                    Threat[] enemies = FindObjectsOfType(typeof(Threat)) as Threat[];

                    if (dazeStats.levelDaze >= 2)
                    {

                        dazeCD_two = dazeStats.duration / 2f;
                        dazeCD = dazeStats.duration / 2f;
                        while (dazeCD >= 0)
                        {
                            if (GameManager.Instance.gameStates == GameStates.Play)
                            {
                                dazeCD -= Time.deltaTime;

                                enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                                foreach (Threat obj in enemies)
                                {
                                    if (obj.enemy != null)
                                    {
                                        if (!obj.dazed)
                                        {
                                            obj.speedType = SpeedType.Stun;
                                            if (dazeStats.levelDaze >= 3)
                                            {
                                                obj.mainStats.health -= 2;
                                            }
                                            obj.dazed = true;
                                        }
                                    }
                                }
                            }
                            yield return null;
                        }
                        dazeCD = 0;
                        //yield return new WaitForSeconds(dazeStats.duration/2);
                    }
                    foreach (Threat obj in enemies)
                    {
                        if (obj.enemy != null)
                        {
                            obj.dazed = false;
                        }
                    }
                    if (dazeStats.levelDaze < 2)
                    {
                        dazeCD = dazeStats.duration/2f;
                        while (dazeCD >= 0)
                        {
                            if (GameManager.Instance.gameStates == GameStates.Play)
                            {
                                dazeCD -= Time.deltaTime;
                                
                                enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                                foreach (Threat obj in enemies)
                                {
                                    if (obj.enemy != null)
                                    {
                                        if (!obj.dazed)
                                        {
                                            switch (obj.speedType)
                                            {
                                                case SpeedType.Low:
                                                    obj.speedType = SpeedType.Slow;
                                                    break;
                                                case SpeedType.Mid:
                                                    obj.speedType = SpeedType.Low;
                                                    break;
                                                case SpeedType.Fast:
                                                    obj.speedType = SpeedType.Mid;
                                                    break;
                                            }
                                            obj.dazed = true;
                                        }
                                    }
                                }

                            }
                            yield return null;
                        }
                        dazeCD = 0;
                    }
                    else
                    {
                        dazeCD = 0;
                        dazeCD_two = dazeStats.duration / 2f;
                        while (dazeCD_two >= 0)
                        {
                            if (GameManager.Instance.gameStates == GameStates.Play)
                            {
                                dazeCD_two -= Time.deltaTime;

                                enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                                foreach (Threat obj in enemies)
                                {
                                    if (obj.enemy != null)
                                    {
                                        if (!obj.dazed)
                                        {
                                            switch (obj.speedType)
                                            {
                                                case SpeedType.Low:
                                                    obj.speedType = SpeedType.Slow;
                                                    break;
                                                case SpeedType.Mid:
                                                    obj.speedType = SpeedType.Low;
                                                    break;
                                                case SpeedType.Fast:
                                                    obj.speedType = SpeedType.Mid;
                                                    break;
                                            }
                                            obj.dazed = true;
                                        }
                                    }
                                }

                            }
                            yield return null;
                        }
                        dazeCD_two = 0;
                    }
                    //yield return new WaitForSeconds(dazeStats.duration);
                    enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                    foreach (Threat obj in enemies)
                    {
                        if (obj.enemy != null)
                        {
                            obj.speedType = obj.enemy.speedType;
                            obj.dazed = false;
                        }
                    }

                }
            }
            yield return null;
        }
    }
    IEnumerator Boost()
    {
        while (true)
        {
            if (GameManager.Instance.gameStates == GameStates.Play)
            {
                if (hasBoost)
                {
                    boostCd = boostStats.duration * boostStats.levelBoost;
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R) && GameManager.Instance.gameStates == GameStates.Play);
                    hasBoost = false;
                    //Threat[] enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                    Threat[] enemies = new Threat[0];
                    PowerUpGet[] powerUps = new PowerUpGet[0];
                    //logic
                    GameManager.Instance.timeMultiplier = boostStats.speedMultiplier;
                    GameManager.Instance.player.conditions.invincible = true;

                    boostCd = boostStats.duration * boostStats.levelBoost;
                    while (boostCd > 0f)
                    {
                        if (GameManager.Instance.gameStates == GameStates.Play)
                        {
                            enemies = FindObjectsOfType(typeof(Threat)) as Threat[];
                            powerUps = FindObjectsOfType(typeof(PowerUpGet)) as PowerUpGet[];
                            foreach (Threat enemy in enemies)
                            {
                                if (GameManager.Instance.gameStates == GameStates.Play)
                                    enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y - (.05f * boostStats.speedMultiplier), enemy.transform.position.z);
                            }
                            foreach (PowerUpGet powerUp in powerUps)
                            {
                                if (GameManager.Instance.gameStates == GameStates.Play)
                                    powerUp.transform.position = new Vector3(powerUp.transform.position.x, powerUp.transform.position.y - (.05f * boostStats.speedMultiplier), powerUp.transform.position.z);
                            }
                            GameManager.Instance.bubble.GetComponent<SpriteRenderer>().color = new Color(Random.Range(.25f, .75f), Random.Range(.25f, .75f), Random.Range(.25f, .75f));

                            boostCd -= Time.deltaTime;
                        }
                        yield return null;
                    }
                    GameManager.Instance.bubble.GetComponent<SpriteRenderer>().color = Color.white;
                    boostCd = 0;
                    //end logic
                    GameManager.Instance.timeMultiplier = 1;
                    GameManager.Instance.player.conditions.invincible = false;

                    foreach (Threat enemy in enemies)
                    {
                        //https://discussions.unity.com/t/check-if-gameobject-in-visible-on-screen/635654
                        Vector3 screenPoint = Camera.main.WorldToViewportPoint(enemy.transform.position);
                        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                        if (!(onScreen && enemy.GetComponent<Renderer>().isVisible))
                        {
                            enemy.mainStats.health = 0;
                        }
                        else
                        {
                            Debug.Log("Enemy is still on screen");
                        }
                    }
                }
            }
            yield return null;
        }
    }
    public void LevelUpAttack()
    {
        
    }
    public void LevelUpShield()
    {
        hasShield = true;
        shieldStats.levelShield++;
        PlayerPrefs.SetInt("Shield Level", shieldStats.levelShield);
        GameManager.Instance.upgrades.Find(upgrade => upgrade.upgradeName == "Shield").Maxed("Shield" + " Level");
    }
    public void LevelUpDaze()
    {
        hasDaze = true;
        dazeStats.levelDaze++;
        dazeCD = (dazeStats.duration / 2f);
        PlayerPrefs.SetInt("Daze Level", dazeStats.levelDaze);
        GameManager.Instance.upgrades.Find(upgrade => upgrade.upgradeName == "Daze").Maxed("Daze" + " Level");
    }
    public void LevelUpBoost()
    {
        hasBoost = true;
        boostStats.levelBoost++;
        boostCd = boostStats.duration * boostStats.levelBoost;
        PlayerPrefs.SetInt("Boost Level", dazeStats.levelDaze);
        GameManager.Instance.upgrades.Find(upgrade => upgrade.upgradeName == "Boost").Maxed("Boost" + " Level");
    }
    public void LevelUpHp()
    {
        GameManager.Instance.player.stats.health++;
        maxHp++;
        PlayerPrefs.SetInt("Health Level", (maxHp - 2));
        Debug.Log(PlayerPrefs.GetInt("Health Level"));
        GameManager.Instance.upgrades.Find(upgrade => upgrade.upgradeName == "Health").Maxed("Health" + " Level");
    }


    public IEnumerator FadeImage(bool fadeAway, Image img, float duration)
    {
        for (float i = 0; i <= duration; i += Time.deltaTime)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, fadeAway ? (duration - i) / duration : i / duration);
            yield return null;
        }
    }
}
[System.Serializable]
public class AttackSkill
{
    public int damage;
    public int cd;
    public GameObject bubble;
}
[System.Serializable]
public class ShieldSkill 
{
    public int duration;
    public GameObject shield;
    public GameObject shield2;
    public GameObject shield3;
   public int levelShield = 1;
}
[System.Serializable]
public class DazeSkill
{
    public int duration;
    public int levelDaze = 1;
}

[System.Serializable]
public class BoostSkill
{
    public int speedMultiplier;
    public int duration;
    public int levelBoost=1;
}
