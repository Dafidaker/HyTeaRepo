using System.Collections.Generic;
using Mediapipe;
using UnityEngine;

public class SkeletonVisualization : MonoBehaviour
{
    [SerializeField] private List<GameObject> _skeletonPoints;
    [SerializeField] private GameObject _invisPoint;
    [SerializeField] private GameObject _originPoint;
    [SerializeField] private GameObject _connectionRendererPrefab;
    [SerializeField] private Transform _connectionRendererParent;
    [SerializeField] private GameObject _headObject;

    [SerializeField] private float _positionScale; 
    [Range(0,1)]
    [SerializeField] private float _requiredVisibility; 
    
    private readonly List<LineRenderer> _connectionRendererList = new List<LineRenderer>();
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        foreach (var unused in PoseTrackingInfo.Connections)
        {
            SpawnConnectionRenderer();
        }

        _headObject.transform.localScale = Vector3.one * 0.2f *_positionScale;
    }

    private void SpawnConnectionRenderer()
    {
        GameObject connectionRenderer = Instantiate(_connectionRendererPrefab, _connectionRendererParent);
        _connectionRendererList.Add(connectionRenderer.GetComponent<LineRenderer>());
    }
    
    private void Update()
    {
        UpdateSkeletonPointPositions();
        UpdateSkeletonConnections();
        UpdateHead();
    }
    
    private void UpdateSkeletonPointPositions()
    {
        NormalizedLandmarkList landMarkList = LandMarkProvider.Instance.LandmarkList;
        for (int i = 0; i < landMarkList.Landmark.Count; i++)
        {
            if (landMarkList.Landmark[i].Visibility < _requiredVisibility)
            {
                HideSkeletonPoint(_skeletonPoints[i]);
            }
            else
            {
                // TODO: add switch ? 
                if (true)
                {
                    PositionSkeletonPointFromLandmarkRelativeToOrigin(i, landMarkList.Landmark[i]);
                }
                else
                {
                    PositionSkeletonPointFromLandmarkRelativeToImage(i, landMarkList.Landmark[i]);
                }
            }
        }
    }
    
    private void HideSkeletonPoint(GameObject point)
    {
        point.transform.position = _invisPoint.transform.position;
    }

    private void PositionSkeletonPointFromLandmarkRelativeToOrigin(int pointNumber, NormalizedLandmark normalizedLandmark)
    {
        if (pointNumber >= _skeletonPoints.Count) return;

        Vector3 vectorFromLandmark = new Vector2(normalizedLandmark.X, normalizedLandmark.Y);
        _skeletonPoints[pointNumber].transform.localPosition = _originPoint.transform.localPosition + ( vectorFromLandmark * _positionScale );
    }
    
    private void PositionSkeletonPointFromLandmarkRelativeToNose(int pointNumber, NormalizedLandmark normalizedLandmark)
    {
        if (pointNumber >= _skeletonPoints.Count) return;

        _skeletonPoints[pointNumber].transform.localPosition = _originPoint.transform.localPosition + (GetOffsetToNose(normalizedLandmark) * _positionScale);
    }
    

    private void PositionSkeletonPointFromLandmarkRelativeToImage(int pointNumber, NormalizedLandmark normalizedLandmark)
    {
        if (pointNumber >= _skeletonPoints.Count) return;

        _skeletonPoints[pointNumber].transform.localPosition =
            new Vector3(normalizedLandmark.X, normalizedLandmark.Y,0) * _positionScale;
    }
    
    private Vector3 GetOffsetToNose(NormalizedLandmark normalizedLandmark)
    {
        NormalizedLandmarkList landMarkList = LandMarkProvider.Instance.LandmarkList;
        
        Vector3 vectorFromLandmark = new Vector3(normalizedLandmark.X, normalizedLandmark.Y, 0);
        Vector3 noseVector = new Vector3(landMarkList.Landmark[0].X, landMarkList.Landmark[0].Y, 0);

        return vectorFromLandmark - noseVector;
    }

    private void UpdateSkeletonConnections()
    {
        if (_connectionRendererList.Count == 0) return;
        
        for (int i = 0; i < PoseTrackingInfo.Connections.Count; i++)
        {
            Vector3 pos1 = _skeletonPoints[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.Connections[i].Item1]].transform.position;
            Vector3 pos2 = _skeletonPoints[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.Connections[i].Item2]].transform.position;

            // If the connection is not visible, set the positions to zero
            if (! (pos1 == _invisPoint.transform.position || pos2 == _invisPoint.transform.position) )
            {
                _connectionRendererList[i].SetPosition(0, pos1);
                _connectionRendererList[i].SetPosition(1, pos2);
            }
            else
            {
                _connectionRendererList[i].SetPosition(0, Vector3.zero);
                _connectionRendererList[i].SetPosition(1, Vector3.zero);
            }
        }
    }
    
    private void UpdateHead()
    {
        _headObject.transform.localPosition = _skeletonPoints[0].transform.localPosition;
    }
    
    public List<GameObject> GetRightHandPoints()
    {
        return GetGameObjectsFromIndexList(PoseTrackingInfo.BodyPartIndexes[PoseTrackingInfo.BodyPart.RightHand]);
    }
    
    private List<GameObject> GetGameObjectsFromIndexList(List<int> idList)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (int id in idList)
        {
            gameObjects.Add(_skeletonPoints[id]);
        }

        return gameObjects;
    }
}
