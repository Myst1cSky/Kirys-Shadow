using UnityEngine;
using UnityEngine.InputSystem;

public class SSocketPlace : MonoBehaviour
{
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private GameObject mPlugObject;
    [SerializeField] private Transform mPlugTargetPosition;
    [SerializeField] private GameObject mInteractionPrompt;
    [SerializeField] private Material mSocketMaterial;

    private bool isPlayerNearby = false;
    private bool isPlugPlaced = false;

    private InputAction mInteractAction;
    private PlayerInputActions mPlayerInputActions;

    void Start()
    {
        if (mPlayerPrefab == null || mPlugObject == null || mPlugTargetPosition == null)
        {
            Debug.LogError("Missing references in SocketInteraction.");
            return;
        }

        MovementController movement = mPlayerPrefab.GetComponent<MovementController>();
        if (movement != null)
        {
            mPlayerInputActions = movement.GetInputActions();
            if (mPlayerInputActions != null)
            {
                mInteractAction = mPlayerInputActions.Gameplay.Interact;
                mInteractAction.performed += OnInteract;
            }
            else
            {
                Debug.LogError("PlayerInputActions not initialized in MovementController.");
            }
        }

        if (mInteractionPrompt != null)
        {
            mInteractionPrompt.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (mInteractAction != null)
        {
            mInteractAction.performed -= OnInteract;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerNearby && !isPlugPlaced)
        {
            PlacePlug();
        }
    }

    void PlacePlug()
    {
        SPlugNSocket plugScript = mPlugObject.GetComponent<SPlugNSocket>();
        if (plugScript != null && plugScript.GetPlugMaterial() == mSocketMaterial)
        {
            mPlugObject.transform.position = mPlugTargetPosition.position;
            mPlugObject.transform.rotation = mPlugTargetPosition.rotation;
            mPlugObject.transform.SetParent(transform);
            isPlugPlaced = true;

            if (mInteractionPrompt != null)
            {
                mInteractionPrompt.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Plug material does not match socket material.");
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isPlugPlaced)
        {
            isPlayerNearby = true;
            if (mInteractionPrompt != null)
            {
                mInteractionPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            isPlayerNearby = false;
            if (mInteractionPrompt != null)
            {
                mInteractionPrompt.SetActive(false);
            }
        }
    }

}
