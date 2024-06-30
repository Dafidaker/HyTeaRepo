using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediapipe;
using UnityEngine;

public class LandMarkProvider : MonoBehaviour
{
    public NormalizedLandmarkList LandmarkList { get; private set; } = new NormalizedLandmarkList();
    public List<Vector2> VectorLandmarkList { get; private set; } = new List<Vector2>();
    public static LandMarkProvider Instance { get; private set; }
    
    [SerializeField] private bool _flipped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PresentablePoseTrackingSolution.ReceivedLandmarks += SaveLandMarks;
    }
    
    private void OnDestroy()
    {
        PresentablePoseTrackingSolution.ReceivedLandmarks -= SaveLandMarks;
    }

    private void SetFlipped(bool newFlippedStatus)
    {
        _flipped = newFlippedStatus;
    }

    private void SaveLandMarks(NormalizedLandmarkList list)
    {
        if (_flipped)
        {
            SaveLandMarksFlipped(list);
        }
        else
        {
            SaveLandMarksNotFlipped(list);
        }
    }

    private void SaveLandMarksNotFlipped(NormalizedLandmarkList list)
    {
        LandmarkList = list;
        VectorLandmarkList = NormalizedLandmarkListToVector2List(list);
    }

    private void SaveLandMarksFlipped(NormalizedLandmarkList list)
    {
        LandmarkList = FlipLandMarkList(list);
        VectorLandmarkList = NormalizedLandmarkListToVector2List(LandmarkList);
    }

    private static NormalizedLandmarkList FlipLandMarkList(NormalizedLandmarkList list)
    {
        foreach (var normalizedLandmark in list.Landmark)
        {
            normalizedLandmark.X *= -1;
            normalizedLandmark.Y *= -1;
            normalizedLandmark.Z *= -1;
        }

        return list;
    }
    
    private static List<Vector2> NormalizedLandmarkListToVector2List(NormalizedLandmarkList landmarkList)
    {
        return landmarkList.Landmark.Select(landmark => new Vector2(landmark.X, landmark.Y)).ToList();
    }
    
    public async Task WaitForLandMarkData()
    {
        while (!LandMarkDataAvailable())
        {
            await Task.Delay(500);
        }
    }

    private bool LandMarkDataAvailable()
    {
        return VectorLandmarkList.Count > 0;
    }
}
