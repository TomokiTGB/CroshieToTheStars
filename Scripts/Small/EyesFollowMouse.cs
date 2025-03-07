using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesFollowMouse : MonoBehaviour
{
    public GameObject eye1;

    public float limit;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameStates == GameStates.Play)
        {
            Vector3 mousePosOnScreen = Camera.main.WorldToViewportPoint(GameManager.Instance.MousePosition());
            mousePosOnScreen.x = Mathf.Clamp01(mousePosOnScreen.x);
            mousePosOnScreen.y = Mathf.Clamp01(mousePosOnScreen.y);
            mousePosOnScreen.z = Mathf.Clamp01(mousePosOnScreen.z);
            EyeFollowMouse(mousePosOnScreen, eye1);
        }
    }

    private void EyeFollowMouse(Vector3 mousePosOnScreen, GameObject eye)
    {
        float xPos = Mathf.Lerp(-limit, limit, mousePosOnScreen.x);
        float yPos = Mathf.Lerp(-limit, limit, mousePosOnScreen.y);
        eye.transform.localPosition = new Vector3(xPos,yPos, 0);
    }
}
