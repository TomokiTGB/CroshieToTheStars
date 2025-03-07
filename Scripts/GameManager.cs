using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GameStates { Play, Upgrade, Pause, End, Win}
public enum GameLength { Short, Normal, Long}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStates gameStates;
    public GameStates beforePauseGameState;
    public GameLength gameLength;
    public bool gameLengthSwitch;
    public List<GameObject> uIList;
    public List<Image> indicators;
    public Timer playTime;
    public bool timerRunning;
    public int kills;
    public int score=0;
    public Player player;
    public int level;
    public int levelVisually;
    private int prevLevel;
    private int prevLevelVisually;
    [HideInInspector] public bool levelChanged { get; private set; }
    [HideInInspector] public bool levelVisuallyChanged { get; private set; }
    public List<Level> Stages;

    [Header("Upgrades")]
    public List<Button> upgradeButtons;
    public List<Upgrades> upgrades;
    [TextArea(4,7)]
    public List<string> quotes;
    [Header("Extras")]
    public BgScroller bgScroller;
    public GameObject pauseButton;
    public GameObject bg;
    public GameObject bubble;
    public float maxScale = 5;
    public float maxCamDistance = -75;
    [SerializeField] private Vector3 mousePos;
    public Slider progress;

    public bool devMode;
    public Toggle devModeToggle;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;

    public AudioSource popSound;
    public AudioSource powerUpGetSound;

    public float timeScale;

    [HideInInspector] public bool upgradeCalled;
    [HideInInspector] public float timeMultiplier = 1;
    //checker
    int upgradesUnlocked;
    int maxUnlock;
    bool scoreChecked;
    EscapeOffOn[] escapeOffOns;
    void OnToggleValueChanged(bool isOn)
    {
        player.conditions.invincible = isOn;
    }
    private void OnValidate()
    {  
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(Instance);
        }
        escapeOffOns = Resources.FindObjectsOfTypeAll<EscapeOffOn>();

        //devModeToggle.onValueChanged.AddListener(OnToggleValueChanged);
        player = FindFirstObjectByType(typeof(Player)).GetComponent<Player>();

        prevLevel = level;
        levelVisually = level;
        prevLevelVisually = levelVisually;

#if UNITY_EDITOR
#else
        player.conditions.invincible = false;
        timeScale = 1;
#endif
    }
    private void Start()
    {
        scoreChecked = false;
        //StartCoroutine(TickInterval(playTime));
        upgradesUnlocked = 0;
        maxUnlock = (3 * 4); // 3 is the max per ability and 4 is the number of upgradables
        if (PlayerPrefs.HasKey("Shield Level"))
        {
            upgradesUnlocked += PlayerPrefs.GetInt("Shield Level");
            Debug.Log("Shield: " + PlayerPrefs.GetInt("Shield Level"));
        }
        else
        {
            upgradesUnlocked++;
        }
        if (PlayerPrefs.HasKey("Daze Level"))
        {
            upgradesUnlocked += PlayerPrefs.GetInt("Daze Level");
            Debug.Log("Daze: " + PlayerPrefs.GetInt("Daze Level"));
        }
        else
        {
            upgradesUnlocked++;
        }
        if (PlayerPrefs.HasKey("Boost Level"))
        {
            upgradesUnlocked += PlayerPrefs.GetInt("Boost Level");
            Debug.Log("Boost: " + PlayerPrefs.GetInt("Boost Level"));
        }
        else
        {
            upgradesUnlocked++;
        }
        if (PlayerPrefs.HasKey("Health Level"))
        {
            upgradesUnlocked += PlayerPrefs.GetInt("Health Level");
            Debug.Log("Health: " + PlayerPrefs.GetInt("Health Level"));
        }
        else
        {
            upgradesUnlocked++;
        }

        if (gameLengthSwitch)
        {
            Debug.Log((float)upgradesUnlocked / maxUnlock);
            if (((float)upgradesUnlocked / maxUnlock) < (2f / 3f))
            {
                gameLength = GameLength.Short;
            }
            else if (((float)upgradesUnlocked / maxUnlock) < (1f))
            {
                gameLength = GameLength.Normal;
            }
            else if (((float)upgradesUnlocked / maxUnlock) == 1f)
            {
                gameLength = GameLength.Long;
            }
        }

        foreach (Upgrades up in upgrades)
        {
            up.Maxed(up.upgradeName + " Level");
        }
    }
    // Update is called once per frame
    void Update()
    {
        CurrentState();
        //devMode = devModeToggle.isOn;

        pauseButton.GetComponent<Image>().raycastTarget = gameStates.Equals(GameStates.Play) || gameStates.Equals(GameStates.Upgrade);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStates == GameStates.Pause)
            {
                foreach (EscapeOffOn esc in escapeOffOns)
                {
                    if (esc.gameObject.activeSelf)
                    {
                        esc.NextUI();
                        return;
                    }
                }
                gameStates = beforePauseGameState;
            }
            else
            {
                beforePauseGameState = gameStates;
                gameStates = GameStates.Pause;
            }
        }

        /*if (levelChanged && level >= 4)
            gameStates = GameStates.Upgrade;*/

        if (devMode)
        {
            SkillManager.Instance.hasShield = devMode;
            SkillManager.Instance.hasDaze = devMode;
            SkillManager.Instance.hasBoost = devMode;
            player.conditions.invincible = devMode;
        }
    }

    private void LateUpdate()
    {
        levelChanged = prevLevel != level;
        levelVisuallyChanged = levelVisually != prevLevelVisually;
        prevLevel = level;
        prevLevelVisually = levelVisually;
    }

    void CurrentState() 
    {
        switch (gameStates)
        {
            case GameStates.Play:
                StateLogic("PlayPanel", true);
                //Time.timeScale = 1;
                Time.timeScale = timeScale;
                #region Time Stuff
                playTime.sec_affected += Time.deltaTime * timeMultiplier;

                playTime.sec_unaffected += Time.deltaTime;
                playTime.min = (int)playTime.sec_unaffected / 60;
                int second = (int)playTime.sec_unaffected % 60;

                playTime.text = string.Format("Time: {0:00}:{1:00}", playTime.min, second);
                timeText.text = playTime.text;
                #endregion
                #region Scale Stuff
                switch (gameLength)
                {
                    case GameLength.Short:
                        level = 1 + ((int)playTime.sec_affected / 60) / 1;
                        break;
                    case GameLength.Normal:
                        level = 1 + ((int)playTime.sec_affected / 60) / 2;
                        break;
                    case GameLength.Long:
                        level = 1 + ((int)playTime.sec_affected / 60) / 3;
                        break;
                }
                float targetScale = 0;
                switch (gameLength)
                {
                    case GameLength.Short:
                        targetScale = (maxScale - 1) * (playTime.sec_unaffected / (60f * 3f));
                        break;
                    case GameLength.Normal:
                        targetScale = (maxScale - 1) * (playTime.sec_unaffected / (60f * 6f));
                        break;
                    case GameLength.Long:
                        targetScale = (maxScale - 1) * (playTime.sec_unaffected / (60f * 9f));
                        break;
                }
                if (bubble.transform.localScale.x < maxScale)
                {
                    bubble.transform.localScale = new Vector3(targetScale + 1, targetScale + 1, targetScale + 1);
                }
                float targetZpos = 0;
                switch (gameLength)
                {
                    case GameLength.Short:
                        targetZpos = (30 + maxCamDistance) * (playTime.sec_unaffected / (60f * 2.5f));
                        break;
                    case GameLength.Normal:
                        targetZpos = (30 + maxCamDistance) * (playTime.sec_unaffected / (60f * 3.75f));
                        break;
                    case GameLength.Long:
                        targetZpos = (30 + maxCamDistance) * (playTime.sec_unaffected / (60f * 4f));
                        break;
                }
                if (Camera.main.transform.position.z > maxCamDistance)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -30 + targetZpos);

                    //background location
                    /*Vector2 area1 = new Vector2(-35, 70);
                    Vector2 area2 = new Vector2(67, 22);
                    float z = TwoPosition(targetZpos-30, area1, area2);
                    bg.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, z);*/ // this code is for readability. it does the same code as the one line of code below
                    bg.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, TwoPosition(targetZpos - 30, new Vector2(-30, -70), new Vector2(67, 22)));
                }
                switch (gameLength)
                {
                    case GameLength.Short:
                        progress.value = playTime.sec_affected / (60f * 4f);
                        break;
                    case GameLength.Normal:
                        progress.value = playTime.sec_affected / (60f * 8f);
                        break;
                    case GameLength.Long:
                        progress.value = playTime.sec_affected / (60f * 12f);
                        break;
                }
                #endregion
                #region UI Fill Stuff
                indicators[0].fillAmount = (player.stats.health / SkillManager.Instance.maxHp);
                indicators[1].fillAmount = SkillManager.Instance.shieldCD / SkillManager.Instance.shieldStats.duration;
                indicators[2].fillAmount = SkillManager.Instance.dazeCD / (SkillManager.Instance.dazeStats.duration / 2f);
                indicators[4].fillAmount = SkillManager.Instance.dazeCD_two / (SkillManager.Instance.dazeStats.duration / 2f);
                indicators[3].fillAmount = SkillManager.Instance.boostCd / (SkillManager.Instance.boostStats.duration * SkillManager.Instance.boostStats.levelBoost);
                #endregion
                if (level > 4)
                {
                    gameStates = GameStates.Win;
                }
                break;
            case GameStates.Pause:
                StateLogic("PausePanel", false);
                Time.timeScale = 0;
                break;
            case GameStates.End:
                StateLogic("EndPanel", false);
                Time.timeScale = 1;
                indicators[0].fillAmount = (player.stats.health / SkillManager.Instance.maxHp);
                if (!scoreChecked)
                    CheckScore(scoreText1);
                break;
            case GameStates.Upgrade:
                if (upgradesUnlocked != maxUnlock)
                {
                    StateLogic("UpgradesPanel", false);
                }
                Time.timeScale = 0;
                if (!upgradeCalled)
                {
                    upgradeCalled = true;
                    if (PlayerPrefs.HasKey("WinGames"))
                    {
                        if (PlayerPrefs.GetInt("WinGames") > 0)
                        {
                            uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(1).gameObject.SetActive(false);
                            uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(2).gameObject.SetActive(true);
                        }
                        else
                        {
                            uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(1).gameObject.SetActive(true);
                            uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quotes[levelVisually - 2];
                            uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(2).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(1).gameObject.SetActive(true);
                        uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quotes[levelVisually - 2];
                        uIList.Find(ui => ui.name == "UpgradesPanel").transform.GetChild(2).gameObject.SetActive(false);
                    }
                    //turns each buttons off
                    foreach (Button upgrade in upgradeButtons)
                    {
                        upgrade.gameObject.SetActive(false);
                    }
                    //makes it so it is not selected yet
                    foreach (Upgrades up in upgrades)
                    {
                        up.selected = false;
                    }
                    //apply
                    foreach (Button upgrade in upgradeButtons)
                    {
                        int assign = Random.Range(0, upgrades.Count);
                        bool assigned = true;
                        for (int i = 0; i < 10; i++)
                        {
                            assigned = false;
                            if (upgrades[assign].selected || upgrades[assign].maxedOut)
                            {
                                assign = Random.Range(0, upgrades.Count);
                            }
                            else
                            {
                                if (!upgrades[assign].selected && !upgrades[assign].maxedOut)
                                {
                                    assigned = true;
                                    break;
                                }
                            }
                        }
                        if (assigned)
                        {
                            upgrade.gameObject.SetActive(true);
                            upgrades[assign].selected = true;

                            upgrade.GetComponent<Image>().sprite = upgrades[assign].upgradeSprite;

                            int upgradeLevel = 0;
                            switch (upgrades[assign].upgradeName)
                            {
                                case "Shield":
                                    upgradeLevel = SkillManager.Instance.shieldStats.levelShield - 1;
                                    break;
                                case "Daze":
                                    upgradeLevel = SkillManager.Instance.dazeStats.levelDaze - 1;
                                    break;
                                case "Boost":
                                    upgradeLevel = SkillManager.Instance.boostStats.levelBoost - 1;
                                    break;
                                case "Health":
                                    upgradeLevel = SkillManager.Instance.maxHp - 3;
                                    break;
                            }
                            upgrade.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgrades[assign].upgradeStages[upgradeLevel].upgradeDescription;
                            upgrade.GetComponent<Buttons>().@event.RemoveAllListeners();
                            upgrade.GetComponent<Buttons>().@event.AddListener(() => upgrades[assign].@event.Invoke());
                        }
                        else
                        {
                            upgrade.gameObject.SetActive(false);
                        }
                    }
                    //check if all the buttons are off
                    int inactiveCount = 0;
                    foreach (Button upgrade in upgradeButtons)
                    {
                        if (!upgrade.gameObject.activeSelf)
                        {
                            inactiveCount++;
                        }
                    }
                    if (inactiveCount == upgradeButtons.Count)
                    {
                        //Debug.Log(inactiveCount + " : " + upgradeButtons.Count);
                        gameStates = GameStates.Play;
                    }
                }
                break;
            case GameStates.Win:
                StateLogic("WinPanel", false);
                Time.timeScale = 1;
                PlayerPrefs.SetInt("WinGames", 1);
                if (!scoreChecked)
                    CheckScore(scoreText2);
                break;
        }
    }

    private static float TwoPosition(float objectToCompare, Vector2 objectLimit, Vector2 newLimit)
    {
        float normalizedA = Mathf.InverseLerp(objectLimit.x, objectLimit.y, objectToCompare);
        float normalizedB = Mathf.Lerp(newLimit.x, newLimit.y, normalizedA);
        return normalizedB;
    }

    void StateLogic(string ui, bool time) 
    {
        FindObjectList(uIList, ui);
        timerRunning = time;
    }

    void FindObjectList(List<GameObject> list,string objName)
    {
        bool objFound=false;
        foreach (GameObject obj in list)
        {
            if (obj.name == objName)
            {
                obj.SetActive(true);
                objFound = true;
            }
            else 
            {
                obj.SetActive(false);
            }
            
        }
        if (objFound == false) 
        {
            //Debug.Log($"Object {objName}: Is missing in {list.ToString()}");
        }
    }

    IEnumerator TickInterval(Timer time) 
    {
        while (true)
        {
            while (timerRunning)
            {
                //Debug.Log("test");
                time.sec_affected += 1;
                time.min = (int)time.sec_affected / 60;
                int second = (int)time.sec_affected % 60;

                time.text = string.Format("Time: {0,00}:{1:00}", time.min, second);
                yield return new WaitForSeconds(1);
            }
            yield return null;
        }
    }

    void CheckScore(TextMeshProUGUI scoreText) 
    {
        score = ((int)playTime.sec_unaffected + (playTime.min * 60))*10;
        score = score + (kills*50);
        scoreText.text = "Score: " + score.ToString() + "\nEnemies: " + kills;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
                scoreText.text += "\nNew High Score";
            }
            else
            {
                scoreText.text += "\nHigh Score: " + PlayerPrefs.GetInt("HighScore");
            }
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", score);
            scoreText.text += "\nNew High Score";
        }
    }


    public void Pause()
    {
        beforePauseGameState = gameStates;
        gameStates = GameStates.Pause;
    }
    public void UnPause()
    {
        gameStates = beforePauseGameState;
    }

    //mouse
    public Vector3 MousePosition()
    {
        mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.localPosition.z;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos;
    }
    public float AngleOfTwo(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}


[System.Serializable]
public class Timer 
{
    public float sec_affected;
    [Space]
    public float sec_unaffected;
    public int min;
    public string text;
}

[System.Serializable]
public class Upgrades
{
    public string upgradeName;
    public bool selected;
    public bool maxedOut;
    public Sprite upgradeSprite;
    [Space]
    public List<UpgradeStages> upgradeStages;
    [Space]
    public UnityEvent @event;

    public void Maxed(string playerPref)
    {
        if (PlayerPrefs.HasKey(playerPref))
        {
            if (PlayerPrefs.GetInt(playerPref) == 3)
            {
                maxedOut = true;
            }
        }
    }
}
[System.Serializable]
public class UpgradeStages
{
    public int levelNo;
    [TextArea(4,7)]
    public string upgradeDescription;
}