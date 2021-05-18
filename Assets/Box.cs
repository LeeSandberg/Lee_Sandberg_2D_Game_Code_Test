// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Box : MonoBehaviour
{
    // Assign this in editor
    public Tilemap BoxesAndCharacter;

    // Keep track of the box GameObjects with Sprite Renders in the tile map.
    public static Dictionary<int, GameObject> dictSpriteToGameObject = new Dictionary<int, GameObject>();
    public static Box instance; // Todo: Keep and use or remove.
    
    private bool onTarget = false;
    private const string managerObject = "ManagerObject";

    [SerializeField]
    private static TileMapManager tileMapManager;
   
    private bool FindManagers()
    {
        GameObject gameObject = GameObject.Find(managerObject);
        if (gameObject)
        {          
            tileMapManager = gameObject.GetComponent<TileMapManager>();         

            if (tileMapManager) return true;
            else return false;
        }
        else
        {
            Debug.Log("Box FindGameManager Manager");
            return false;
        }
    }

    public void SetOnTarget(bool flag)
    {
        // Make sure not to count things twice or more.
        if (onTarget && !flag)
        {
            tileMapManager.DecreaseBoxesOnTargets();
        }
        else if (!onTarget && flag)
        {
            tileMapManager.IncreaseBoxesOnTargets();
        }
        onTarget = flag;
    }

    // Called before Start()
    void Awake()
    {
        if (!instance) instance = this; // Todo: Keep and use or remove.    
        if (!tileMapManager) FindManagers();

        AddBoxGameObjectAndSprite(transform);
    }

    void OnDestroy() 
    { 
        if (this == instance) 
        {
            dictSpriteToGameObject.Clear(); // Todo: Remove if not needed.
            instance = null; 
        } 
    }

    public void AddBoxGameObjectAndSprite(Transform transform)
    {
            Sprite gameObjectsSprite = BoxesAndCharacter.GetSprite(BoxesAndCharacter.WorldToCell(transform.position));

            if (gameObjectsSprite)
            {
                // GetInstanceID() here is why we have several uniqe sprites in assets one for each box placed in the level.
                // Because there is no way to find the Box with Sprite render on in the tile map, besides ray casting, which we can't use, because of rules in breif.
                dictSpriteToGameObject.Add(gameObjectsSprite.GetInstanceID(), gameObject);
            }
            else
            {
                Debug.Log("Box GameObject is missing a SpriteRenderer or sprite");
            }
    }

    // Unity Tilemap API missing feature.
    public GameObject GetGameObjectInTile(int sprite)
    {
        return dictSpriteToGameObject[sprite];
    }

    public void ClearSpriteToGameObjectDict()
    {
        dictSpriteToGameObject.Clear();
    }
}




