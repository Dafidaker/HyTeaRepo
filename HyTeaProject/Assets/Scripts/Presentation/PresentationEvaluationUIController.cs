using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresentationEvaluationUIController : MonoBehaviour
{
    [field: SerializeField] private GameObject VerticalLayout1;
    [field: SerializeField] private GameObject VerticalLayout2;

    [field: SerializeField] private GameObject Title;
    [field: SerializeField] private GameObject PositiveThing;
    [field: SerializeField] private GameObject NegativeThing;
    [field: SerializeField] private GameObject NeutralThing;
    
    [field: SerializeField] private List<Feedback> AllFeedback;
    private List<Feedback> UsedFeedback;
    
    public void PopulateScreenEditorButton()
    {
        if (AllFeedback.Count <= 0 ) return;

        List<TypeOfFeedback> typeOfFeedbacksCompleted = new List<TypeOfFeedback>();
        GameObject Layout = VerticalLayout1;
        UsedFeedback = new List<Feedback>();
        
        foreach (var selectedFeedback in AllFeedback)
        {
            if (typeOfFeedbacksCompleted.Contains(selectedFeedback.TypeOfFeedback))
            {
                continue;
            }
            
            UsedFeedback.Add(selectedFeedback);
            typeOfFeedbacksCompleted.Add(selectedFeedback.TypeOfFeedback);
            
            foreach (var feedback in AllFeedback)
            {
                if (feedback != selectedFeedback && feedback.TypeOfFeedback == selectedFeedback.TypeOfFeedback)
                {
                    UsedFeedback.Add(feedback);
                }
            }

            AddToLayout(Layout);
            
            Layout = Layout == VerticalLayout1 ? VerticalLayout2 : VerticalLayout1;
        }
    }
    
    public void PopulateScreen(List<Feedback> feedbacks)
    {
        GameObject Layout = VerticalLayout1;
        UsedFeedback = new List<Feedback>();
        AllFeedback = feedbacks;


        foreach (var selectedFeedback in AllFeedback)
        {
            UsedFeedback.Add(selectedFeedback);
            foreach (var feedback in AllFeedback)
            {
                if (feedback.TypeOfFeedback == selectedFeedback.TypeOfFeedback)
                {
                    UsedFeedback.Add(selectedFeedback);
                }
            }

            AddToLayout(Layout);
            
            Layout = Layout == VerticalLayout1 ? VerticalLayout2 : VerticalLayout1;
        }
    }

    private void AddToLayout(GameObject VerticalLayout)
    {
        if (UsedFeedback.Count <= 0) return;

        AddTitle(VerticalLayout, UsedFeedback[0].TypeOfFeedback);
        
        foreach (var feedback in UsedFeedback)
        {
            AddText(VerticalLayout,feedback);
        }
        
        UsedFeedback.Clear();
    }

    private void AddTitle(GameObject VerticalLayout, TypeOfFeedback typeOfFeedback)
    {
        var go = Instantiate(Title, VerticalLayout.transform);

        go.GetComponentInChildren<TextMeshProUGUI>().text = typeOfFeedback.ToString();
    }

    private void AddText(GameObject VerticalLayout, Feedback feedback)
    {
        var selectedPrefab = NeutralThing;

        switch (feedback.FeedbackPolarity)
        {
            case FeedbackPolarity.Negative:
                selectedPrefab = NegativeThing;
                break;
            case FeedbackPolarity.Positive:
                selectedPrefab = PositiveThing;
                break;
            case FeedbackPolarity.Neutral:
                selectedPrefab = NeutralThing;
                break;
        }
        
        var go = Instantiate(selectedPrefab, VerticalLayout.transform);

        go.GetComponentInChildren<TextMeshProUGUI>().text = feedback.TextFeedback;
    }
    
    
}
