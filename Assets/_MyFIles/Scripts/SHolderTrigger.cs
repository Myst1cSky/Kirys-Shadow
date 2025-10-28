using UnityEngine;

public class SHolderTrigger : MonoBehaviour
{
    public SButtonPress buttonScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[HolderTrigger] Player entered trigger.");
            buttonScript.NotifyPlayerNearby(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[HolderTrigger] Player exited trigger.");
            buttonScript.NotifyPlayerNearby(false);
        }
    }

}

