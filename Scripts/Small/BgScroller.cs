using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BgScroller : MonoBehaviour
{
    [Header("Image")]
    public SpriteRenderer bg_img;
    public Sprite invis;
    public List<Backgrouds> backgrouds;
    public List<SpriteRenderer> bg_separation;
    [Header("Music")]
    public List<AudioSource> bgMs;
    [Header("Furniture")]
    public Transform parentOfRenderers;
    public List<Furnitures> furnituresImages;


    //debug stuff
    public bool transitionHappened;
    public bool nextLevel;
    float newPosition;
    float prevPosition;
    bool posReset;
    //int posResetCount;
    int distanceOfTransition;
    // Start is called before the first frame update
    void Start()
    {
        bgMs[0].Play();
        bgMs[0].Stop();
        bgMs[1].Play();
        bgMs[1].Stop();
        bgMs[2].Play();
        bgMs[2].Stop();
        bgMs[3].Play();
        bgMs[3].Stop();
        bgMs[GameManager.Instance.level - 1].Play();
        foreach (Transform child in parentOfRenderers)
        {
            StartCoroutine(Hover(child.gameObject));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(bg_separation[1].transform.position);
        //the furnitures are currently a child of bubble so their scale is changing up too so i have this to cancel it
        ControlFurnitureScale();
        // scrolls the bg img (looping so it resets) 
        newPosition = GameManager.Instance.playTime.sec_affected * 9.99f % 200;   //position
                                                                            //newPosition = Mathf.Repeat(Time.time * 10f, 200);   //position
        posReset = prevPosition > newPosition;  //bool if it resets
        bg_img.transform.localPosition = new Vector3(bg_img.transform.position.x, -newPosition, bg_img.transform.position.z);    //sets the position(might need to put this below the if(posReset))
                                                                                                                            //scrolls down the images once 

        // transition first before the image can reset
        if (GameManager.Instance.levelChanged)
        {
            Transition();
        }
        //Debug.Log(Mathf.FloorToInt(bg_img.transform.position.y));
        if (transitionHappened)
        {
            //i think it's this logic that is the problem (for when i add an initial pos or parent to offset it) in level 3
            if (bg_img.transform.position.y < -156)
            {
                if (bg_separation[2].sprite != invis && bg_separation[3].sprite == invis)
                {
                    GameManager.Instance.levelVisually = GameManager.Instance.level;
                    bg_img.sprite = backgrouds[GameManager.Instance.level - 1].loop;
                    StartCoroutine(SwitchAudio(bgMs[GameManager.Instance.levelVisually - 2], bgMs[GameManager.Instance.levelVisually - 1]));

                    transitionHappened = false;
                    nextLevel = true;
                }
            }
        }
        if (posReset)
        {
            BgGoDown();
        }

        if (nextLevel)
        {
            nextLevel = false;
            GameManager.Instance.upgradeCalled = false;
            GameManager.Instance.gameStates = GameStates.Upgrade;
        }
    }
    private void LateUpdate()
    {
        prevPosition = newPosition;
    }
    void Transition()
    {
        #region Background Images
        if (bg_img.transform.position.y > -44)
        {
            SetTransitionImage(1);
        }
        else
        {
            SetTransitionImage(0);
        }
        #endregion


        //furnitures
        int x = 0;
        foreach (Transform child in parentOfRenderers)
        {
            SpriteRenderer sR = child.GetComponent<SpriteRenderer>();
            if (sR.color == Color.clear)
            {
                if (furnituresImages[GameManager.Instance.level - 1].furnitures[x] != null)
                {
                    sR.sprite = furnituresImages[GameManager.Instance.level - 1].furnitures[x];
                    sR.color = Color.white;
                    x++;
                }
            }
        }
        /*for (int i = 0; i < furnituresRenderer.Count; i++)
        {
            SpriteRenderer sR = furnituresRenderer[i];
            if (furnituresImages[GameManager.Instance.level - 1].furnitures[i] != null)
            {
                sR.sprite = furnituresImages[GameManager.Instance.level - 1].furnitures[i];
                sR.color = Color.white;
            }
            else
            {
                sR.sprite = null;
                sR.color = Color.clear;
            }
        }*/

        transitionHappened = true;
    }

    private void SetTransitionImage(int startingIndex)
    {
        //both transition and start are present
        if (backgrouds[GameManager.Instance.level - 2].transistion != null && backgrouds[GameManager.Instance.level - 1].start != null)
        {
            bg_separation[startingIndex+1].sprite = backgrouds[GameManager.Instance.level - 2].transistion;
            bg_separation[startingIndex].sprite = backgrouds[GameManager.Instance.level - 1].start;
        }
        //only transition
        if (backgrouds[GameManager.Instance.level - 2].transistion != null && backgrouds[GameManager.Instance.level - 1].start == null)
        {
            bg_separation[startingIndex+1].sprite = backgrouds[GameManager.Instance.level - 2].transistion;
        }
    }

    void BgGoDown()
    {
        for (int i = bg_separation.Count - 1; i >= 0; i--)
        {
            if (i != 0)
            {
                bg_separation[i].sprite = bg_separation[i - 1].sprite;
            }
            else
            {
                bg_separation[i].sprite = invis;
            }
        }
    }

    public IEnumerator FadeAudio(AudioSource audioSource, float FadeTime, bool fadeIn)
    {
        if (fadeIn)
        {
            while (audioSource.volume < 0.8f)
            {
                audioSource.volume += Time.deltaTime / FadeTime;
                yield return null;
            }
            audioSource.volume = 0.8f;
        }
        else
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime / FadeTime;

                yield return null;
            }
            audioSource.Stop();
        }

    }
    IEnumerator SwitchAudio(AudioSource fadeOut, AudioSource fadeIn)
    {
        Coroutine coroutine = StartCoroutine(FadeAudio(fadeOut, 3f, false));
        yield return coroutine;
        /*bgm.clip = audioSources[GameManager.Instance.level - 1];
        bgm.Play();*/
        fadeIn.volume = 0;
        fadeIn.Play();
        StartCoroutine(FadeAudio(fadeIn, 3f, true));
    }

    void ControlFurnitureScale()
    {
        foreach (Transform go in parentOfRenderers)
        {
            go.transform.localScale = Vector3.one / GameManager.Instance.bubble.transform.localScale.x;
        }
    }

    IEnumerator Hover(GameObject @object)
    {
        Vector3 ogPos = @object.transform.localPosition;
        Vector3 targetPos;

        float speed = Random.Range(0.1f, 0.35f);

        float randomValue = Random.Range(0.1f, 5f);
        yield return new WaitForSeconds(randomValue);
        while (true)
        {
            targetPos = ogPos + (Vector3.up);
            while (Mathf.Abs(@object.transform.localPosition.y - targetPos.y) > 0.1f)
            {
                @object.transform.localPosition = Vector3.MoveTowards(@object.transform.localPosition, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            //@object.transform.position = targetPos;

            randomValue = Random.Range(0.1f, .75f);
            yield return new WaitForSeconds(randomValue);

            targetPos = ogPos;
            while (Mathf.Abs(@object.transform.localPosition.y - targetPos.y) > 0.1f)
            {
                @object.transform.localPosition = Vector3.MoveTowards(@object.transform.localPosition, targetPos, speed * Time.deltaTime);
                yield return null;
            }
            //@object.transform.position = targetPos;

            randomValue = Random.Range(0.1f, .75f);
            yield return new WaitForSeconds(randomValue);

        }
    }
}
[System.Serializable]
public class Backgrouds
{
    public int levelNo;
    public Sprite start;
    public Sprite loop;
    public Sprite transistion;
}
[System.Serializable]
public class Furnitures
{
    public int levelNo;
    public List<Sprite> furnitures;
}