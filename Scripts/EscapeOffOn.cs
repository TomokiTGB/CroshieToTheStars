using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeOffOn : MonoBehaviour
{
    public GameObject nextOn;
    [Header("if Self -> Leave Blank")]
    public GameObject nextOff;

    public void NextUI()
    {
        nextOn.SetActive(true);
        if (nextOff == null)
            gameObject.SetActive(false);
        else
            nextOff.SetActive(false);
    }
}
