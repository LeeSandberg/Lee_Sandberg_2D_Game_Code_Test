// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Tilemap world layers.
    // Todo: Perhaps break up character from boxes tile map.
    public Tilemap BoxesAndCharacter; 
    public Tilemap GoalTargets;
    public Tilemap StaticObsticals; 
}

