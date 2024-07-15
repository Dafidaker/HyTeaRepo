using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TypeOfFeedback
{
   Loudness,
   Pauses,
   Duration,
   Gestures,
   Neutral
}


public enum FeedbackPolarity
{
   Positive,
   Negative,
   Neutral
}


[Serializable]
public class Feedback
{
   public FeedbackPolarity FeedbackPolarity; 
   public TypeOfFeedback TypeOfFeedback;
   [TextArea]public string TextFeedback;

   public Feedback(FeedbackPolarity feedbackPolarity, TypeOfFeedback typeOfFeedback, string textFeedback)
   {
      FeedbackPolarity = feedbackPolarity;
      TypeOfFeedback = typeOfFeedback;
      TextFeedback = textFeedback;
   }
}


public class PresentationEvaluation : MonoBehaviour
{
   [field:SerializeField] private PresentationData _presentationData;

   private List<Feedback> _feedbacks;

   private float _minScriptDuration;
   private float _maxScriptDuration;

   private int _amountOfWords;
   /*private void DetermineAmountOfWords()
   {
      if (_presentationData == null) return;

      AmountOfWords = CountWords(_presentationData.presentationScript);
   }*/
   
   /*private void DetermineDesiredDuration()
   {
      if (_presentationData == null) return;

      if (AmountOfWords == 0)
      {
         DetermineAmountOfWords();
      }

      if (AmountOfWords == 0) return; 

      //being that 100 and 150 are the min and max average words per min for presentations 
      _minScriptDuration = AmountOfWords / 100;
      _maxScriptDuration = AmountOfWords / 150;
   }*/
   
   /*private int CountWords(string input)
   {
      if (string.IsNullOrEmpty(input))
      {
         return 0;
      }

      string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
      return words.Length;
   }*/

   private void Awake()
   {
      _presentationData = PresentationManager.Instance.GetPresentationData();
   }

   private float GetSumOfPausesDuration()
   {
      var totalPauseDuration = 0.0f;
      foreach (var pause in _presentationData._timedPauses)
      {
         totalPauseDuration += pause.Duration;
      }
      return totalPauseDuration;
   }

   private float PercentageOfPresentationInPauses()
   {
      return Mathf.Clamp(GetSumOfPausesDuration() / (_presentationData.Duration / 100),0f,100f);
   }
      
   private List<float> EvaluateLoudness()
   {
      int numOfSections = 3;
      float sectionDuration =  _presentationData.Duration / numOfSections;

      List<float> loudnessAverages = new List<float>();
      
      List<TimedLoudness> currentLoudness = new();
      float startingTime = _presentationData._timedLoudness[0].Time;

      for (int i = 0; i < _presentationData._timedLoudness.Count; i++)
      {
         TimedLoudness timedLoudness = _presentationData._timedLoudness[i];
         bool isLastElement = i == _presentationData._timedLoudness.Count - 1;
         
         if (startingTime + sectionDuration <= timedLoudness.Time || isLastElement )
         {
            startingTime = timedLoudness.Time;
            
            loudnessAverages.Add(GetAverageLoudness(currentLoudness));
            
            currentLoudness.Clear();
         }
         
         currentLoudness.Add(timedLoudness);
      }

      return loudnessAverages;
   }

   private float GetAverageLoudness(List<TimedLoudness> loudnesses)
   {
      float average = loudnesses.Sum(loudness => loudness.Loudness);

      return average / loudnesses.Count;
   }

   private void CheckClipForWords()
   {
      //_presentationData._AudioClips
      
   }
   
   public void EvaluatePresentation()
   {
      _feedbacks = new List<Feedback>();

      //get how loud the player was in the beginning / micdle / end
      List<float> loudnessAverages = new List<float>(EvaluateLoudness());

      string loudnessFeedback = "loudness in the beginning middle and end where ";
      foreach (var loudness in loudnessAverages)
      {
         loudnessFeedback += MicrophoneManager.Instance._volumeAnalyzer.GetSpeakingVolume(loudness) + " ";
      }
      
      Debug.Log(loudnessFeedback);
      
      //relate the time spent pausing vs the time of the presentation
      Debug.Log("the percentage of the presentation used for pauses was " + PercentageOfPresentationInPauses());
      
      // try to get words from specific time signatures
      
      
      // see what poses the player did 
      
      //create feedback

      //send event
   }

   private void CreateFeedback()
   {
      _feedbacks ??= new List<Feedback>();
      _feedbacks.Clear();

      
      
      _feedbacks.Add(new Feedback(FeedbackPolarity.Neutral,TypeOfFeedback.Neutral,"Thank you for the presentation"));
   }

}
