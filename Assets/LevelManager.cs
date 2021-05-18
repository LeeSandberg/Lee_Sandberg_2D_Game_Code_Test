// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class LevelInfo
{
    public int numberTimesPlayed;
    public string level;
    public int levelBuildIndex;
    public float time;
}

[System.Serializable]
public class LevelsInfoHolder
{
    public LevelInfo[] LevelsInfo;
   // public List<LevelInfo> LevelsInfo;
}

public class LevelManager : MonoBehaviour
{
    LevelsInfoHolder levelsInfoHolder = new LevelsInfoHolder();

    const int maxNoLevels = 100;
    int[] numberTimesLevelPlayed = new int[maxNoLevels];
    // Tilemap world layers.
    public Tilemap BoxesAndCharacter; // Todo: Perhaps break up character from boxes tile map.
    public Tilemap GoalTargets;
    public Tilemap StaticObsticals;

    private void Awake()
    {
        levelsInfoHolder.LevelsInfo = new LevelInfo[maxNoLevels]; //= new List<LevelInfo>();
        // Make sure time is huge on all of them.
        for (int i = 0; i < maxNoLevels; i++) levelsInfoHolder.LevelsInfo[i].time = 100000f;
    }

    public void InsertLoadeGamesStats(LevelInfo levelInfo)
    {
        int index = levelInfo.levelBuildIndex;
        // Check if the info exists.



        // Check if the time for that level is faster, if so replace it.
        if (levelsInfoHolder.LevelsInfo[index].time >= levelInfo.time)
        {

        }

        // Todo: Continue here.
        // Simpler if LevelsInfoHolder had an Array instead of List. Too index scenens that way. 
    }

    public LevelInfo GetGamesStats(int level)
    {
        return levelsInfoHolder.LevelsInfo[level];
    }
}

