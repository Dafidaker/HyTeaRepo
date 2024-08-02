using UnityEngine;

public class DistanceIndicatorLogic : MonoBehaviour
{
    [SerializeField] private TextIndicator _textIndicator;
    
    private void Update()
    {
        if (LandMarkProvider.Instance.VectorLandmarkList.Count <= 0)
        {
            return;
        }
        float cameraDistance = PoseAnalysis.GetShoulderPointDistance(LandMarkProvider.Instance.VectorLandmarkList);
        _textIndicator.SetDistanceText(cameraDistance.ToString("0.00"));
    }
}
