using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image barra_superior;
    public Image barra_inferior;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(Random.Range(5, 10));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestoreHealth(Random.Range(5, 10));
        }
    }

    public void UpdateHealthUI()
    {
        Debug.Log(health);
        float fillF = barra_superior.fillAmount;
        float fillB = barra_inferior.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction) 
        {
            barra_superior.fillAmount = hFraction;
            barra_inferior.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            barra_inferior.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);

        }
        if (fillF < hFraction)
        {
            barra_inferior.color = Color.green; 
            barra_inferior.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            barra_superior.fillAmount = Mathf.Lerp(fillF, barra_inferior.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public void RestoreHealth(float healAmount) 
    {
        health += healAmount;
        lerpTimer = 0f;
    }

}
