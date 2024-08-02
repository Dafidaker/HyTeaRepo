using System;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

public class BodyPartVisibilityIndicatorLogic : MonoBehaviour
{
    
    [Serializable]
    private class IndicatorColorPair
    {
        public PoseTrackingInfo.BodyPart bodyPart;
        public ColorSwitchIndicator indicator;
    }
    
    private readonly Dictionary<PoseTrackingInfo.BodyPart, ColorSwitchIndicator> _indicatorFromBodyPart = new Dictionary<PoseTrackingInfo.BodyPart, ColorSwitchIndicator>();
    [SerializeField] private List<IndicatorColorPair> _switchIndicators;

    private void InitializeDictionary()
    {
        foreach (var pair in _switchIndicators)
        {
            _indicatorFromBodyPart.Add(pair.bodyPart, pair.indicator);
        }
    }

    private void Awake()
    {
        InitializeDictionary();
    }
    
    private void Update()
    {
        if (LandMarkProvider.Instance.LandmarkList.Landmark.Count > 0)
        {
            UpdateIndicators();
        }
        
    }

    private void UpdateIndicators()
    {
        List<PoseTrackingInfo.BodyPart> visibleBodyParts = PoseTrackingInfo.GetVisibleBodyParts(LandMarkProvider.Instance.LandmarkList);
        foreach (var keyPair in  _indicatorFromBodyPart)
        {
            keyPair.Value.SetColor(visibleBodyParts.Contains(keyPair.Key) ? Color.green : Color.red);
        }
    }
}
