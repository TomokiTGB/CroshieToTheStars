using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public List<GameObject> objects;
    public GameObject off;
    public GameObject on;
    public int index;
    public bool toggle;
    public TextMeshProUGUI textBox;
    public AudioSource sfx;
    public GameStates gameStates;
    [TextArea(5,10)]
    public string text;
    [Space]
    public UnityEvent @event;

    public void TurnOffOn()
    {
        if (off != null)
        {
            off.SetActive(false);
        }
        if (on != null)
        {
            on.SetActive(true);
        }
        if (sfx != null)
        {
            sfx.Play();
        }
        //objects[index].SetActive(toggle);
    }
    public void CallEvent()
    {
        @event.Invoke();
    }
    public void SwitchState()
    {
        GameManager.Instance.gameStates = gameStates;
    }

    public void DisplayText()
    {
        textBox.text = text;
    }
    public void TurnOnOffAll()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(toggle);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void LoadScene()
    {
        Time.timeScale = 1.0f;
        SceneLoading.instance.LoadLevel(index);
        //SceneManager.LoadScene(index);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }


}