using UnityEngine;
using UnityEngine.UI;

public class SCameraSwitch : MonoBehaviour
{
    
    [SerializeField] private Vector3 mCamPosition;
    [SerializeField] private Vector3 mCamRotation;

    public void SwitchToThisLocation()
    {
        
        Camera.main.transform.position = mCamPosition;
        Camera.main.transform.rotation = Quaternion.Euler(mCamRotation);
    }

    public void SetCamTarget(Vector3 position, Vector3 rotation)
    {
        mCamPosition = position;
        mCamRotation = rotation;
    }
}
