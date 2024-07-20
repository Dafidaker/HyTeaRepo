using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private GameObject DialogueUIPrefab;
    private AudioClip _talkingClip;

    private void StartDialogue()
    {
        //it uses the robot to get the transform for the player to lock into 
        //start coroutine that is responsible for showing the dialogue 
    }
    
    private void StopDialogue()
    {
        //unlocks player
    }

}
