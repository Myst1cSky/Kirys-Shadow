using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SPlugNSocket : MonoBehaviour
{
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private Transform mBackPosition;
    [SerializeField] private GameObject mPlugUI;
    [SerializeField] private Material mPlugMaterial;
    private bool isPlayerNearby = false;
    private bool isPickedUp = false;

    private InputAction mInteractAction;
    private PlayerInputActions mPlayerInputActions;


    void Start()
    {
        if (mPlayerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned in Inspector.");
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
        else
        {
            Debug.LogError("MovementController not found on player.");
        }
        if (mPlugUI != null)
        {
            mPlugUI.SetActive(false);
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
        if (isPlayerNearby && !isPickedUp)
        {
            PickUpObject();
        }
    }

    void PickUpObject()
    {
        if (mBackPosition == null || mPlayerPrefab == null)
        {
            Debug.LogError("Missing reference: backPosition or player not assigned.");
            return;
        }

        transform.position = mBackPosition.position;
        transform.rotation = mBackPosition.rotation;
        transform.SetParent(mPlayerPrefab.transform);
        isPickedUp = true;

        if (mPlugUI != null)
        {
            mPlugUI.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            isPlayerNearby = true;
            if (mPlugUI != null && !isPickedUp)
            {
                mPlugUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            isPlayerNearby = false;
            if (mPlugUI != null)
            {
                mPlugUI.SetActive(false);
            }
        }
    }

    public Material GetPlugMaterial()
    {
        return mPlugMaterial;
    }
}
