using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PoseAnalysis
{
    public static bool AnalyzeForCrossedArms(List<Vector2> landmarks, bool flipped = false)
    {
        // Cancel analysis if not enough landmarks are found
        if (landmarks.Count <= 30)
        {
            return false;
        }
        
        var leftShoulder =  landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftShoulder]];
        var rightShoulder = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightShoulder]];
        var leftElbow = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftElbow]];
        var rightElbow = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightElbow]];
        var leftWrist = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftWrist]];
        var rightWrist = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightWrist]];

        if (flipped)
        {
            rightShoulder =  landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftShoulder]];
            leftShoulder = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightShoulder]];
            rightElbow = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftElbow]];
            leftElbow = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightElbow]];
            rightWrist = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftWrist]];
            leftWrist = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightWrist]];
        }

        // Calculate shoulder midpoint
        Vector2 shoulderMidpoint =
            new Vector2((leftShoulder.x + rightShoulder.x) / 2, (leftShoulder.x + rightShoulder.x) / 2);
        
        // Elbow position check
        if (leftElbow.x < shoulderMidpoint.x && rightElbow.x > shoulderMidpoint.x)
        {
            // Wrist distance check
            double distanceWrists = Vector2.Distance(leftWrist, rightWrist);
            double distanceShoulders =  Vector2.Distance(leftShoulder, rightShoulder);
            
            if (distanceWrists < distanceShoulders * 0.75f)
            {
                // Angle Analysis 
                float leftElbowAngle = CalculateElbowAngle(leftShoulder, leftElbow, leftWrist);
                float rightElbowAngle = CalculateElbowAngle(rightShoulder, rightElbow, rightWrist);

                if (leftElbowAngle is > 90 and < 150 &&
                    rightElbowAngle is > 90 and < 150)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    // Creating 2 vectors from the 3 points and calculating the angle between them
    private static float CalculateElbowAngle(Vector2 p1,  Vector2 p2,  Vector2 p3)
    {
        Vector2 vector1 = new Vector2 ( p2.x - p1.x, p2.y - p1.y ); 
        Vector2 vector2 = new Vector2 (p3.x - p2.x,  p3.y - p2.y ); 
        
        float dotProduct = vector1.x * vector2.x + vector1.y * vector2.y; 
        
        double magnitude1 =  Vector2.Distance(p1, p2);
        double magnitude2 =  Vector2.Distance(p2, p3); 
        
        double angleRadians = Math.Acos(dotProduct / (magnitude1 * magnitude2));
        
        return (float)(angleRadians * 180.0 / Math.PI); 
    }

    public static (float, float) GetHandsToNoseDistance(List<Vector2> landmarks)
    {
        
        Vector2 leftHandMiddlePoint = GetHandMiddlePoint(PoseTrackingInfo.BodyPart.LeftHand, landmarks);
        Vector2 rightHandMiddlePoint = GetHandMiddlePoint(PoseTrackingInfo.BodyPart.RightHand, landmarks);
        
        Vector2 nosePoint = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.Nose]];

        return (Vector2.Distance(nosePoint, leftHandMiddlePoint), Vector2.Distance(nosePoint, rightHandMiddlePoint));
    }
    
    private static Vector2 GetHandMiddlePoint(PoseTrackingInfo.BodyPart hand, List<Vector2> landmarks)
    {
        
        if (hand != PoseTrackingInfo.BodyPart.LeftHand && hand != PoseTrackingInfo.BodyPart.RightHand)
        {
            Debug.LogError("Can't calculate HandMiddlePoint for non Hand");
        }
        
        List<Vector2> handPoints = PoseTrackingInfo.BodyPartIndexes[hand].Select(index => landmarks[index]).ToList();
        Vector2 handMidpoint = new Vector2(handPoints.Average(p => p.x), handPoints.Average(p => p.y));

        return handMidpoint;
    }
    
    public static float GetShoulderPointDistance(List<Vector2> landmarks)
    {
        var leftShoulder = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftShoulder]];
        var rightShoulder = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightShoulder]];
        
        // Calculate the reference distance (between the shoulders)
        float shoulderDistance = Vector2.Distance(leftShoulder, rightShoulder);

        return shoulderDistance;
    }
    
    public static bool AnalyzeForCrossedLegs(List<Vector2> landmarks, bool flipped = false)
    {
        // Cancel analysis if not enough landmarks are found
        if (landmarks.Count <= 30)
        {
            return false;
        }

        var leftHip = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftHip]];
        var rightHip = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightHip]];
        var leftKnee = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftKnee]];
        var rightKnee = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightKnee]];
        var leftAnkle = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftAnkle]];
        var rightAnkle = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightAnkle]];

        if (flipped)
        {
            rightHip = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftHip]];
            leftHip = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightHip]];
            rightKnee = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftKnee]];
            leftKnee = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightKnee]];
            rightAnkle = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.LeftAnkle]];
            leftAnkle = landmarks[PoseTrackingInfo.LandmarkIndexes[PoseTrackingInfo.LandmarkNames.RightAnkle]];
        }

        // Calculate hip midpoint
        Vector2 hipMidpoint = new Vector2((leftHip.x + rightHip.x) / 2, (leftHip.y + rightHip.y) / 2);

        // Knee position check
        if (leftKnee.x < hipMidpoint.x && rightKnee.x > hipMidpoint.x)
        {
            // Ankle distance check
            double distanceAnkles = Vector2.Distance(leftAnkle, rightAnkle);
            double distanceHips = Vector2.Distance(leftHip, rightHip);

            if (distanceAnkles < distanceHips * 0.75f)
            {
                // Legs are likely crossed
                return true;
            }
        }

        return false;
    }
}
