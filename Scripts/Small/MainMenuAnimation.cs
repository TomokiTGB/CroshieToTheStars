using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuAnimation : MonoBehaviour
{
    public bool test;
    public bool won;
    [Header("Starting BG Images")]
    public float fps1;
    public List<Sprite> fallingStart;
    public float fps2;
    public List<Sprite> fallingLoop;
    [Header("Win BG Image")]
    public Sprite win;
    [Header("Title Images")]
    public List<Sprite> titleImgs;
    
    //positions
    [Header("BG")]
    public Image img_anim;
    public Vector3 bg_StartPos;
    public Vector3 bg_WinPos;
    [Header("Title")]
    public Image img_Title;
    public Vector3 title_StartPos;
    public Vector3 title_WinPos;
    [Header("Buttons")]
    public GameObject mainMenuButtons;
    public Vector3 btn_StartPos;
    public Vector3 btn_WinPos;
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (test)
            {
                test = false;
                if (won)
                {
                    img_anim.sprite = win;
                    SetPos(bg_WinPos, title_WinPos, 1, btn_WinPos, 1.5f);
                }
                else
                {
                    img_anim.sprite = fallingLoop[1];
                    SetPos(bg_StartPos, title_StartPos, 0, btn_StartPos, 1f);
                }
            }
        }
    }
    private void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("WinGames"))
        {
            if (PlayerPrefs.GetInt("WinGames") > 0)
            {
                SetMainMenu(true);
            }
            else
            {
                SetMainMenu(false);
            }
        }
        else
        {
            SetMainMenu(false);
        }
    }

    void SetMainMenu(bool wonGame)
    {
        if (wonGame)
        {
            img_anim.sprite = win;
            SetPos(bg_WinPos, title_WinPos, 1, btn_WinPos, 1.5f);
        }
        else
        {
            StartCoroutine(Animation());
            SetPos(bg_StartPos, title_StartPos, 0, btn_StartPos, 1f);
        }
    }

    void SetPos(Vector3 bgPos, Vector3 titlePos, int titleIndex, Vector3 btn_pos, float btnScale)
    {
        img_anim.transform.localPosition = bgPos;

        img_Title.GetComponent<RectTransform>().localPosition = titlePos;
        img_Title.sprite = titleImgs[titleIndex];

        mainMenuButtons.GetComponent<RectTransform>().localPosition = btn_pos;
        mainMenuButtons.GetComponent<RectTransform>().localScale = Vector3.one * btnScale;
    }
    IEnumerator Animation()
    {
        foreach (Sprite sprite in fallingStart)
        {
            img_anim.sprite = sprite;
            yield return new WaitForSeconds(1 / fps1);
        }
        while (true)
        {
            foreach (Sprite sprite in fallingLoop)
            {
                img_anim.sprite = sprite;
                yield return new WaitForSeconds(1 / fps2);
            }
        }
    }
}
