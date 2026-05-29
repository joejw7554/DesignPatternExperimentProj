using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance
    {
        get; private set;
    }



    List<GameObject> ObjectPool = new List<GameObject>();

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    int initialPoolsize = 20;

    public GameObject GetBullet()
    {


        foreach (GameObject obj in ObjectPool)
        {
            if(!obj.activeSelf)
            {
                Debug.Log("Ç®¿¡¼­ ²¨³¿");
                obj.SetActive(true);
                return obj;
            }
        }

        //¸ðµÎ Active »óÅÂÀÌ¸é

        var temp = Instantiate(bulletPrefab);
        ObjectPool.Add(temp);

        Debug.Log("»õ·Î »ý¼º");
        return temp;
    }

    public void ReturnBullet (GameObject bullet)
    {

        Debug.Log("Ç®¿¡ ¹Ý³³µÊ");
        bullet.SetActive(false);

    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        for(int i=0; i<initialPoolsize; i++)
        {
            var temp = Instantiate(bulletPrefab);
            temp.SetActive(false);
            ObjectPool.Add(temp);

        }

    }

    void Update()
    {
        
    }
}
