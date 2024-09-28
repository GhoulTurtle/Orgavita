using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RoomSceneManager{
    public static Action OnSaveRoom;
    public static Action<TransitionPointToSpawnAt> OnGetLevelTransition;
    public static Action OnTransitionPointUpdated;
    public static Action OnLoadRoom;

    public static RoomDataSO RoomToLoad {get; private set;}
    public static TransitionPointToSpawnAt CurrentTransitionPoint {get; private set;}
    public static LevelTransition TransitionPointToSpawnAt {get; private set;}
    public static FadeTime OverrideFadeTime;

    public static bool UpdateRoomData = true;

    public static void LoadRoom(RoomDataSO roomDataSO, TransitionPointToSpawnAt pointToSpawnAt, FadeTime overrideFadeTime){
        if(UpdateRoomData){
            //Save the current room status
            OnSaveRoom?.Invoke();
        }

        CurrentTransitionPoint = pointToSpawnAt;
        RoomToLoad = roomDataSO;
        OverrideFadeTime = overrideFadeTime;

        //Pause the game
        Time.timeScale = 0;
        //Switch Cutscene State
        GameManager.UpdateGameState(GameState.Cutscene);
        //Fade out audio and trigger UI transistion screen
        TransitionHandler.Instance.StartFadeIn(OnLoadScene, overrideFadeTime);
    }

    private static void OnLoadScene(){
        //Load the scene
        SceneManager.LoadScene(RoomToLoad.GetSceneName());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode){    
        //Move the player to the proper location
        OnGetLevelTransition?.Invoke(CurrentTransitionPoint);

        //Update the scene
        if(UpdateRoomData){
            OnLoadRoom?.Invoke();
        }
        //Unpause the game
        Time.timeScale = 1;

        UnsubscribeFromSceneLoadedCallback();

        //Fade in audio and screen transition
        TransitionHandler.Instance.StartFadeOut(OnLevelTransitionFinished, OverrideFadeTime);
    }

    private static void OnLevelTransitionFinished(){
        //Switch Back to Game State
        GameManager.UpdateGameState(GameState.Game);
    }

    public static void UnsubscribeFromSceneLoadedCallback(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void SetTransitionPoint(LevelTransition transitionPointToSpawnAt){
        TransitionPointToSpawnAt = transitionPointToSpawnAt;
        OnTransitionPointUpdated?.Invoke();
    }
}