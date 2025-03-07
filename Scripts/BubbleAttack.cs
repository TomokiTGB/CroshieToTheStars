using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAttack : MonoBehaviour
{
    SkillManager sM;
    public Vector3 targetDir;

    float timer;

    Vector3 direction;
    void OnEnable()
    {
        timer = 0;
        gameObject.name = "Bubble";
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(.25f, .75f), Random.Range(.25f, .75f), Random.Range(.25f, .75f));
        targetDir = GameManager.Instance.MousePosition() - SkillManager.Instance.wandPos.position;
        transform.localScale = (Vector3.one * GameManager.Instance.bubble.transform.localScale.x) * 0.075f;
        float angle = GameManager.Instance.AngleOfTwo(GameManager.Instance.MousePosition(), SkillManager.Instance.wandPos.position);
        float radianAngle = angle * Mathf.Deg2Rad;  // Convert degrees to radians
        direction = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, targetDir, 20 * Time.deltaTime);
        if (SkillManager.Instance.bubbleDistanceDependent)
        {
            transform.position += targetDir * 2 * Time.deltaTime;
        }
        else
        {
            transform.position += direction * 20 * Time.deltaTime;
        }

        timer += Time.deltaTime;
        if (timer > 10)
        {
            gameObject.SetActive(false);
            //GameManager.Instance.popSound.Play();
        }
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Threat>() != null)
        {
            SpawnManager.Instance.PutAfterTheInactives(gameObject);
            gameObject.SetActive(false);
            Threat threat = other.gameObject.GetComponent<Threat>();
            threat.mainStats.health -= SkillManager.Instance.attackStats.damage;
            GameManager.Instance.popSound.Play();
        }
        else if (other.gameObject.GetComponent<PowerUpGet>() != null)
        {
            //Debug.Log("PowerUpHit");
            PowerUpGet powerUp = other.gameObject.GetComponent<PowerUpGet>();
            switch (powerUp.powerUpType)
            {
                case PowerUpType.Shield:
                    SkillManager.Instance.hasShield = true;
                    break;
                case PowerUpType.Daze:
                    SkillManager.Instance.hasDaze = true;
                    break;
                case PowerUpType.Boost:
                    SkillManager.Instance.hasBoost = true;
                    break;
                case PowerUpType.Hp:
                    if (GameManager.Instance.player.stats.health < SkillManager.Instance.maxHp)
                    {
                        GameManager.Instance.player.stats.health++;
                    }
                    break;
            }
            GameManager.Instance.powerUpGetSound .Play();
            other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
