using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
   public static UnityEvent GrabOptionEvent = new UnityEvent();
   public static UnityEvent DropOptionEvent = new UnityEvent();
   public static UnityEvent<int> GetNumberOfSiblings = new UnityEvent<int>();
   public static UnityEvent<Topic> TopicSelectedEvent = new UnityEvent<Topic>();
   public static UnityEvent<int> GetIndexOfHeldSlideEvent = new UnityEvent<int>();
   public static UnityEvent<int, int> SwapSlidesEvent = new UnityEvent<int, int>();
   //public static UnityEvent RefreshGridEvent = new UnityEvent();
   public static UnityEvent<Interactable> InteractableIsBeingWatched = new();
   public static UnityEvent NothingIsBeingDetected = new();
   
   ///////////////////////////*Audio Events*//////////////////////////////////////
   public static UnityEvent PressedVolumeAnalysisButton = new();
   public static UnityEvent StartedVolumeAnalysis = new();
   public static UnityEvent FinishedVolumeAnalysis = new();
   
   public static UnityEvent<int> ChangedMicrophone = new();
   public static UnityEvent PopulateMicrophoneList = new();
   public static UnityEvent<bool> ClickedPlaybackButton = new();

   public static UnityEvent StartedNewRecording = new();
   public static UnityEvent<float> ChangedGain = new();
   
   public static UnityEvent<bool> OnPauseStateChanged = new();
}
