using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SButtonPress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mPlayerPrefab;
    [SerializeField] private GameObject mPressPromptUI;
    [SerializeField] private GameObject mReleasePromptUI;

    [Header("Animation Settings")]
    [SerializeField] private float mPressDistance = 0.5f;
    [SerializeField] private float mPressSpeed = 3f;

    [Header("Events")]
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private InputAction interactAction;
    private PlayerInputActions inputActions;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isPressed;
    private bool playerNearby;

    void Awake()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + Vector3.down * mPressDistance;

        SetUI(false, false);
    }

    void Start()
    {
        var movement = mPlayerPrefab.GetComponent<MovementController>();
        if (movement != null)
        {
            inputActions = movement.GetInputActions();
            interactAction = inputActions?.Gameplay.Interact;
            if (interactAction != null)
                interactAction.performed += OnInteract;
        }
    }

    void OnDestroy()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteract;
    }


    void Update()
    {
        HandleButtonMovement();
    }

    private void HandleButtonMovement()
    {
        Vector3 desiredPosition = isPressed ? targetPosition : originalPosition;

        if (Vector3.Distance(transform.position, desiredPosition) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * mPressSpeed);
        }
    }


    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerNearby) return;
        ToggleButton();
    }

    private void ToggleButton()
    {
        isPressed = !isPressed;
        if (isPressed)
        {
            onButtonPressed?.Invoke();
            SetUI(false, true);
        }
        else
        {
            onButtonReleased?.Invoke();
            SetUI(true, false);
        }
    }

    public void NotifyPlayerNearby(bool nearby)
    {
        playerNearby = nearby;
        if (!nearby)
        {
            SetUI(false, false);
            return;
        }
        SetUI(!isPressed, isPressed);
    }

    private void SetUI(bool showPress, bool showRelease)
    {
        if (mPressPromptUI) mPressPromptUI.SetActive(showPress);
        if (mReleasePromptUI) mReleasePromptUI.SetActive(showRelease);
    }
}

