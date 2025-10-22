using UnityEngine;
using UnityEngine.InputSystem;

public class SSocketPlace : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private GameObject mPlugObject;
    [SerializeField] private Transform mPlugTargetPosition;
    [SerializeField] private GameObject mInteractionPrompt;
    [SerializeField] private Material socketMaterial;

    private bool isPlayerNearby = false;
    private bool isPlugPlaced = false;

    private InputAction mInteractAction;
    private PlayerInputActions mPlayerInputActions;

    void Start()
    {
        MovementController movement = mPlayerPrefab.GetComponent<MovementController>();
        if (movement != null)
        {
            mPlayerInputActions = movement.GetInputActions();
            if (mPlayerInputActions != null)
            {
                mInteractAction = mPlayerInputActions.Gameplay.Interact;
                mInteractAction.performed += OnInteract;
            }
        }

        if (mInteractionPrompt != null) mInteractionPrompt.SetActive(false);
    }

    void OnDestroy()
    {
        if (mInteractAction != null)
            mInteractAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerNearby && !isPlugPlaced)
            TryPlacePlug();
    }

    void TryPlacePlug()
    {
        SPlugNSocket plugScript = mPlugObject.GetComponent<SPlugNSocket>();
        bool isPlugOnPlayerBack = plugScript != null && plugScript.IsPickedUp() && mPlugObject.transform.parent == mPlayerPrefab.transform;

        if (isPlugOnPlayerBack && plugScript.GetPlugMaterial() == socketMaterial)
        {
            PlacePlug(plugScript);
        }
        else
        {
            Debug.Log("Cannot place plug: Either it's not on player's back or material doesn't match.");
        }
    }

    void PlacePlug(SPlugNSocket plugScript)
    {
        mPlugObject.transform.SetParent(transform);
        mPlugObject.transform.position = mPlugTargetPosition.position;
        mPlugObject.transform.rotation = mPlugTargetPosition.rotation;

        Rigidbody rb = mPlugObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isPlugPlaced = true;
        plugScript.SetInteractionEnabled(false); // Disable interaction and trigger collider permanently
        SPlugNSocket.anyPlugPickedUp = false;

        if (mInteractionPrompt != null) mInteractionPrompt.SetActive(false);

        Debug.Log("Plug successfully placed in socket.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isPlugPlaced)
        {
            isPlayerNearby = true;

            SPlugNSocket plugScript = mPlugObject.GetComponent<SPlugNSocket>();
            if (plugScript != null) plugScript.SetInteractionEnabled(false); // Disable interaction while near socket

            if (mInteractionPrompt != null) mInteractionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isPlugPlaced)
        {
            isPlayerNearby = false;

            SPlugNSocket plugScript = mPlugObject.GetComponent<SPlugNSocket>();
            if (plugScript != null) plugScript.SetInteractionEnabled(true); // Re-enable interaction when leaving

            if (mInteractionPrompt != null) mInteractionPrompt.SetActive(false);
        }
    }

}
