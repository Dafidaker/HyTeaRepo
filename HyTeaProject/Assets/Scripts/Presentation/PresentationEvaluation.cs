using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfFeedback
{
   Loudness,
   Pauses,
   Duration,
   Gestures
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
}


public class PresentationEvaluation : MonoBehaviour
{
   [field:SerializeField] private PresentationData _presentationData;

   private float _minScriptDuration;
   private float _maxScriptDuration;

   private int AmountOfWords;
   private void DetermineAmountOfWords()
   {
      if (_presentationData == null) return;

      AmountOfWords = CountWords(_presentationData.presentationScript);
   }
   
   private void DetermineDesiredDuration()
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
   }
   
   private int CountWords(string input)
   {
      if (string.IsNullOrEmpty(input))
      {
         return 0;
      }

      string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
      return words.Length;
   }


   private void EvaluatePresentation()
   {
      
   }

}
