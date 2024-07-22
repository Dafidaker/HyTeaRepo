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
   public static UnityEvent<int> SetChosenOptionEvent = new UnityEvent<int>();
   public static UnityEvent<Interactable> InteractableIsBeingWatched = new();
   public static UnityEvent NothingIsBeingDetected = new();

   public static UnityEvent<int> SetNumOfSectionsAdded = new UnityEvent<int>();
   public static UnityEvent<Section, Vector2> AddSectionToOrderEvent = new UnityEvent<Section, Vector2>();

   public static UnityEvent<List<GameObject>> GetCompletedSlides = new UnityEvent<List<GameObject>>();
   
   ///////////////////////////*Audio Events*//////////////////////////////////////
   public static UnityEvent PressedVolumeAnalysisButton = new();
   public static UnityEvent StartedVolumeAnalysis = new();
   public static UnityEvent FinishedVolumeAnalysis = new();
   
   public static UnityEvent<int> DifferentMicrophoneSelectedInUI = new();
   public static UnityEvent PopulateMicrophoneList = new();
   public static UnityEvent<bool> ClickedPlaybackButton = new();

   public static UnityEvent StartedNewRecording = new();
   public static UnityEvent StoppedRecording = new();
   
   public static UnityEvent<float> ChangedGain = new();
   public static UnityEvent<float> ChangedLoudnessMultiplier = new();
   
   public static readonly UnityEvent<float> OnPauseDone = new();
   public static readonly UnityEvent PauseStarted = new();
   public static readonly UnityEvent<float> LatestLoudnessCaptured = new();
   
   public static readonly UnityEvent<AudioClip> NewAudioClipFinished = new();
   
   
   public static readonly UnityEvent CalibrationHasFinished = new();
   public static readonly UnityEvent CalibrationHasStarted = new();

   ///////////////////////////*Gestures*//////////////////////////////////////
   public static readonly UnityEvent<float, float, GesturesEnum> GestureCaptured = new();
   
   ///////////////////////////*Settings*//////////////////////////////////////
   public static readonly UnityEvent<Vector2> ChangedMouseSensitivity = new();
   
   ///////////////////////////*Scene*//////////////////////////////////////
   public static readonly UnityEvent<Camera> CameraWasChanged = new();
   
   ///////////////////////////*Presentation*//////////////////////////////////
   public static readonly UnityEvent ChangeToNextSlide = new();
   public static readonly UnityEvent CameraWasLocked = new();
   public static readonly UnityEvent CameraWasUnlocked = new();
   
   ///////////////////////////*AI*//////////////////////////////////
   public static readonly UnityEvent<RobotController,Transform> ArrivedAtTarget = new();
   public static readonly UnityEvent<RobotController> FeedbackWasGiven = new();
   
   
   ///////////////////////////*Input*//////////////////////////////////
   public static readonly UnityEvent MouseWasPressed = new();
   public static readonly UnityEvent SpaceWasPressed = new();
   public static readonly UnityEvent<DialogueID> DialogueWasEnded = new();
   
}
