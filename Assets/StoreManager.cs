// Written by Lee Sandberg

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    const string FileNameForResults = "RecordResults";   // Level, time records stored.
    string dummyLevelName = "SokoLevel0";

    public static StoreManager instance;

    // Record times and levels stored.
    public Dictionary<string, float> dictLevelTimeRecords = new Dictionary<string, float>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Load record times for levels.
        LoadFromFile();  
    }

    public string GetDummyLevelName()
    {
        return dummyLevelName;
    }

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
        public List<LevelInfo> LevelsInfo;
    }

    public bool LoadFromFile()
    {
        LevelsInfoHolder classHolder = new LevelsInfoHolder();
        classHolder.LevelsInfo = new List<LevelInfo>();

        // Todo: Remove toolBar = new ToolBar();
        string jsontoolBar;
        string fileName = FileNameForResults + ".txt";
        string path = Application.persistentDataPath + "/" + fileName;

        // And the fileStream.close();
        if (File.Exists(path))
        {
            dictLevelTimeRecords.Clear();

            using (StreamReader reader = new StreamReader(path))
            {
                jsontoolBar = reader.ReadToEnd();

                if (jsontoolBar.Length > 2)
                {
                    classHolder = JsonUtility.FromJson<LevelsInfoHolder>(jsontoolBar);

                    for (int i = 0; i < classHolder.LevelsInfo.Count; i++)
                    {
                        dictLevelTimeRecords.Add(classHolder.LevelsInfo[i].level, classHolder.LevelsInfo[i].time);
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

    public void SaveToFile()
    {
        LevelInfo toolBar;
        List<LevelInfo> toolList = new List<LevelInfo>(); 
        string jsontoolBar;   

        string file = FileNameForResults + ".txt";
        string path = Application.persistentDataPath + "/" + file;

        LevelInfo[] dataArray = new LevelInfo[SceneManager.GetActiveScene().buildIndex];
        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

        int i = 0;

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in dictLevelTimeRecords)
        {
            toolBar = new LevelInfo();
            string name = dictLevelTimeRecord.Key;
            float time = dictLevelTimeRecord.Value;

            toolBar.level = name;
            toolBar.time = time;

            dataArray[i] = toolBar;
            toolList.Add(toolBar);

            i++;
        }

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            var variable = new LevelsInfoHolder() { LevelsInfo = toolList };
            jsontoolBar = JsonUtility.ToJson(variable);

            Debug.Log("json string: " + jsontoolBar);

            // Store value pair to file.
            writer.Write(jsontoolBar); 
        }
        // Todo: remove fileStream.Flush(); 
        fileStream.Close();
    }
}
