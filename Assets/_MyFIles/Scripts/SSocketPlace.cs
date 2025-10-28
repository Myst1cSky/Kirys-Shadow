using UnityEngine;
using UnityEngine.InputSystem;

public class SSocketPlace : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private GameObject mPlugObject;
    [SerializeField] private Transform mPlugTargetPosition;
    [SerializeField] private GameObject mSocketUI;
    [SerializeField] private Material mSocketMaterial;
    [SerializeField] private SSocketManager mSocketManager;

    private bool isPlayerNearby = false;
    private bool isPlugPlaced = false;
    public bool IsPlugPlaced() => isPlugPlaced;

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

        if (mSocketUI != null) mSocketUI.SetActive(false);
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
        SPlugPickUp plugScript = mPlugObject.GetComponent<SPlugPickUp>();
        bool isPlugOnPlayerBack = plugScript != null && plugScript.IsPickedUp() && mPlugObject.transform.parent == mPlayerPrefab.transform;

        if (isPlugOnPlayerBack && plugScript.GetPlugMaterial() == mSocketMaterial)
        {
            PlacePlug(plugScript);
        }
        else
        {
            Debug.Log("Cannot place plug: Either it's not on player's back or material doesn't match.");
        }
    }

    void PlacePlug(SPlugPickUp plugScript)
    {
        mPlugObject.transform.SetParent(transform);
        mPlugObject.transform.position = mPlugTargetPosition.position;
        mPlugObject.transform.rotation = mPlugTargetPosition.rotation;

        Rigidbody rb = mPlugObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isPlugPlaced = true;
        plugScript.SetInteractionEnabled(false);
        SPlugPickUp.anyPlugPickedUp = false;

        if (mSocketUI != null) mSocketUI.SetActive(false);
        if (plugScript.mDropPlugUI != null) plugScript.mDropPlugUI.SetActive(false);

        Debug.Log("Plug successfully placed in socket.");

        mSocketManager.CheckSockets();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isPlugPlaced)
        {
            isPlayerNearby = true;

            SPlugPickUp plugScript = mPlugObject.GetComponent<SPlugPickUp>();
            if (plugScript != null)
            {
                plugScript.SetInteractionEnabled(false);
                if (plugScript.mDropPlugUI != null) plugScript.mDropPlugUI.SetActive(false);
            }

            if (mSocketUI != null) mSocketUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isPlugPlaced)
        {
            isPlayerNearby = false;

            SPlugPickUp plugScript = mPlugObject.GetComponent<SPlugPickUp>();
            if (plugScript != null)
            {
                plugScript.SetInteractionEnabled(true);
                if (plugScript.IsPickedUp() && plugScript.mDropPlugUI != null)
                    plugScript.mDropPlugUI.SetActive(true);
            }

            if (mSocketUI != null) mSocketUI.SetActive(false);
        }
    }

}
