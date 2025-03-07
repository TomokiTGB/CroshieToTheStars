using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    //[SerializeField] private float speed = 5f;
    private float smoothTime = 0.25f;
    public float speed = 5;
    private Vector3 velocity = Vector3.zero;
    public List<GameObject> aimAtMouseObjects;
    public GameObject eye;
    //Vector3 movement;
    float distance;
    //[SerializeField] private GameObject follow;
    public Transform rabbitArm;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameStates == GameStates.Play)
        {
            PlayerMove();
            HandleAim();
            EyeFollowMouse();
        }
        else if (GameManager.Instance.gameStates == GameStates.End)
        {
            rb.mass = 100;
            rb.useGravity = true;
            GameManager.Instance.bubble.SetActive(false);
        }
    }
    void PlayerMove()
    {
        transform.position = Vector3.SmoothDamp(transform.position, GameManager.Instance.MousePosition() * speed, ref velocity, smoothTime);
        transform.position = Vector3.ClampMagnitude(transform.position, 4*GameManager.Instance.bubble.transform.localScale.x);
    }
    private void FixedUpdate()
    {
        //rb.velocity = new Vector3(movement.x, movement.y, 0) * speed;
    }
    void HandleAim()
    {
        foreach (GameObject go in aimAtMouseObjects)
        {
            float targetRotation = AngleOfTwo(go.transform.position, GameManager.Instance.MousePosition());
            go.transform.eulerAngles = new Vector3(0, 0, targetRotation);
        }
    }
    private void EyeFollowMouse()
    {
        Vector3 mousePosOnScreen = Camera.main.WorldToViewportPoint(GameManager.Instance.MousePosition());
        mousePosOnScreen.x = Mathf.Clamp01(mousePosOnScreen.x);
        mousePosOnScreen.y = Mathf.Clamp01(mousePosOnScreen.y);
        mousePosOnScreen.z = Mathf.Clamp01(mousePosOnScreen.z);
        float xPos = Mathf.Lerp(-0.25f, 0.25f, mousePosOnScreen.x);
        float yPos = Mathf.Lerp(-0.25f, 0.25f, mousePosOnScreen.y);
        eye.transform.localPosition = new Vector3(xPos, yPos, 0);
    }

    /*void FollowPlayer()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.z = 0;
        distance = Vector3.Distance(cameraPos, transform.position);
        //Debug.Log(distance);
        *//*if (distance > 6)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, transform.position, ref velocity, smoothTime);
        }*//*
    }*/
    private float AngleOfTwo(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    
}