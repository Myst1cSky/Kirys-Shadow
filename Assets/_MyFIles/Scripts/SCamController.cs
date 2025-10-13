using UnityEngine;

public class SCamController : MonoBehaviour
{
    [SerializeField] private Transform[] mCamPositions;
    private int mCurrentAreaIndex = 0;

    public void MoveToPosition(int index)
    {
        if (index >= 0 && index < mCamPositions.Length)
        {
            Camera.main.transform.position = mCamPositions[index].position;
            Camera.main.transform.rotation = mCamPositions[index].rotation;
        }
    }

    public void SetAreaPositions(Transform[] newPositions)
    {
        mCamPositions = newPositions;
    }
}
