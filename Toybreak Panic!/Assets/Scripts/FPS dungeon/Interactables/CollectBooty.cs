using UnityEngine;

public class CollectBooty : Interactable
{
    public GameObject particle;
    protected override void Interact()
    {
        Destroy(gameObject);
        Instantiate(particle, transform.position, Quaternion.identity);
    }
}
