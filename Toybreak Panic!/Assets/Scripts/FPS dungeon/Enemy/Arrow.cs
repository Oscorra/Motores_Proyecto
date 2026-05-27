using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if (hitTransform.CompareTag("Player"))
        {
            // Apply damage to the player here
            Debug.Log("Player hit by arrow!");
            hitTransform.GetComponent<PlayerHealth>().TakeDamage(15);
        }
        Destroy(gameObject);
    }
}
