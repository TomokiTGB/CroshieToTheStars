using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { Shield, Daze, Boost, Hp }
public class PowerUpGet : MonoBehaviour
{
    public PowerUpType powerUpType;

    #region Start, Update, etc...
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameStates == GameStates.Play)
        {
            transform.position += Vector3.up * Time.deltaTime * 9f;
        }
    }
    #endregion
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
