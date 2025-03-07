using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private int index = 0;
    public List<Tutorial> tutorials;

    public Image image_holder;
    public TextMeshProUGUI text_holder;

    public Button btn_previous;
    public Button btn_next;

    [Header("Extra")]
    private List<string> konami = new List<string>() { "UpArrow", "UpArrow", "DownArrow", "DownArrow", "LeftArrow", "RightArrow", "LeftArrow", "RightArrow", "B", "A" };
    private List<string> speedUp = new List<string>() { "S", "P", "E", "E", "D", "U", "P"};
    private List<string> slowDown = new List<string>() { "S", "L", "O", "W", "D", "O", "W", "N"};
    private List<string> resetSpeed = new List<string>() { "R", "E", "S", "E", "T", "S", "P", "E", "E", "D"};
    public List<string> strings;
    [Header("Konami")]
    public GameObject devModeActivate;
    public TextMeshProUGUI textForKonami;
    void Start()
    {
        PlaceInUI();
    }
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (btn_next != null)
                btn_next.interactable = index != tutorials.Count - 1;
            if (btn_previous != null)
                btn_previous.interactable = !(index == 0);

            if (Input.anyKeyDown)
            {
                //https://discussions.unity.com/t/find-out-which-key-was-pressed/616242/3
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kcode))
                    {
                        strings.Add(kcode.ToString());

                        /*if (!KonamiListCheck())
                        {
                            if (strings.Count == 3 && SpecialKonamiListCheck())
                            {
                                strings = new List<string>
                                {
                                    "UpArrow", "UpArrow"
                                };
                            }
                            else
                            {
                                strings = new List<string>
                                {
                                    kcode.ToString()
                                };
                            }
                        }*/
                        break;
                    }
                }
            }

            if (CheckCheatCodes(konami))
            {
                strings = new List<string>();
                GameManager.Instance.devMode = !GameManager.Instance.devMode;
                devModeActivate.SetActive(true);
                textForKonami.text = GameManager.Instance.devMode ? "Dev Mode has been enabled" : "Dev Mode has been disabled";
                gameObject.SetActive(false);
            }
            else if (CheckCheatCodes(speedUp))
            {
                strings = new List<string>();
                if (GameManager.Instance.timeScale < 1f)
                    GameManager.Instance.timeScale = 1;
                else if (GameManager.Instance.timeScale != 10)
                    GameManager.Instance.timeScale++;
            }
            else if (CheckCheatCodes(slowDown))
            {
                strings = new List<string>();
                if (GameManager.Instance.timeScale != 1)
                    GameManager.Instance.timeScale--;
                else
                {
                    if ((GameManager.Instance.timeScale / 2f) != 0)
                        GameManager.Instance.timeScale /= 2f;
                }
            }
            else if (CheckCheatCodes(resetSpeed))
            {
                strings = new List<string>();
                GameManager.Instance.timeScale = 1;
            }
        }
    }
    string ListStringConvertToString(List<string> stringList)
    {
        string toReturn = "";
        foreach (string str in stringList)
        {
            toReturn += str;
        }
        return toReturn;
    }
    bool CheckCheatCodes(List<string> cheat)
    {
        string theCheat = ListStringConvertToString(cheat);
        string convertedStrings = ListStringConvertToString(strings);
        if (convertedStrings.Contains(theCheat))
            return true;
        return false;
    }
    bool KonamiListCheck()
    {
        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i] != konami[i])
            {
                return false;
            }
        }
        return true;
    }
    bool SpecialKonamiListCheck()
    {
        for (int i = 0; i < 3; i++)
        {
            if (strings[i] != "UpArrow")
            {
                return false;
            }
        }
        return true;
    }
    bool InTheListOfStrings(List<string> sequence1, List<string> sequence2)
    {
        for (int i = 0; i <= sequence1.Count - sequence2.Count; i++)
        {
            if (sequence1.Skip(i).Take(sequence2.Count).SequenceEqual(sequence2))
            {
                return true;
            }
        }
        return false;
    }
    public void Next()
    {
        index++;
        PlaceInUI();
    }
    public void Previous()
    {
        index--;
        PlaceInUI();
    }
    void PlaceInUI()
    {
        if (image_holder!=null)
            image_holder.sprite = tutorials[index].image;
        text_holder.text = tutorials[index].description;
    }
}
[System.Serializable]
public class Tutorial
{
    public Sprite image;
    [TextArea(3,5)]
    public string description;
}