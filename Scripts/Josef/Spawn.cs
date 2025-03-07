using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float delay=3;

    private void OnEnable()
    {
        StartCoroutine(DelayReturn());
    }

    IEnumerator DelayReturn() 
    {
        yield return new WaitForSeconds(delay);
        SpawnManager.Instance.ReturnSpawn(transform.parent.name, this.gameObject);
    }
}
