using UnityEngine;

public class CalibrationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue beginningCalibration;
    private void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(beginningCalibration);
    }
}
