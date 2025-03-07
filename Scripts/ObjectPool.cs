using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject empty;
    public GameObject TestSpawn;

    public bool spawn;
    public bool despawn;
    // Start is called before the first frame update
    void Start()
    {
        //Spawn(TestSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            spawn = false;
            Spawn2(TestSpawn);
        }
        if (despawn)
        {
            despawn = false;
            gameObject.transform.GetChild(0).transform.GetChild(Random.Range(0, gameObject.transform.GetChild(0).childCount)).gameObject.SetActive(false);
        }
    }


    public GameObject Spawn2(GameObject @object)
    {
        foreach (Transform parent in gameObject.transform)
        {
            //parent type found
            if (parent.name == "P_" + @object.name)
            {
                foreach(Transform child in parent.transform)
                {
                    if (!child.gameObject.activeSelf)
                    {
                        child.gameObject.SetActive(true);
                        return child.gameObject;
                    }
                }
                return Instantiate(@object, transform.position, Quaternion.identity, parent.transform);
            }
        }
        //sets the parent
        GameObject newParent2 = Instantiate(empty, transform.position, Quaternion.identity, gameObject.transform);
        newParent2.name = "P_" + @object.name;
        //summons the object
        return Instantiate(@object, transform.position, Quaternion.identity, newParent2.transform);
    }
}