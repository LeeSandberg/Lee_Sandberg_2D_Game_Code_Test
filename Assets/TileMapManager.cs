// Written by Lee Sandberg

// Using Unity Tilemap with https://docs.unity3d.com/Packages/com.unity.2d.tilemap.extras@1.6/manual/index.html

// Tilemap extension is in preview and adds new brushes to Tile map palette. 
// Like a brush for drawing GameObjects with Sprite Renders in tiles.
//
// Why use it?
// Sprites in tiles or tiles can't have scripts, which GameObjects with sprite renders can.
//
// However the Tilemap API doesn't let us know what GameObject is in which tile.
// It just returns sprites in Tiles, but not the GameObject Sprite Render sprite in a tile.
//
// We are not allowed to use ray casting in this code test, which would solve this by projecting.
//
// The wokraround: Another sprite is places in the tile, under the GameObject Sprite render tile.
//
// Use getComponent<SpriteRender>().Sprite.getInstanceId(), sadly each tile in the tilemap doesn't have a uniqe id, 
// not even if from different tile pallet slots, so we need ten different similar sprites in assets folder,
// to get ten different IDs on the sprites.
// 
// A dictionary is used to pair these Sprite ID's with Tile map Gameobjects.
// Using a dictionary in Box.cs to map box sprites in Tilemap to GameObjects (we could try using spriteid -> GameObject)
// 
// !! Oops Make sure you don't use the same sprite behind a box twice in a level, or this will start behaving weird !!
// In the Tile palette they have them in a row, in the layer "boxes and charcter".
//
// Future work could use rule tiles in Tilemap extra. https://www.youtube.com/watch?v=Ky0sV9pua-E
//
// And dynamic 2D lights with normal maps on sprites. https://www.youtube.com/watch?v=48dsapjS55k
//
// Use of TileBase when create your own tile classes.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    private const string characterName = "character";

    // Specify the target sprite.
    public Sprite TargetSprite;

    private int boxesOnTargets = 0;
    private int numberOfTargets = 0;

    //[SerializeField]
    private PlayerController playerController;
    private LevelManager levelManager;

    public void Init(PlayerController playContr, LevelManager levManag)
    {
        playerController = playContr;
        levelManager = levManag;
        boxesOnTargets = 0;             
        numberOfTargets = 0;            

        if (!TargetSprite) Debug.Log("Assign the Target sprite in _preload scene.");
        else
        {
            int amount = GetTileAmountSprite(TargetSprite);
            SetNumberOfTargets(amount);
        }
    }

    public int GetNumberOfTargets()
    {
        return numberOfTargets;
    }

    public int GetNumberOfBoxesOnTargets()
    {
        return boxesOnTargets;
    }

    public void IncreaseBoxesOnTargets()
    {
        boxesOnTargets++;
    }

    public void DecreaseBoxesOnTargets() // Todo: Check that this one is used correctly.
    {
        boxesOnTargets--;
    }

    // Todo: Remove set below.
    public void SetNumberOfTargets(int nrOfTargets)
    {
        numberOfTargets = nrOfTargets;
    }


    //public bool IsBoxOnTarget(string spriteName)
    //{
    //    if (levelManager.GoalTargets == null) return false;

    //    Vector3Int tileCellPosition = levelManager.GoalTargets.WorldToCell(transform.position);

    //    Sprite spriteResult = levelManager.GoalTargets.GetSprite(tileCellPosition);

    //    if (spriteResult == null) return false; // No sprite in this tile.

    //    string nameResult = spriteResult.name;

    //    bool result = nameResult.Equals(spriteName);

    //    return result; // Yes, found sprite.
    //}

    public void MoveTriggerSprite(Vector3Int vector, Vector3 usedVector, Vector3 position)
    {
        TileBase tile = levelManager.BoxesAndCharacter.GetTile(vector);
        levelManager.BoxesAndCharacter.SetTile(vector, null);
        Vector3Int vector2 = levelManager.BoxesAndCharacter.WorldToCell(position + 2 * usedVector);
        levelManager.BoxesAndCharacter.SetTile(vector2, tile);
    }

    public bool IsAllBoxesOnTargets()
    {
        if (GetNumberOfTargets() == 0)
        {
            Debug.Log("You forgott box targets in Your level.");
            return false;
        }
        else
        {
            if (GetNumberOfBoxesOnTargets() == GetNumberOfTargets()) return true;
            else return false;
        }
    }

    // Tilemap API lacks this feature.
    public int GetTileAmountSprite(Sprite targetSprite)
    {
        int amount = 0;
        if (!levelManager.GoalTargets) Debug.Log("Assign Goal Tragets in edior for all levels. Or maybe there is no target sprites in a level.");
        else
        {
            // Loop through all of the tiles.        
            BoundsInt bounds = levelManager.GoalTargets.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                Tile tile = levelManager.GoalTargets.GetTile<Tile>(pos);
                if (tile)
                {
                    if (tile.sprite == targetSprite)
                    {
                        amount += 1;
                    }
                }
            }
            Debug.Log("Number of target sprites in level: " + amount);
        }
        return amount;
    }
}


