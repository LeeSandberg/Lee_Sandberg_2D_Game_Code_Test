// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const string targetSpriteName = "box_holder";

    public Transform    TargetPoint;      // Motition helper
    public GameObject   Box;              // Handel for Boxes

    private const float speed = 5f;       // Motition speed
    private bool hasStartedPlaying = false;

    private GameObject gameManagerGameObject;
    private GameManager gameManager;
    private TileMapManager tileMapManager;
    private LevelManager levelManager;

    private bool FindManagers()
    {
        if (gameManagerGameObject)
        {
            gameManager = gameManagerGameObject.GetComponent<GameManager>();
            tileMapManager = gameManagerGameObject.GetComponent<TileMapManager>();
            if (tileMapManager) return true;
            else return false;
        }
        else
        {
            Debug.Log("findManagers in Playercontroller can't find: gameManagerGameObject it should be there in _proload scene as a object, maybe DontDestroyOnLoad(gameObject); didn't run in DDOL.cs? So it's deleted when first level is loaded.");
            return false;
        }
    }

    // Called before Start()
    private void Awake()
    {
        // Get Managers in this GameObject.
        levelManager = GetComponent<LevelManager>();
        Debug.Log("Levels PlayerControlllers Awake called.");
        
        hasStartedPlaying = false;
    }

    public void Init(GameObject gameManagerGameObj)
    {
        Debug.Log("Levels PlayerControlllers Ïnit called ");

        gameManagerGameObject = gameManagerGameObj;
        FindManagers();

        // Detach TargetPoint as child object from players Character.
        TargetPoint.parent = null;      
    }
    public void ClearSpriteToGameObjectDict()
    {
        if (!Box) Box.GetComponent<Box>().ClearSpriteToGameObjectDict();
        else Debug.Log("Box needs to be assigned in editor to Player Controller.");
    }

    public void EnablePlayerTostartToPlay()
    {
        hasStartedPlaying = true;
    }

    public bool IsAllBoxesOnTargets()
    {
        if (Box) return tileMapManager.IsAllBoxesOnTargets();
        else
        {
            Debug.Log("Assigne one of the boxes to PlayerController in the editor.");
            return false;
        }
    }

    void DropPushedBox(GameObject pushedBox)
    {
        if (pushedBox.GetComponent<Box>())
        {
            pushedBox.transform.SetParent(levelManager.BoxesAndCharacter.transform);
        }
        else Debug.Log("Assigne pushedBox in PlayerController:DropPushedBox.");
    }

    bool RemoveBoxFromTarget(GameObject pushedBox)
    {
        Vector3Int cellPosition = levelManager.GoalTargets.WorldToCell(pushedBox.transform.position);
        Sprite sprite = levelManager.GoalTargets.GetSprite(cellPosition);

        if (sprite)
        {
            // Check for Target sprite in cell.
            if (sprite.name.Equals(targetSpriteName))
            {
                // Debug.Log("Got that one right!");
                pushedBox.GetComponent<Box>().SetOnTarget(true);
            }
            else
            {
                pushedBox.GetComponent<Box>().SetOnTarget(false);
            }
            return true;
        }
        else
        {
            Debug.Log("RemoveBoxFromTarget function in PlayerController return false Sprite from positon null.");
            return false;
        }
    }
bool IsBoxOnTarget(GameObject pushedBox)
    {
        Vector3Int cellPosition = levelManager.GoalTargets.WorldToCell(pushedBox.transform.position);
        Sprite sprite = levelManager.GoalTargets.GetSprite(cellPosition);

        if (sprite)
        {
            // Check for Target sprite in cell.
            if (sprite.name.Equals(targetSpriteName))
            {
               // Debug.Log("Got that one right!");
                pushedBox.GetComponent<Box>().SetOnTarget(true);
                return true;
            }
            else
            {
                pushedBox.GetComponent<Box>().SetOnTarget(false);
                return false;
            }
        }
        else return false;
    }

    void Move(Vector3 direction)
    {
        float checkDistance = 1f;
        Vector3 usedVector; 

        Sprite justInFront;
        Sprite isBoxInFrontOfPushedBox;
        Sprite isWallInFrontOfPushedBox;

        if (direction == new Vector3(1f, 0f, 0f))
        {
            usedVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        }
        else
        {
            usedVector = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
        }

        if (transform.childCount == 0) // Player is not pushing a box.
        {
            // Check for obstruction.
            justInFront = levelManager.StaticObsticals.GetSprite(levelManager.StaticObsticals.WorldToCell(TargetPoint.position + usedVector));

            if (!justInFront) // Ok, no obstruction -> Let's check for a box in front.
            {
                Vector3Int vector = levelManager.BoxesAndCharacter.WorldToCell(TargetPoint.position + usedVector);
                justInFront = levelManager.BoxesAndCharacter.GetSprite(vector);

                if (justInFront) // Ok, a box ahead.
                {
                    // Check first if there is another box or obstruction on the other side of the box, before adding box to player character.

                    // Set look ahead distance.
                    checkDistance = 2f;
                   
                    if (!levelManager.StaticObsticals.GetSprite(levelManager.StaticObsticals.WorldToCell(TargetPoint.position + checkDistance * usedVector)) && !levelManager.BoxesAndCharacter.GetSprite(levelManager.BoxesAndCharacter.WorldToCell(TargetPoint.position + checkDistance * usedVector)))
                    {
                        GameObject spriteGameObject = Box.GetComponent<Box>().GetGameObjectInTile(justInFront.GetInstanceID());

                        if (spriteGameObject)
                        {
                            // Add box for player character to push.
                            spriteGameObject.transform.SetParent(transform);
                            // Remove activation sprite (a almost transparent sprite tile behind the box made with a GameObject (that has box sprite in its Sprite Rendrer
                            tileMapManager.MoveTriggerSprite(vector, usedVector, transform.position); // Todo: check that we used the correct transform.
                        }
                        else Debug.Log("PlayerController->Move GetGameObjectInTile() did return a sprite GameObject.");
                    }
                    else // Ok, box and then obstruction.
                    {
                        // So, stop going in this direction.
                        usedVector = new Vector3(0f, 0f, 0f);
                        checkDistance = 1f;
                    }
                }
            }
            else
            {
                // Static obstruction in the way. Character can't go in this direction.
                usedVector = new Vector3(0f, 0f, 0f);
            }
        }
        else // Maybe the player is pushing a box, if so double the distance.
        {
            // If player is pushing the box from a box target, decrease the target count.
            GameObject pushedBox = gameObject.transform.GetChild(0).gameObject;
            if (pushedBox) RemoveBoxFromTarget(pushedBox); // It may or may not be pushing. Be a child of player. 

            // First check that we have a box in front and not on the side of, or else drop "pushed" box.
            justInFront = levelManager.BoxesAndCharacter.GetSprite(levelManager.BoxesAndCharacter.WorldToCell(TargetPoint.position + usedVector));

            if (!justInFront) // justInFront
            {
                pushedBox = gameObject.transform.GetChild(0).gameObject;
                if (pushedBox)
                {
                    DropPushedBox(pushedBox);
                }
                checkDistance = 1f;
            }
            else // Yes, pushing a cube in front.
            {
                checkDistance = 2f;
                // Check what´s in front of the pushed box.

                // Check for walls in front of pushed box.
                isWallInFrontOfPushedBox = levelManager.StaticObsticals.GetSprite(levelManager.StaticObsticals.WorldToCell(TargetPoint.position + checkDistance * usedVector));
                // Check for other boxes in front of pushed box.
                isBoxInFrontOfPushedBox = levelManager.BoxesAndCharacter.GetSprite(levelManager.BoxesAndCharacter.WorldToCell(TargetPoint.position + checkDistance * usedVector));

                if (!isWallInFrontOfPushedBox || !isBoxInFrontOfPushedBox)
                {
                    // Player can't move further in this direction.
                    usedVector = new Vector3(0f, 0f, 0f);
                }
            }
        }
        TargetPoint.position += usedVector;
    }

    void Update()
    {
        // Game not initialized yet.
        if (!gameManager) return;

        // Horizontal user input move - figure out the direction on the x axis.
        float horizontalUserInput = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        float verticalUserInput = Mathf.Abs(Input.GetAxisRaw("Vertical"));

        if (horizontalUserInput == 1f || verticalUserInput == 1f)
        {
            // Start timer when player starts to move around character.
            if (!hasStartedPlaying)
            {
                gameManager.StartPlaying();
                hasStartedPlaying = true;
            }
        }

        // Game Manager not ready yet or level halted.
        if (!gameManager.HasLevelStarted()) return; // Todo: || getLevelNo()) return; // Check if it´s level one.

        // Choosen not to use a coroutine for moving. Timer.cs uses a coroutine.
        // Move character every frame if needed.
        if (Vector3.Distance(transform.position, TargetPoint.position) >= .01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPoint.position, speed * Time.deltaTime);
        }
        else // Character has reached a target position.
        {
            // Adjust to position of character when it arrives at a target position.
            transform.position = transform.position;

            // Is character pushing?
            if (transform.childCount >= 1)
            {
                // Release any child box objects that we where pushing as we arrived.
                GameObject pushedBox = gameObject.transform.GetChild(0).gameObject;

                if (pushedBox)
                {
                    DropPushedBox(pushedBox);

                    if (IsBoxOnTarget(pushedBox))
                    {
                        // Great, one is right, now check all.
                        if (IsAllBoxesOnTargets())
                        {
                            gameManager.AllBoxesArePlaced(); 
                        }
                    }
                }
            }
            // Horisontal user input move - figure out the direction on the x and y axis.
            if (horizontalUserInput == 1f) Move(new Vector3(1f, 0f, 0f));
            else if (verticalUserInput == 1f) Move(new Vector3(0f, 1f, 0f));
        }
    }
}