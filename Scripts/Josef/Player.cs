using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MainStats stats;
    public ConditionStats conditions;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Threat>() != null)
        {
            Threat threat = other.GetComponent<Threat>();
            if (!conditions.invincible)
            {
                stats.health -= 1;
                BubbleShake();


            }
            threat.mainStats.health=0;
            if (stats.health <= 0)
            {
                GameManager.Instance.gameStates = GameStates.End;
                GameManager.Instance.popSound.Play();
            }
        }
    }
    void BubbleShake()
    {
        StartCoroutine(Shake());
    }
    IEnumerator Shake()
    {
        conditions.invincible = true;
        Vector3 initialPos = transform.position;
        float distance = .3f;
        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.left * distance;
            yield return new WaitForSeconds(0.01f);
            transform.position = Vector3.right * distance;
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = initialPos;
        conditions.invincible = false;
    }
}
