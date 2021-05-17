// Written by Lee Sandberg

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    const string FileNameForResults = "RecordResults";   // Level, time records stored.
   // const string dummyLevelName = "SokoLevel0";

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
        // LoadFromFile();  // Todo: Add this WHEN filee storing works or debugging will be tuff with corrupt data loaded..

        // Add a blank level and record. To avoid problems. Todo: Remove this.
        //dictLevelTimeRecords.Add(dummyLevelName, 0f);
    }
    
    public float GetTimeRecordOfLevel(string levelName) 
    {
        return dictLevelTimeRecords[levelName];
    }

    public void StoreResultInDictionary()
    {
        string sceneName = SceneManager.GetActiveScene().name;
     
        float time = GetComponent<Timer>().GetElapsedTimeAsFloat();

        // Ckeck to see if thsi level is in the list. if not add it and the new elapsed time.
        if (dictLevelTimeRecords.ContainsKey(sceneName))
        {
            // If the new record time is faster replace the old time.
            if (dictLevelTimeRecords[sceneName] > time)
            {
                dictLevelTimeRecords[sceneName] = time;
            }
        }
        else // Ok this level is new, so add it and its record time.
        {
            dictLevelTimeRecords.Add(sceneName, time);
        }
    }

    [System.Serializable]
    public class ToolBar
    {
        public string level;
        public float time;
    }

    public bool LoadFromFile()
    {
        ToolBar toolBar;
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
                    toolBar = JsonUtility.FromJson<ToolBar>(jsontoolBar);
                    dictLevelTimeRecords.Add(toolBar.level, toolBar.time);
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
        ToolBar toolBar;
        toolBar = new ToolBar();

        string jsontoolBar;

        string file = FileNameForResults + ".txt";
        string path = Application.persistentDataPath + "/" + file;

        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in dictLevelTimeRecords)
        {
            string name = dictLevelTimeRecord.Key;
            float time = dictLevelTimeRecord.Value;

            toolBar.level = name;
            toolBar.time = time;

            jsontoolBar = JsonUtility.ToJson(toolBar, true);
            Debug.Log("json string: " + jsontoolBar);

            // Store value pair to file
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(jsontoolBar); // Todo: Tried WriteLine before.
            }
        }
        // Todo: remove fileStream.Flush(); 
        fileStream.Close();
    }
}
