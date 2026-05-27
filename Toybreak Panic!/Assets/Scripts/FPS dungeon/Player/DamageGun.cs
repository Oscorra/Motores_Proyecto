using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float damage = 5;
    public float range;
    private Transform playerCamera;
    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    // Update is called once per frame
    public void Shooting()
    {
        Ray gunray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(gunray, out RaycastHit hit, range))
        {
            if (hit.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.health -= damage;
            }
        }
    }
}
