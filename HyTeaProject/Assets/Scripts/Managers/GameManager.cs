using System;
using UnityEngine;


public enum GameState
{
    Dialogue,
    Walking,
    Presentation,
    PrePresentation,
    Calibration
}

public class GameManager : Singleton<GameManager>
{
    public Camera currentCamera;
    public GameState GameState { get; private set; }
    public int Difficulty { get; private set; }
    public Vector2 MouseSensitivity { get; private set; }
    public GameObject Player { get; private set; }
    
    public PlayerCam PlayerCam { get; private set; }
    
    #region Setters

        public void SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
        }
        
        public void SetMouseSensitivity(Vector2 mouseSensitivity)
        {
            MouseSensitivity = mouseSensitivity;
            EventManager.ChangedMouseSensitivity.Invoke(mouseSensitivity);
        }
        
        public void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }
         
        public void SetPlayer(GameObject player)
        {
            Player = player;
        }
        
        public void SetPlayerCam(PlayerCam playerCam)
        {
            PlayerCam = playerCam;
            currentCamera = playerCam.gameObject.GetComponent<Camera>();
            EventManager.CameraWasChanged.Invoke(currentCamera);
        }

    #endregion

    private Camera oldCamera;

    protected override void Awake()
    {
        base.Awake();
        currentCamera = Camera.main;
        EventManager.CameraWasChanged.Invoke(currentCamera);
    }
    
    private void OnEnable()
    {
        EventManager.ChangeToNextSlide.AddListener(EndPresentation);
    }

    private void OnDisable()
    {
        EventManager.ChangeToNextSlide.RemoveListener(EndPresentation);
    }
    
    public void LockPlayerCameraOnTarget(Transform target)
    {
        if (PlayerCam == null) return;

        PlayerCam.LockOnTarget(target);
    }
    
    public void UnlockPlayerCameraOnTarget(Transform target)
    {
        if (PlayerCam == null) return;

        PlayerCam.UnlockOnTarget(target);
    }

    public void StartPresentation(PresentationStartSettings presentationStartSettings)
    {
        SetGameState(GameState.Presentation);
        if (presentationStartSettings != null)
        {
            LockPlayerCameraOnTarget(presentationStartSettings.lookAtTransform);
        }
        //make player not be able to move
        //make player move the place 

        if (presentationStartSettings == null) PresentationManager.Instance.StartPresentation();
        else PresentationManager.Instance.StartPresentation(presentationStartSettings);
    }

    public void PrepareforPresentation(PresentationStartSettings presentationStartSettings)
    {
        SetGameState(GameState.PrePresentation);
        if (presentationStartSettings != null)
        {
            LockPlayerCameraOnTarget(presentationStartSettings.lookAtTransform);
        }
    }

    public void EndPresentation()
    {
        SetGameState(GameState.Walking);
        UnlockPlayerCameraOnTarget(PresentationManager.Instance.GetPresentationStartSettings().lookAtTransform);
        //make player not be able to move
        //make player move the place 

        PresentationManager.Instance.HandleEndPresentation();
    }

    public void SetUpNewCamera(Camera newMainCamera)
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        if (cameras.Length <= 0) return;
        
        if (newMainCamera == null)
        {
            Debug.LogError("New Main Camera is not assigned!");
            return;
        }
        
        foreach (Camera cam in cameras)
        {
            if (cam == newMainCamera)
            {
                cam.tag = "MainCamera";
                //cam.gameObject.SetActive(true);
                cam.enabled = true;
            }
            else
            {
                if (cam.CompareTag("MainCamera"))
                {
                    cam.tag = "Untagged"; // Clear the previous main camera tag
                }
                //cam.gameObject.SetActive(false);
                cam.enabled = false;
            }
        }
        
        /*Camera oldMainCamera = Camera.main;

        if (oldMainCamera == newMainCamera)  return; 
            
        if (oldMainCamera != null)
        {
            oldMainCamera.tag = "Untagged"; 
            oldMainCamera.gameObject.SetActive(false); 
        }
            
        newMainCamera.tag = "MainCamera";
        currentCamera = newMainCamera;
        currentCamera.gameObject.SetActive(true);*/
        
        EventManager.CameraWasChanged.Invoke(currentCamera);
        
    }

}
