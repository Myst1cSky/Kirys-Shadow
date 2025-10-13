using UnityEngine;

public class SCamAreaManager : MonoBehaviour
{

    [System.Serializable]
    public class AreaData
    {
        public Vector3 mBasePosition;
        public Vector3 mBaseRotation;
        public SCameraSwitch[] mButtonConfigs;
        public Vector3[] mButtonPositions;
        public Vector3[] mButtonRotations;
    }

    [SerializeField] private AreaData[] areas;
    [SerializeField] private int currentAreaIndex = 0;

    public void SwitchToArea(int areaIndex)
    {
        if (areaIndex < 0 || areaIndex >= areas.Length) { return;}

        currentAreaIndex = areaIndex;
        AreaData area = areas[areaIndex];

        Camera.main.transform.position = area.mBasePosition;
        Camera.main.transform.rotation = Quaternion.Euler(area.mBaseRotation);

        // Update buttons
        for (int i = 0; i < area.mButtonConfigs.Length; i++)
        {
            area.mButtonConfigs[i].SetCamTarget(area.mButtonPositions[i], area.mButtonRotations[i]);
        }
    }

}
