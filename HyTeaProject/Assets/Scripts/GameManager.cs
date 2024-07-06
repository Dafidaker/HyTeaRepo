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
    
    #region Setters

    private void SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
        }
        
        private void SetMouseSensitivity(Vector2 mouseSensitivity)
        {
            MouseSensitivity = mouseSensitivity;
            EventManager.ChangedMouseSensitivity.Invoke(mouseSensitivity);
        }
        
        private void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }

    #endregion

    private void Awake()
    {
        currentCamera = Camera.main;
        EventManager.CameraWasChanged.Invoke(currentCamera);
    }
}
