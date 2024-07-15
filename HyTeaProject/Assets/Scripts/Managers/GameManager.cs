using System;
using UnityEngine;


public enum GameState
{
    
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

    private void Awake()
    {
        currentCamera = Camera.main;
        EventManager.CameraWasChanged.Invoke(currentCamera);
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
}
