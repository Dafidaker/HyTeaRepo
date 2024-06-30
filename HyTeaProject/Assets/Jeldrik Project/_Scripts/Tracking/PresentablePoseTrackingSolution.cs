using System;
using System.Collections;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.PoseTracking;
using UnityEngine;

// Extension for the MediaPipe ImageSourceSolution that mainly provides an Event which is invoked when the landmarks are received.
// This is then used by the LandMarkProvider to get the landmarks and update the pose of the character.

public class PresentablePoseTrackingSolution : ImageSourceSolution<PoseTrackingGraph>
{
  public static event Action<NormalizedLandmarkList> ReceivedLandmarks;

  protected override void OnStartRun()
  { 
      graphRunner.OnPoseLandmarksOutput += HandlePoseLandmarks;
      SetupScreen(ImageSourceProvider.ImageSource);
  }

  private void HandlePoseLandmarks(object sender, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
  {
      var packet = eventArgs.packet;
      var value = packet?.Get(NormalizedLandmarkList.Parser);
      if (value != default)
      { 
          ReceivedLandmarks?.Invoke(value);
      }
  }
  
  protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
  { 
      graphRunner.AddTextureFrameToInputStream(textureFrame);
  }

  protected override IEnumerator WaitForNextValue()
  { 
      var task = graphRunner.WaitNextAsync();
      yield return new WaitUntil(() => task.IsCompleted);
  }

  protected override void SetupScreen(ImageSource imageSource)
  {
      // Override to ignore screen so we dont need to add the screen component 
  }
}