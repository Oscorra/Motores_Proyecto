using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private float StartingHealth = 10;
    private float Health;

    public float health
    {
        get
        {
            return Health;
        }
        set
        {
            Health = value;
            Debug.Log(Health);

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    void Start()
    {
        health = StartingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
