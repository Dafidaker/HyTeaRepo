using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Gesture")]
public class Gesture : ScriptableObject
{
   public PoseTrackingInfo.LandmarkNames LandMarkToTrack;
   [HideInInspector]
   public TrackAnalysis.StepAnalysisParameters StepAnalysisParameters;
   public List<DirectionPercentage> DirectionPercentages;
   public List<PositionCondition> PositionConditions;
   
   [Serializable]
   public class DirectionPercentage
   {
      [EnumToggleButtons]
      public MovementAnalysis.MotionDirection Direction;
      
      [Range(0,1)]
      public float MinimumPercentage = 0;
      [Range(0,1)]
      public float MaximumPercentage = 1;

      public bool CheckPercentageCondition(int foundCount, int totalCount)
      {
         return foundCount < totalCount * MaximumPercentage && foundCount > totalCount * MinimumPercentage;
      }
   }

   [Serializable]
   public class PositionCondition
   {
      [EnumToggleButtons]
      public enum PositionConditionDirection
      {
         Above,
         Below,
         Left,
         Right
      }

      [EnumToggleButtons]
      public enum PositionConditionType
      {
         Complete,
         Start,
         End,
         StartAndEnd
      }
      
      public PoseTrackingInfo.LandmarkNames Landmark;
      public PositionConditionDirection Direction;
      public PositionConditionType Type;

      public bool CheckPositionConditionOnTrack(List<Tracker.TimeStep> objectTrack, List<Tracker.TimeStep> conditionTrack)
      {
         switch (Type)
         {
            case PositionConditionType.Complete:
               Debug.Log((objectTrack.Count, conditionTrack.Count));
               for (int i = 0; i < objectTrack.Count; i++)
               {
                  if (!CheckPositionConditionOnTimeSteps(objectTrack[i], conditionTrack[i]))
                  {
                     return false;
                  }
               }
               return true;
            case PositionConditionType.Start:
               return CheckPositionConditionOnTimeSteps(objectTrack[0],conditionTrack[0]);
            case PositionConditionType.End:
               return CheckPositionConditionOnTimeSteps(objectTrack[^1],conditionTrack[^1]);
            case PositionConditionType.StartAndEnd:
               return CheckPositionConditionOnTimeSteps(objectTrack[0],conditionTrack[0])
                      && CheckPositionConditionOnTimeSteps(objectTrack[^1],conditionTrack[^1]);
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private bool CheckPositionConditionOnTimeSteps(Tracker.TimeStep objectStep, Tracker.TimeStep conditionStep )
      {
         Vector3 conditionObjectPos = conditionStep.Position;
         Vector3 trackedObjectPos = objectStep.Position;

         switch (Direction)
         {
            case PositionConditionDirection.Above:
               return trackedObjectPos.y > conditionObjectPos.y;
            case PositionConditionDirection.Below:
               return trackedObjectPos.y < conditionObjectPos.y;
            case PositionConditionDirection.Left:
               return trackedObjectPos.x < conditionObjectPos.x;
            case PositionConditionDirection.Right:
               return trackedObjectPos.x > conditionObjectPos.x;
            default:
               throw new ArgumentOutOfRangeException();
            
         }
      }
   }

   public List<PoseTrackingInfo.LandmarkNames> GetNeededTrackers()
   {
      List<PoseTrackingInfo.LandmarkNames> neededLandMarks = new List<PoseTrackingInfo.LandmarkNames>();
      
      neededLandMarks.Add(LandMarkToTrack);

      neededLandMarks.AddRange(PositionConditions.Select(positionCondition => positionCondition.Landmark));

      return neededLandMarks;
   }
}
