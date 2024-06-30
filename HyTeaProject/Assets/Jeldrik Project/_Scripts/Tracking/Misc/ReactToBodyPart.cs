using System.Collections.Generic;
using UnityEngine;

// 
public class ReactToBodyPart : MonoBehaviour
{
    [SerializeField] private SkeletonVisualization _visualization;
    [SerializeField] private float _neededTime;
    [SerializeField] private Vector2 _reactionRange = Vector2.one;
    
    private List<GameObject> _objectsToReactTo;
    
    private bool _startedCounting;
    private float _enteredTime;
    private Vector3 _startingScale;

    private void Start()
    {
        InitializeObjectsToReactTo();
        _startingScale = transform.localScale;
    }

    private void InitializeObjectsToReactTo()
    {
        // TODO: Currently only reacting to right hand points, should be generalized
        _objectsToReactTo = _visualization.GetRightHandPoints();
    }
    
    private bool IsBodyPartInRange()
    {
        foreach (var handObject in _objectsToReactTo)
        {
            if (IsPointInRect(handObject.transform.position))
            {
                return true;
            }
        }
        return false;
    }
    
    private bool IsPointInRect(Vector2 point)
    {
        Vector2 pos = transform.position;
        
        bool xInRange = point.x < pos.x + ( _reactionRange.x * 0.5f) && point.x > pos.x - ( _reactionRange.x * 0.5f);
        bool yInRange = point.y < pos.y + ( _reactionRange.y * 0.5f) && point.y > pos.y - ( _reactionRange.y * 0.5f);
        
        return xInRange && yInRange;
    }

    private void Update()
    {
        CountStayInFieldTime();
        ScaleFromStayDuration();
    }
    
    private void CountStayInFieldTime()
    {
        if (IsBodyPartInRange())
        { 
            if (_startedCounting)
            {
                if (HasNeededTimePassed())
                {
                    LoadTimeFinished();
                    _startedCounting = false;
                }
            }
            else
            {
                _enteredTime = Time.time;
                _startedCounting = true;
            }
        }
        else
        {
            _startedCounting = false;
        }
    }

    private void LoadTimeFinished()
    {
        Debug.Log("Finished counting time");
    }

    private bool HasNeededTimePassed()
    {
        return Time.time - _enteredTime > _neededTime;
    }

    private void ScaleFromStayDuration()
    {
        if (_startedCounting)
        {
            transform.localScale = _startingScale + ( _startingScale * (0.5f * ((Time.time - _enteredTime) / _neededTime)));
        }
        else
        {
            transform.localScale = _startingScale;
        }
    }
}
