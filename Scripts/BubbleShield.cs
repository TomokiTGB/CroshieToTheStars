using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Threat>() != null)
        {
            Threat threat = other.gameObject.GetComponent<Threat>();
            threat.mainStats.health -= SkillManager.Instance.attackStats.damage;
        }
    }
}
