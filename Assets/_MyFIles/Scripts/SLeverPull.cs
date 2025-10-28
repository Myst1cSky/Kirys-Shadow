using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SLeverPull : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private GameObject pullPromptUI;
    [SerializeField] private Transform lever; // Assign the actual lever here
    [SerializeField] private GameObject objectToHide; // Optional
    [SerializeField] private GameObject objectToShow; // Optional

    [Header("Animation Settings")]
    [SerializeField] private float rotateAngle = 45f; // How far the lever rotates
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private Vector3 rotationAxis = Vector3.right; // Adjust based on hinge orientation

    [Header("Events")]
    public UnityEvent onLeverPulled; // For future actions

    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private bool isActivated = false;
    private bool playerNearby = false;

    private InputAction mInteractAction;
    private PlayerInputActions mPlayerInputActions;

    void Start()
    {
        originalRotation = lever.localRotation;
        targetRotation = originalRotation * Quaternion.Euler(rotationAxis * rotateAngle);

        if (pullPromptUI != null) pullPromptUI.SetActive(false);

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
    }

    void OnDestroy()
    {
        if (mInteractAction != null)
            mInteractAction.performed -= OnInteract;
    }

    void Update()
    {
        Quaternion desiredRotation = isActivated ? targetRotation : originalRotation;
        lever.localRotation = Quaternion.Lerp(lever.localRotation, desiredRotation, Time.deltaTime * rotateSpeed);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerNearby)
        {
            ToggleLever();
        }
    }

    private void ToggleLever()
    {
        isActivated = !isActivated;

        // Optional: Toggle objects
        if (objectToHide != null) objectToHide.SetActive(!isActivated);
        if (objectToShow != null) objectToShow.SetActive(isActivated);

        // Invoke future actions
        onLeverPulled?.Invoke();

        if (pullPromptUI != null) pullPromptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == mPlayerPrefab && !isActivated)
        {
            playerNearby = true;
            if (pullPromptUI != null) pullPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == mPlayerPrefab)
        {
            playerNearby = false;
            if (pullPromptUI != null) pullPromptUI.SetActive(false);
        }
    }

}