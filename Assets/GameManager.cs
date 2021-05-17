// Written by Lee Sandberg

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
    DEFAULT         // Is state should never happen
};

public class GameManager : MonoBehaviour
{
    // Game start using pre-level or start-level, called _preload with this
    // GameManger script on a non destructable object on it.
    // So the object and its GameManger script component stays intact inbetwin levels.

    private const int secondsToWaitForLevelLoad = 3; // Wait seconds for Load scene.
    private StateType gameState = StateType.PLAYINGLEVEL;
    private const string characterName = "character";
    private GameObject Character;          // Handle for CharacterController.cs thats on this game object.
    

    // Keep the the current scene index (BuildIndex)
    private int currentSceneIndex = 0;

    [SerializeField]
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

    public StateType GetGameState()
    {
        return gameState;
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

    private bool IsLastLevel()
    {
        if (SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex) return true;
        return false;
    }

    public void AllBoxesArePlaced() 
    {
        Debug.Log("Congratulations You have finished this level !");
        GetComponent<MenuManager>().HideButtonsAndMenus(false);
        GetComponent<Timer>().EndTimer();
        GetComponent<StoreManager>().StoreResultInDictionary();

        gameState = StateType.LEVELDONE;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadScene(1);
    }

    void Init() // Todo: Use or remove.
    {

    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // Start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
       
        //if (Character) Character.GetComponent<PlayerController>().ClearDictionaryOnReloadLevel();   // Remove if not needed.
        currentSceneIndex = sceneIndex;

        GetComponent<MenuManager>().HideButtonsAndMenus(true);

        gameState = StateType.GAMELOADED;
    }

    private void LoadScene(int sceneIndex)
    {
        gameState = StateType.GAMELOADING;
        // Start loading the scene
        StartCoroutine("LoadSceneCoroutine", sceneIndex);

        // currentSceneIndex = sceneIndex;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case StateType.GAMELOADING:
                break;

            case StateType.GAMELOADED:
                // Attach to player controller and levels manager.             
                FindManagers();

                // Initate player controller and level manager.
                playerController.Init(this.gameObject); // Todo: Sort what to do in Awake/Start and not and what to do in Init.
                levelManager.Init(this.gameObject);

                // Initate tile map manager, this game manager.
                tileMapManager.Init(playerController, levelManager);
                this.Init();

                gameState = StateType.PLAYERBEGINPLAY;
                break;

            case StateType.PLAYERBEGINPLAY:

                // No State change here.  Its done in Update when player moves character.
                break;

            case StateType.GAMESTARTED:

                // No State change here.  Its done in 
                break;

            case StateType.STARTLEVEL:

                GetComponent<MenuManager>().HideButtonsAndMenus(true);

                if (Character)
                    {
                        playerController.EnablePlayerTostartToPlay();
                        GetComponent<MenuManager>().HideButtonsAndMenus(true);                     
                        gameState = StateType.PLAYINGLEVEL;
                    }
                break;

            case StateType.GAMEOVER:

                break;

            case StateType.PLAYINGLEVEL:

                    // No State change here. Its done in AllBoxesArePlaced()
                break;

            case StateType.LEVELDONE:
                //int a = 10;
                // No State change here. 
                break;

            case StateType.RESETLEVEL:
                    gameState = StateType.GAMELOADING;
                    LoadScene(currentSceneIndex);
                    
                break;

            case StateType.NEXTLEVEL:
                if (IsLastLevel())
                {
                    Debug.Log("GameManager Switch NEXTLEVEL trying to load more than max number of levels");
                    GetComponent<MenuManager>().HideGameOverText(false);
                    gameState = StateType.GAMEOVER;
                }
                else
                {
                    gameState = StateType.GAMELOADING;
                    playerController.ClearSpriteToGameObjectDict();
                    currentSceneIndex++;
                    LoadScene(currentSceneIndex);
                }
                break;

            default:
                Debug.Log("Game Manager reached an unmapped state.");
                break;
        }
    }
}
