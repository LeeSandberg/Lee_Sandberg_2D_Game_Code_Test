// Written by Lee Sandberg
// © Copyright 2021 Lee Sandberg
// Lee.Sandberg@gmail.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Todo: Hook up in editor.
    public Button NextLevelButton;  
    public Dropdown DropDownMenu;     
    public Text RecordTimeText;   
    public Text TimeText;
    public Button MenuButton;
    public Button ResetButton;
    public Text GameOverText;
    public Button QuitButton;

    private void Awake()
    {
        DropDownMenu.gameObject.GetComponent<Dropdown>().value = -1;
        // Todo: add RemoveListner when it quits or loads level.
        DropDownMenu.gameObject.SetActive(false);
        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        // Todo: It´s connected from the editor as well, fix.
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);        
        });
    }

    // Drop down menu listner function.
    public void DropdownValueChanged(Dropdown change)
    {   
        // Unity doesn't support ONE option in Dropdown menu, so just present the time.
        if (GetComponent<GameManager>().GetCurrentScene() == 1 && change.options.Count < 1) return;

        string levelName = change.options[change.value].text;
        float levelTime = GetComponent<StoreManager>().GetTimeRecordOfLevel(levelName);
        string recordTimeText = GetComponent<Timer>().ElapsedTimeFloatToString(levelTime);
        RecordTimeText.gameObject.SetActive(true);
        RecordTimeText.GetComponent<Text>().text = recordTimeText;
    }

    // UI Next Level button. DON'T remove.
    public void NextLevel()
    {
        GetComponent<GameManager>().NextLevel();     
    }

    // UI Next Level button. DON'T remove.
    public void ResetLevel()
    {
        GetComponent<GameManager>().ResetLevel();
    }

    // UI Quit button. DON'T remove.
    public void QuitGame()
    {
        HideAllUserInterfaces();
        GetComponent<GameManager>().QuitGame();
    }

    // UI Menu button. DON'T remove.
    public void DisplayDropDownMenu()
    {
        // Unity doesn't support ONE option in Dropdown menu, so just present the time.
        if (GetComponent<GameManager>().GetCurrentScene() == 1 && !GetComponent<GameManager>().HasLevelStarted()) 
        {
            string recordTimeText;
            float levelTime = GetComponent<StoreManager>().GetTimeRecordOfLevel("SokoLevel1");

            recordTimeText = GetComponent<Timer>().ElapsedTimeFloatToString(levelTime);
            RecordTimeText.gameObject.SetActive(true);
            RecordTimeText.GetComponent<Text>().text = recordTimeText;

            return;
        }

        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        List<string> dropOptions = new List<string>();

        if (DropDownMenu.gameObject.activeSelf == false) DropDownMenu.gameObject.SetActive(true);
        dropdown.ClearOptions();
        dropOptions.Clear();

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in GetComponent<StoreManager>().dictLevelTimeRecords)
        {
           dropOptions.Add(dictLevelTimeRecord.Key);
        }
        dropdown.AddOptions(dropOptions);
        dropdown.RefreshShownValue();
        DropDownMenu.GetComponent<Dropdown>().value = -1; // Todo: remove if not needed.   
    }

    public void SetNextLevelButtonActive(bool active)
    {
        NextLevelButton.gameObject.SetActive(active);
    }

    public void HideAllUserInterfaces()
    {
        MenuButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);
        DropDownMenu.gameObject.SetActive(false);
        TimeText.gameObject.SetActive(false);
        RecordTimeText.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    public void SetMenuButtonActive(bool state)
    {
        MenuButton.gameObject.SetActive(state);
    }

    public void SetResetButtonActive(bool state)
    {
        ResetButton.gameObject.SetActive(state);
    }

    public void SetDropDownMenuActive(bool state)
    {
        DropDownMenu.gameObject.SetActive(state);
    }

    public void SetTimeTextActive(bool state)
    {
        TimeText.gameObject.SetActive(state);
    }

    public void SetRecordTimeTextActive(bool state)
    {
        RecordTimeText.gameObject.SetActive(state);
    }

    public void SetQuitButtonActive(bool state)
    {
        QuitButton.gameObject.SetActive(state);
    }

    public void SetGameOverTextActive(bool state)
    {
       GameOverText.gameObject.SetActive(state);
    }

    public void HideButtonsAndMenus(bool hide)
    {
        if (hide)
        {
            RecordTimeText.gameObject.SetActive(false);
            DropDownMenu.gameObject.SetActive(false);
            NextLevelButton.gameObject.SetActive(false);
            GameOverText.gameObject.SetActive(false);
        }
    }

    public void HideGameOverText(bool hide)
    {
        if (hide)
        {
            GameOverText.gameObject.SetActive(false);
        }
        else
        {
            GameOverText.gameObject.SetActive(false);
        }
    }

    public void QuitMenu()
    {
        #if UNITY_EDITOR
            DropDownMenu.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            DropDownMenu.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
            Application.Quit();
        #endif
    }
}
