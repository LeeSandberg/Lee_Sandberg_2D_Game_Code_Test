// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    // Tilemap world layers.
    public Tilemap BoxesAndCharacter; // Todo: Perhaps break up character from boxes tile map.
    public Tilemap GoalTargets;
    public Tilemap StaticObsticals;

    private GameObject gameManagerGameObject;
    private GameManager gameManager;
    private TileMapManager tileMapManager;

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

    public void Init(GameObject gameManagerGameObj)
    {
        gameManagerGameObject = gameManagerGameObj;
        FindManagers();
    }
}

