// Written by Lee Sandberg
// © Copyright 2021 Lee Sandberg
// Lee.Sandberg@gmail.com

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    const string FileNameForResults = "RecordResults";   // Level, time records stored.

    // Record times and levels stored.
    public Dictionary<string, float> dictLevelTimeRecords = new Dictionary<string, float>();

    StoreManager.LevelsInfoHolder levelsInfoHolder;

    public float GetTimeRecordOfLevel(string levelName) 
    {
        return dictLevelTimeRecords[levelName];
    }

    public void StoreResultInDictionary()
    {
        string sceneName = SceneManager.GetActiveScene().name;   
        float time = GetComponent<Timer>().GetElapsedTimeAsFloat();

        // Ckeck to see if this level is in the list. If not add it and the new elapsed time.
        if (dictLevelTimeRecords.ContainsKey(sceneName))
        {
            // If the new record time is faster replace the old time.
            if (dictLevelTimeRecords[sceneName] > time)
            {
                dictLevelTimeRecords[sceneName] = time;
            }
        }
        else // Ok, this level is new, so add it and its record time.
        {
            dictLevelTimeRecords.Add(sceneName, time);
        }
    }

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
    }

    public bool LoadFromFile()
    {
        LevelsInfoHolder classHolder = new LevelsInfoHolder();

        string jsontoolBar;
        string fileName = FileNameForResults + ".txt";
        string path = Application.persistentDataPath + "/" + fileName;


        if (File.Exists(path))
        {
            dictLevelTimeRecords.Clear();

            using (StreamReader reader = new StreamReader(path))
            {
                jsontoolBar = reader.ReadToEnd();

                if (jsontoolBar.Length > 2)
                {
                    classHolder = JsonUtility.FromJson<LevelsInfoHolder>(jsontoolBar);

                    int length = classHolder.LevelsInfo.Length;

                    for (int i = 0; i < length; i++)
                    {
                        if (classHolder.LevelsInfo[i].time != 0f)
                        {
                            dictLevelTimeRecords.Add(classHolder.LevelsInfo[i].level, classHolder.LevelsInfo[i].time);
                            InsertLoadedGamesStats(classHolder.LevelsInfo[i]);
                        }
                    }
                }
            }
            return true;
        }
        else
        {
            // Debug.Log("File for stored record results, not found ! " + path);
            return false;
        }
    }

    // Missing Unity API feature: Get the name from levels in build index.
    // Todo: Store these once at game start up and reuse. (Levelnames, build index) because this goes out on file, slow.
    // Todo: May only work on PC.
    private static string GetLevelNameFromIndex(int buildIndex)
    {
        string levelPath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        int slash = levelPath.LastIndexOf('/');
        string levelName = levelPath.Substring(slash + 1);
        int dot = levelName.LastIndexOf('.');
        return levelName.Substring(0, dot);
    }

    // Missing Unity API feature: Get the index from level name in build index.
    // Todo: Store these once at game start up and reuse. (Levelnames, build index) because this goes out on file, slow.
    // Todo: May only work on PC.
    private int GetLevelIndexFromName(string levelName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            string testedLevel = GetLevelNameFromIndex(i);
            //Debug.log("sceneIndexFromName: i: " + i + " leveleName = " + testedLevel);
            if (testedLevel == levelName)
                return i;
        }
        return -1;
    }

    public void SaveToFile()
    {
        LevelInfo toolBar;
        LevelInfo[] toolArray = new LevelInfo[SceneManager.sceneCountInBuildSettings];
        string jsontoolBar;   

        string file = FileNameForResults + ".txt";
        string path = Application.persistentDataPath + "/" + file;

        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

        int i = 0;

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in dictLevelTimeRecords)
        {
            toolBar = new LevelInfo();

            toolBar.level = dictLevelTimeRecord.Key;
            toolBar.time = dictLevelTimeRecord.Value;
            toolBar.levelBuildIndex = GetLevelIndexFromName(toolBar.level);

            LevelInfo levelInfo = GetLevelInfo(GetLevelIndexFromName(toolBar.level));
            toolBar.numberTimesPlayed = levelInfo.numberTimesPlayed;

            toolArray[i] = toolBar;
            i++;
        }

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            LevelsInfoHolder variable = new LevelsInfoHolder() { LevelsInfo = toolArray };
            jsontoolBar = JsonUtility.ToJson(variable);

            Debug.Log("json string: " + jsontoolBar);

            // Store value pair to file.
            writer.Write(jsontoolBar); 
        }
        // Todo: remove fileStream.Flush(); 
        fileStream.Close();
    }

    private void Start()
    {
        levelsInfoHolder = new StoreManager.LevelsInfoHolder();
        levelsInfoHolder.LevelsInfo = new StoreManager.LevelInfo[SceneManager.sceneCountInBuildSettings];

        // Make sure time is huge on all of them.
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            StoreManager.LevelInfo levelInfo = new StoreManager.LevelInfo();
            levelsInfoHolder.LevelsInfo[i] = levelInfo;
        }
        // Load record times for levels.
        LoadFromFile();
    }

    public void InsertLoadedGamesStats(StoreManager.LevelInfo levelInfo)
    {
        int index = levelInfo.levelBuildIndex;
        levelsInfoHolder.LevelsInfo[index] = levelInfo;
    }

    public void InsertLevelsInfoFromPlayedGame()
    {
        string  level = SceneManager.GetActiveScene().name;
        int     index = SceneManager.GetActiveScene().buildIndex;
        float   time  = GetComponent<Timer>().GetElapsedTimeAsFloat();

        // Keep track of number of times level been played.
        levelsInfoHolder.LevelsInfo[index].numberTimesPlayed++;
        levelsInfoHolder.LevelsInfo[index].level = SceneManager.GetActiveScene().name;

        // Has level been played before.
        if (levelsInfoHolder.LevelsInfo[index].numberTimesPlayed > 0)
        { 
            // Level has been played before.

            // Check if the new time is faster than previus played, replace time.
            if (levelsInfoHolder.LevelsInfo[index].time >= time)
            {
                levelsInfoHolder.LevelsInfo[index].time = time;
            } 
            // Just keep the old time...
        }
        else  // New level. 
        {
            levelsInfoHolder.LevelsInfo[index].time = time;
        }
    }

    public StoreManager.LevelInfo GetLevelInfo(int level)
    {
        if (levelsInfoHolder == null) Debug.Log("StoreManager.LevelInfo GetLevelInfo : levelsInfoHolder == null ");
        if (levelsInfoHolder.LevelsInfo[level] == null) Debug.Log("StoreManager.LevelInfo GetLevelInfo : LevelsInfo[level] == null");
       
        return levelsInfoHolder.LevelsInfo[level];
    }
}
