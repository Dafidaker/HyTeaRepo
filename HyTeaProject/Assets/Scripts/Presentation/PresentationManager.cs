using System.Collections.Generic;
using UnityEngine;

public class PresentationManager : Singleton<PresentationManager>
{
    [field: SerializeField] private PresentationData presentationData;
    [field: SerializeField] private List<Feedback> _feedbacks;
    [field: SerializeField] private GameObject presentationUIPrefab;
    private PresentationEvaluationUIController _presentationEvaluationUIController;
    
    
    private PresentationEvaluation _presentationEvaluation;
    


    private void CreateEvalutationUI()
    {
        var go = Instantiate(presentationUIPrefab);
        _presentationEvaluationUIController = go.GetComponent<PresentationEvaluationUIController>();
    }
}
