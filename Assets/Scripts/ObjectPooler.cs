using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject projectilePrefab;  

    public List<GameObject> projectilePool;
    public float poolSize = 20;

    private void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectilePool.Add(obj);
        }

    }

    // Update is called once per frame    

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in projectilePool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

}
