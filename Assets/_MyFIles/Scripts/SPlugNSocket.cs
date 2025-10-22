using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SPlugNSocket : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private Transform mBackPosition;
    [SerializeField] private GameObject mPickUpPlugUI;
    [SerializeField] private GameObject mDropPlugUI;
    [SerializeField] private Material plugMaterial;
    [SerializeField] private Collider interactionTrigger; // Assign Box Collider (Trigger) in Inspector

    [Header("Drop Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float safeDropDistance = 1.5f;

    [Header("Respawn Settings")]
    [SerializeField] private float minYPosition = -10f;
    [SerializeField] private Transform respawnPoint;

    public bool canInteract = true;
    private bool isPlayerNearby = false;
    private bool isPickedUp = false;

    public static bool anyPlugPickedUp = false;

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

        if (mPickUpPlugUI != null) mPickUpPlugUI.SetActive(false);
    }

    void OnDestroy()
    {
        if (mInteractAction != null)
            mInteractAction.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!canInteract) return;

        if (isPickedUp)
            DropObject();
        else if (isPlayerNearby)
            PickUpObject();
    }

    void PickUpObject()
    {
        transform.position = mBackPosition.position;
        transform.rotation = mBackPosition.rotation;
        transform.SetParent(mPlayerPrefab.transform);
        isPickedUp = true;
        anyPlugPickedUp = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (mPickUpPlugUI != null) mPickUpPlugUI.SetActive(false);
        if (mDropPlugUI != null) mDropPlugUI.SetActive(true);
    }

    void DropObject()
    {
        transform.SetParent(null);

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        float plugHeight = capsule != null ? capsule.bounds.size.y : 1f;

        Vector3 rayOrigin = mPlayerPrefab.transform.position - mPlayerPrefab.transform.forward * safeDropDistance + Vector3.up * plugHeight;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, plugHeight + 2f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * (plugHeight / 2f);
        }
        else
        {
            transform.position = mPlayerPrefab.transform.position + Vector3.up * (plugHeight + 1f);
            Debug.LogWarning("Ground not detected during drop. Using safe fallback above player.");
        }

        isPickedUp = false;
        anyPlugPickedUp = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (mDropPlugUI != null) mDropPlugUI.SetActive(false);
    }

    void Update()
    {
        if (!isPickedUp && transform.position.y < minYPosition)
        {
            Debug.LogWarning("Plug fell below safe level. Respawning...");
            transform.position = respawnPoint != null ? respawnPoint.position : mPlayerPrefab.transform.position + Vector3.up * 2f;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            isPlayerNearby = true;
            if (mPickUpPlugUI != null && !isPickedUp && !anyPlugPickedUp) mPickUpPlugUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            isPlayerNearby = false;
            if (mPickUpPlugUI != null) mPickUpPlugUI.SetActive(false);
        }
    }

    public Material GetPlugMaterial() => plugMaterial;
    public bool IsPickedUp() => isPickedUp;

    public void SetInteractionEnabled(bool enabled)
    {
        canInteract = enabled;
        if (interactionTrigger != null)
            interactionTrigger.enabled = enabled; // Enable/disable trigger collider
    }

}
