using System;
using UnityEngine;

public class SAreaCamTrigger : MonoBehaviour
{
    [SerializeField] private int mAreaIndex;
    [SerializeField] private SCamAreaManager mCamAreaManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mCamAreaManager.SwitchToArea(mAreaIndex);
        }
    }
}
