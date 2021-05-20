// Written by Lee Sandberg
// © Copyright 2021 Lee Sandberg
// Lee.Sandberg@gmail.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public enum StateType
{
    GAMELOADING,    // First state. GAMELOADING -> GAMELOADED
    GAMELOADED,     // GAMELOADED -> PLAYERBEGINPLAY
    PLAYERBEGINPLAY,// (player moves) -> GAMESTARTED
    GAMESTARTED,    // GAMESTARTED -> (LOADLEVEL level1) -> STARTLEVEL -> PLAYINGLEVEL 
    STARTLEVEL,     // STARTLEVEL -> PLAYINGLEVEL 
    PLAYINGLEVEL,   // PLAYINGLEVEL -> LEVELDONE or (Reset Level) -> LOADLEVEL (again) -> STARTLEVEL
    LEVELDONE,      // LEVELDONE -> NEXTLEVEL -> LOADLEVEL -> (LEVELCHANGED and/or STARTLEVEL)
                    // LEVELDONE -> (if last level) -> GAMEOVER
    NEXTLEVEL,      // NEXTLEVEL -> GAMELOADING
    RESETLEVEL,     // RESETLEVEL -> GAMELOADING
    GAMEOVER,       // Put up "Game Over" sign. -> Explore level times)
    LEVELCHANGED,   // LEVELCHANGED -> STARTLEVEL -> PLAYING
    MENU,           // Todo: decide if we gone have a menu scene or menu manager script.
    DEFAULT         // This state should never happen.
};

public class GameManager : MonoBehaviour
{
    // Game start using pre-level or start-level, called _preload with this.
    // GameManger script on a non destructable object on it.
    // So the object and its GameManger script component stays intact inbetween levels.

    private StateType gameState = StateType.PLAYINGLEVEL;
    private const string characterName = "character";

    // Keep the the current scene index (BuildIndex)
    private int currentSceneIndex = 0;

    private PlayerController playerController;
    private TileMapManager tileMapManager;
    private LevelManager levelManager;

    private bool FindManagers()
    {
        GameObject characterGameObject = GameObject.Find(characterName);
        if (characterGameObject)
        { 
            playerController = characterGameObject.GetComponent<PlayerController>(); // Todo: Remove.
            levelManager = characterGameObject.GetComponent<LevelManager>();
            tileMapManager = GetComponent<TileMapManager>();

            if (playerController && tileMapManager) return true;
            else return false;
        }
        else return false;
    }

    public int GetCurrentScene()
    {
        return currentSceneIndex;
    }

    public void StartPlaying()
    {
        GetComponent<Timer>().BeginTimer();
        gameState = StateType.PLAYINGLEVEL;
    }

    public bool HasLevelStarted()
    {
        if (gameState == StateType.PLAYERBEGINPLAY || gameState == StateType.PLAYINGLEVEL)
        {
            return true;
        }
        else return false;
    }

    public void ResetLevel()
    {
        gameState = StateType.RESETLEVEL; 
    }

    public void NextLevel()
    {
        gameState = StateType.NEXTLEVEL;
    }

    private bool IsThereMoreLevels()
    {
        // We subtract one form sceneCountInBuildSettings, for the _proload level scen.
        if (SceneManager.GetActiveScene().buildIndex < (SceneManager.sceneCountInBuildSettings-1)) return true;
        return false;
    }

    public void AllBoxesArePlaced() 
    {
        Debug.Log("Congratulations You have finished this level !");
        GetComponent<MenuManager>().SetMenuButtonActive(true);
        GetComponent<Timer>().EndTimer();
        GetComponent<StoreManager>().StoreResultInDictionary();
        StoreManager.LevelInfo levelInfo = new StoreManager.LevelInfo();
        
        GetComponent<StoreManager>().InsertLevelsInfoFromPlayedGame();
        if (IsThereMoreLevels()) GetComponent<MenuManager>().SetNextLevelButtonActive(true);
        else
        {
            GetComponent<MenuManager>().SetNextLevelButtonActive(false);
            GetComponent<MenuManager>().SetGameOverTextActive(true);

            gameState = StateType.GAMEOVER;
            return;
        }

        gameState = StateType.LEVELDONE;
    }

    // Start is called before the first frame update.
    void Start()
    {
        GetComponent<MenuManager>().HideButtonsAndMenus(true);
        LoadScene(1);
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // Start loading the scene.
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        // Wait until the level finish loading.
        while (!asyncLoadLevel.isDone)
            yield return null;
       
        currentSceneIndex = sceneIndex;
        GetComponent<Timer>().Init(); 
        GetComponent<MenuManager>().HideButtonsAndMenus(true);

        gameState = StateType.GAMELOADED;
    }

    private void LoadScene(int sceneIndex)
    {
        gameState = StateType.GAMELOADING;
        // Start loading the scene.
        StartCoroutine("LoadSceneCoroutine", sceneIndex);
    }

    public void QuitGame()
    {
        GetComponent<Timer>().EndTimer();
        GetComponent<StoreManager>().SaveToFile();
        GetComponent<MenuManager>().QuitMenu();
        GetComponent<MenuManager>().HideGameOverText(false);

        GetComponent<MenuManager>().HideAllUserInterfaces();
        gameState = StateType.GAMEOVER;
    }

    // Update is called once per frame.
    void Update()
    {
        // Todo: Move this to a coroutine or a function.
        switch (gameState)
        {
            case StateType.GAMELOADING:
            break;

            case StateType.GAMELOADED:
                // Attach to player controller and the levels manager.             
                FindManagers();

                // Initate player controller and the levels manager.
                playerController.Init(this.gameObject); 

                // Initate tile map manager, this game manager.
                tileMapManager.Init(playerController, levelManager);
                //this.Init();

                gameState = StateType.PLAYERBEGINPLAY;
            break;

            case StateType.PLAYERBEGINPLAY:
                // No State change here. State changes in Update when player moves character.
            break;

            case StateType.GAMESTARTED:
                // No State change here. 
            break;

            case StateType.STARTLEVEL:

                GetComponent<MenuManager>().HideButtonsAndMenus(true);

                if (playerController)
                    {
                        playerController.EnablePlayerTostartToPlay();
                        GetComponent<MenuManager>().HideButtonsAndMenus(true);                     
                        gameState = StateType.PLAYINGLEVEL;
                    }
            break;

            case StateType.GAMEOVER:
                Application.Quit();
            break;

            case StateType.PLAYINGLEVEL:
                // No State change here. It´s done in AllBoxesArePlaced()
            break;

            case StateType.LEVELDONE:
                // No State change here. 
            break;

            case StateType.RESETLEVEL:                   
                LoadScene(currentSceneIndex);

                gameState = StateType.GAMELOADING;
            break;

            case StateType.NEXTLEVEL:    
                playerController.ClearSpriteToGameObjectDict(); // Todo: Remove.
                LoadScene(currentSceneIndex + 1);
                    
                gameState = StateType.GAMELOADING;
            break;

            default:
                Debug.Log("Game Manager reached an unmapped state.");
            break;
        }
    }
}
