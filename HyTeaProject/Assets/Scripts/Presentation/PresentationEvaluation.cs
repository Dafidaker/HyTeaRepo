using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Feedback
{
   public FeedbackPolarity feedbackPolarity; 
   public TypeOfFeedback typeOfFeedback;
   [TextArea]public string textFeedback;

   public Feedback(FeedbackPolarity feedbackPolarity, TypeOfFeedback typeOfFeedback, string textFeedback)
   {
      this.feedbackPolarity = feedbackPolarity;
      this.typeOfFeedback = typeOfFeedback;
      this.textFeedback = textFeedback;
   }
}
[Serializable]
public class WordInfo
{
   public string[] word;
   public bool wasSaid;

   public WordInfo(List<string> word)
   {
      this.word = word.ToArray();
      wasSaid = false;
   }
   
   public WordInfo(string[] word)
   {
      this.word = word;
      wasSaid = false;
   }
}
[Serializable]
public class TimedWordList
{
   public float startTime; 
   public float endTime;
   public List<WordInfo> wordInfo;
   public bool areTheWordsValid;

   public TimedWordList(float startTime, float endTime, List<string> words)
   {
      this.startTime = startTime;
      this.endTime = endTime;
      //WordInfo = new List<WordInfo>();
      
      foreach (var word in words)
      {
         //WordInfo.Add(new WordInfo(word));
      }
      areTheWordsValid = true;
   }
}
class LoudnessFeedbackHelper
{
   public SpeakingVolume SpeakingVolume { get; private set; } 
   public bool WasFeedbackCreated { get; private set; }

   public void SetWasFeedbackCreated(bool value)
   {
      WasFeedbackCreated = value;
   }

   public LoudnessFeedbackHelper(SpeakingVolume speakingVolume)
   {
      SpeakingVolume = speakingVolume;
      WasFeedbackCreated = false;
   }
}


public class PresentationEvaluation : MonoBehaviour
{
   [FormerlySerializedAs("_presentationData")] [SerializeField] private PresentationData presentationData;
   [SerializeField] private List<TimedWordList> timedWordLists;

   private List<float> _averageLoudnesses = new();

   [SerializeField] private List<Feedback> _feedbacks = new();

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
      presentationData = PresentationManager.Instance.GetPresentationData();
   }
   
   public void EvaluatePresentation()
   {
      if (presentationData == null)
      {
         presentationData = PresentationManager.Instance.GetPresentationData();
      }
      
      _feedbacks = new List<Feedback>();

      //get how loud the player was in the beginning / middle / end
      List<float> loudnessAverages = new List<float>(EvaluateLoudness());

      //todo add the volume analyzer to the presentation once it starts
      var loudnessFeedback = "loudness in the beginning middle and end where ";
      foreach (var loudness in loudnessAverages)
      {
         loudnessFeedback += MicrophoneManager.Instance.volumeAnalyzer.GetSpeakingVolume(loudness) + " ";
      }
      
      Debug.Log(loudnessFeedback);
      
      //relate the time spent pausing vs the time of the presentation
      Debug.Log("the percentage of the presentation used for pauses was " + PercentageOfPresentationInPauses());
      
      // try to get words from specific time signatures
      EvaluateEnunciation();
      
      // see what poses the player did 
      
      //create feedback
      CreateFeedback();

      //send event
   }

   private float GetSumOfPausesDuration()
   {
      var totalPauseDuration = 0.0f;
      foreach (var pause in presentationData._timedPauses)
      {
         totalPauseDuration += pause.Duration;
      }
      return totalPauseDuration;
   }

   private float PercentageOfPresentationInPauses()
   {
      return Mathf.Clamp(GetSumOfPausesDuration() / (presentationData.Duration / 100),0f,100f);
   }
      
   private List<float> EvaluateLoudness()
   {
      if (presentationData == null)  return null; 
      
      var numOfSections = 3;  
      float sectionDuration =  presentationData.Duration / numOfSections;

      var loudnessAverages = new List<float>();
      
      var currentLoudness = new List<TimedLoudness>();
      float startingTime = presentationData._timedLoudness[0].Time;

      for (int i = 0; i < presentationData._timedLoudness.Count; i++)
      {
         TimedLoudness timedLoudness = presentationData._timedLoudness[i];
         var isLastElement = i == presentationData._timedLoudness.Count - 1;
         
         if (startingTime + sectionDuration <= timedLoudness.Time || isLastElement )
         {
            startingTime = timedLoudness.Time;
            
            loudnessAverages.Add(GetAverageLoudness(currentLoudness));
            
            currentLoudness.Clear();
         }
         
         currentLoudness.Add(timedLoudness);
      }

      _averageLoudnesses = loudnessAverages;

      return loudnessAverages;
   }

   private float GetAverageLoudness(List<TimedLoudness> loudnesses)
   {
      float average = loudnesses.Sum(loudness => loudness.Loudness);

      return average / loudnesses.Count;
   }

   private void EvaluateEnunciation()
   {
      if (timedWordLists == null || presentationData == null 
        ||presentationData._sanatizedAudioClips == null) return;
      
      string errorMsg = SpeechRecognitionUtill.ErrorResponse;
      var possibleAudioClipWithWords = new List<AudioClipInfo>();

      foreach (var wordList in timedWordLists)
      {
         possibleAudioClipWithWords.Clear();
         
         foreach (var clip in presentationData._sanatizedAudioClips)
         {
            if (clip.endTime >= wordList.startTime || clip.startTime <= wordList.endTime)
            {
               if (clip.transcription == errorMsg)
               {
                  wordList.areTheWordsValid = false;
                  break;
               }
               possibleAudioClipWithWords.Add(clip);
            }

            if (!wordList.areTheWordsValid) break;
            
            CheckForWordsInAudioClips(possibleAudioClipWithWords,wordList);
            
         }
      }

      timedWordLists.RemoveAll(word => word.areTheWordsValid == false);
   }

   private void CheckForWordsInAudioClips(List<AudioClipInfo> audioClipInfos, TimedWordList wordLists)
   {
      //it gets the words that are in the transcription of the audio clips
      var wordInfoList = from wordInfo in wordLists.wordInfo
         from clip in audioClipInfos
         from word in wordInfo.word
         where clip.transcription.ToLower().Contains(word.ToLower())
         select wordInfo;

      foreach (var wordInfo in wordInfoList)
      {
         wordInfo.wasSaid = true;
      }
   }
   
   private void CreateFeedback()
   {
      _feedbacks ??= new List<Feedback>();
      _feedbacks.Clear();
      
      //up to 3 feedback for loudness
      _feedbacks.AddRange(CreateLoudnessFeedback());
      
      //1 pause feedback
      _feedbacks.AddRange(CreatePauseFeedback());
      
      //1 word feedback
      _feedbacks.AddRange(CreateEnunciationFeedback());
      
      //amount of time taken feedback
      _feedbacks.AddRange( CreateDurationFeedback());
      
      //gestures done 3 feedback (general, good things, bad things)
      
      
      _feedbacks.Add(new Feedback(FeedbackPolarity.Neutral,TypeOfFeedback.Neutral,"Thank you for the presentation"));

      PresentationManager.Instance._feedbacks = _feedbacks;
   }

   #region Create Feedback Funtions
   
   private List<Feedback> CreateLoudnessFeedback()
   {
      if (_averageLoudnesses.Count != 3 ) return null;

      var feedbacks = new List<Feedback>();
      
      /*var timeString = "";
      var LoudnessString = "";
      var Expression = "";*/
      
      var polarity = FeedbackPolarity.Neutral;

      var averageLoudnessesText = new List<SpeakingVolume>();
      var averageLoudnessesHelpers = new List<LoudnessFeedbackHelper>();
      
      foreach (var loudness in _averageLoudnesses)
      {
         averageLoudnessesHelpers.Add(new LoudnessFeedbackHelper(MicrophoneManager.Instance.volumeAnalyzer.GetSpeakingVolume(loudness)));
      }

      for (var i = 0; i < averageLoudnessesHelpers.Count; i++)
      {
         while (averageLoudnessesHelpers[i].WasFeedbackCreated)
         {
            i++;
            if (i >= averageLoudnessesHelpers.Count) return feedbacks;
         }
         var speakingVolume = averageLoudnessesHelpers[i].SpeakingVolume;
         var feedbackString = "";
         
         switch (speakingVolume)
         {
            case SpeakingVolume.Mute:
               polarity = FeedbackPolarity.Negative;
               feedbackString += "your voice was inaudible";
               //
               break;
            case SpeakingVolume.Whisper:
               polarity = FeedbackPolarity.Negative;
               feedbackString += "you were being so quiet";
               //you sounded like you were whispering
               //I could barely hear you 
               break;
            case SpeakingVolume.Normal:
               polarity = FeedbackPolarity.Positive;
               feedbackString += "you were very audible";
               //you were effectively loud
               //you were clearly loud
               //you were audibly confident
               //your voice was a good volume 
               break;
            case SpeakingVolume.Loud:
               polarity = FeedbackPolarity.Negative;
               feedbackString += "your voice was too loud";
               //you were too loud
               break;
         }

         if (polarity == FeedbackPolarity.Negative)
         {
            feedbackString = "Sadly, " + feedbackString;
            //Unfortunately, 
            //Next time try to fix this,
            //This requires attention,
            //This need adjustments,
            //there some room for improvement,
         }
         else if (polarity == FeedbackPolarity.Positive)
         {
            feedbackString = "Well done, " + feedbackString;
            //Bravo,
            //Awesome work,
            //Great job,
         }

         feedbackString += " in the";
         //during

         if (i == 0)
         {
            feedbackString += " beginning";
         }
         else if (i == 1)
         {
            feedbackString += " middle";
         }
         else if (i == 2)
         {
            feedbackString += " end";
         }

         //if there is another part that also has the same loudness it reduces the 2 feedbacks given into 1
         for (var j = i; j < averageLoudnessesHelpers.Count; j++)
         {
            if (averageLoudnessesHelpers[j].SpeakingVolume == speakingVolume)
            {
               if (j == 1)
               {
                  averageLoudnessesHelpers[j].SetWasFeedbackCreated(true);
                  feedbackString += " and middle";
               }
               else if (j == 2)
               {
                  averageLoudnessesHelpers[j].SetWasFeedbackCreated(true);
                  feedbackString += " and end";
               }
            }
         }
         
         feedbackString += " of the presentation";
         
         feedbacks.Add(new Feedback(polarity,TypeOfFeedback.Loudness,feedbackString));
      }
      
      return feedbacks;
   }

   private List<Feedback> CreatePauseFeedback()
   {
      var feedbacks = new List<Feedback>();
      var feedback = new Feedback(FeedbackPolarity.Neutral,TypeOfFeedback.Pauses,"");
      float percentage = PercentageOfPresentationInPauses();
      
      if (percentage is >= 5 and <= 10)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Positive;
         feedback.textFeedback = "Well done! The amount of pauses you did led to a clear and easy to follow presentation.";
      }
      else if (percentage < 5)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "Consider adding more pauses to your speech. They can make your presentation easier to follow.";
         
      }
      else if (percentage > 10)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "You might want to reduce the number of pauses in your presentation. " +
                                 "Too many pauses can disrupt the flow of the presentation.";
      }
      
      feedbacks.Add(feedback);
      
      return feedbacks;
   }

   private List<Feedback> CreateEnunciationFeedback()
   {
      var feedbacks = new List<Feedback>();
      var feedback = new Feedback(FeedbackPolarity.Neutral,TypeOfFeedback.Enunciation,"");

      var validWordLists = new List<TimedWordList>();
      var amountOfValidWords = 0;
      var amountOfValidWordsSaid = 0;
      
      foreach (var listWords in timedWordLists)
      {
         if (listWords.areTheWordsValid)
         {
            validWordLists.Add(listWords);
         }
      }

      foreach (var validWord in validWordLists)
      {
         amountOfValidWords += validWord.wordInfo.Count;
         foreach (var wordInfo in validWord.wordInfo)
         {
            if (wordInfo.wasSaid)
            {
               amountOfValidWordsSaid++;
            }
         }
      }

      if (amountOfValidWordsSaid > amountOfValidWords || amountOfValidWords == 0)
      {
         return feedbacks;
      }
      
      float percentage = Mathf.Clamp(amountOfValidWordsSaid / (amountOfValidWords / 100f),0f,100f);

      if (percentage >= 95)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Positive;
         feedback.textFeedback = "Well done,you really cover the material well. I think you talked about all of the key points";
      }
      else if (percentage >= 80)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Positive;
         feedback.textFeedback = "You really cover the material well, I think you talked upon most of the key points. " +
                                 " There are a few missing but you can do it";
      }
      else if (percentage < 20)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "I didn't hear you talk about almost any of key points." +
                                 " Remember to talk about the defined key points and enunciate them well";
      }
      else if (percentage < 40)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "I didn't hear you talk about a lot of the key points." +
                                 " Remember to talk about them and enunciate them well";
      }
      else if (percentage < 80)
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "I think you missed a few key points." +
                                 " Remember to talk about all the defined key points.";
      }
      
      feedbacks.Add(feedback);
      return feedbacks;
   }

   private List<Feedback> CreateDurationFeedback()
   {
      var feedbacks = new List<Feedback>();
      var feedback = new Feedback(FeedbackPolarity.Neutral,TypeOfFeedback.Duration,"");

      if (presentationData != null) return feedbacks;
      if (presentationData.DesiredDuration == 0 || presentationData.Duration == 0) return feedbacks;
      
      float duration = presentationData.Duration;
      float desiredDuration = presentationData.DesiredDuration;
      
      float gracePeriod = 60;
      if (desiredDuration < 60)
      {
         gracePeriod = desiredDuration * 0.1f;
      }
      
      if (duration > desiredDuration) // took longer than desired
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "your presentation when over the time determined for you presentation";
      }
      else if (duration <= desiredDuration && duration >= desiredDuration - gracePeriod) // took the time desired 
      {
         feedback.feedbackPolarity = FeedbackPolarity.Positive;
         feedback.textFeedback = "Your duration of your presentation was perfect";
      }
      else if (duration < desiredDuration - gracePeriod) // took way less than time desired 
      {
         feedback.feedbackPolarity = FeedbackPolarity.Negative;
         feedback.textFeedback = "The time you took to present was too little, try to explain each topic a bit more";
      }

      return feedbacks;
   }
   
   #endregion

}
