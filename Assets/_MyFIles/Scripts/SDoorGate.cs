using UnityEngine;

public class SDoorGate : MonoBehaviour
{

    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 3f; // How far the door moves
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool moveUp = true; // Direction toggle

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isOpen = false;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + (moveUp ? Vector3.up : Vector3.down) * moveDistance;
    }

    void Update()
    {
        Vector3 desiredPosition = isOpen ? targetPosition : originalPosition;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
    }

    public void SetDoorOpen(bool open)
    {
        isOpen = open;
    }

}

