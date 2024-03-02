using System;

public static class GameManager{
    public static GameState CurrentState {get; private set;}
    
    public static event EventHandler<GameStateEventArgs> OnGameStateChange;

    public class GameStateEventArgs : EventArgs{
        public GameState State;

        public GameStateEventArgs(GameState _state){
            State = _state;
        }
    }
    
    public static void UpdateGameState(GameState newState){
        CurrentState = newState;
        
        switch (newState){
            case GameState.Game:
                break;
            case GameState.UI:
                break;
            case GameState.Cutscene:
                break;
        }

        OnGameStateChange?.Invoke(null, new GameStateEventArgs(newState));
    }
}
