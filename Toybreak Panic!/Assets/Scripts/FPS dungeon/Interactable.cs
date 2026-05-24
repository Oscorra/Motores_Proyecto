using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;
    // The message that will be shown to the player when they can interact with this object
    public string promptMessage;
    public void BaseInteract()
    {
        if (useEvents)
        {
            GetComponent<InteractionEvent>().onInteract.Invoke();
        }
        Interact();
    }
    protected virtual void Interact()
    {

    }
}
