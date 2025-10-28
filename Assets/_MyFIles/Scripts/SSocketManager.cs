using UnityEngine;

public class SSocketManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SSocketPlace[] sockets;
    [SerializeField] private SDoorGate[] doors;

    public void CheckSockets()
    {
        foreach (var socket in sockets)
        {
            if (!socket.IsPlugPlaced())
            {
                foreach (var door in doors)
                {
                    door.SetDoorOpen(false);
                }
                return;
            }
        }
        foreach ( var door in doors)
        {
            door.SetDoorOpen(true);
        }
    }
}
