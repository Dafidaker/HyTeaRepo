using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CalibrationSceneController : MonoBehaviour
{
    private IEnumerator CheckAndLoadScene(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Debug.Log("Scene loaded additively: " + sceneName);
        }
        else
        {
            Debug.Log("Scene is already loaded: " + sceneName);
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        // Loop through all the loaded scenes and check if the target scene is loaded
         for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    
    
    [SerializeField] private Dialogue[] dialogues;
    [FormerlySerializedAs("tablet")] [SerializeField, Header("Tablet"), Space(5)] private GameObject tabletGameObject;
    [SerializeField] private Transform tabletCurrentLocation;
    [SerializeField] private Transform[] tabletLocations;
    [SerializeField] private FloatingMovement tabletFloatingMovement;
    
    private Dialogue GetDialogueFromID(DialogueID dialogueID)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.dialogueID == dialogueID)
            {
                return dialogue;
            }

        }

        return null;
    }
    
    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
    }

    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
    }

    private void Awake()
    {
        StartCoroutine(CheckAndLoadScene("LoadAdditively"));
    }


    private void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.BeginningCalibration));
    }

    private void HandleDialogueEnding(DialogueID id)
    {
        if (id == DialogueID.BeginningCalibration)
        {
            MoveTablet();
        }
    }

    private void MoveTablet()
    {
        Transform targetLocation = null;
        foreach (var location in tabletLocations)
        {
            if (location != tabletCurrentLocation)
            {
                targetLocation = location;
            }
        }

        if (targetLocation == null) return;
        
        StartCoroutine(LerpTransform(tabletGameObject.transform, targetLocation, 1.5f));
    }
    
    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            tabletGameObject.transform.position = Vector3.Lerp(start, end, t);

            time += Time.deltaTime;

            yield return null;
        }

        tabletGameObject.transform.position = end;
    }
    
    private IEnumerator LerpTransform(Transform target, Transform end, float duration)
    {
        //tabletFloatingMovement.enabled = false;
        float time = 0f;

        Vector3 startPosition = target.position;
        Quaternion startRotation = target.rotation;
        Vector3 endPosition = end.position;
        Quaternion endRotation = end.rotation;

        while (time < duration)
        {
            float t = time / duration;

            target.position = Vector3.Lerp(startPosition, endPosition, t);
            target.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            time += Time.deltaTime;

            yield return null;
        }

        target.position = endPosition;
        target.rotation = endRotation;

        tabletCurrentLocation = end;
    }
    
    
}
