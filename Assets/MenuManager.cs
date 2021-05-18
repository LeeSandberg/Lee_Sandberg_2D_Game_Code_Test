// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Todo: Hook up in editor.
    public GameObject NextLevelButton;  // Todo: use type Button.
    public GameObject DropDownMenu;     // Todo: use Dropdown.
    public GameObject RecordTimeText;   // Todo: use Text.
    public GameObject TimeText;
    public GameObject ButtonMenu;
    public GameObject ResetButton;
    public GameObject GameOverText;

    private void Awake()
    {
        DropDownMenu.GetComponent<Dropdown>().value = -1;
        // Todo: add RemoveListner when it quits or loads level.
        DropDownMenu.SetActive(false);
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
        RecordTimeText.SetActive(true);
        RecordTimeText.GetComponent<Text>().text = recordTimeText;
    }

    // Next Level button. DON'T remove.
    public void NextLevel()
    {
        GetComponent<GameManager>().NextLevel();     
    }

    // Next Level button. DON'T remove.
    public void ResetLevel()
    {
        GetComponent<GameManager>().ResetLevel();
    }

    // Quit button. DON'T remove.
    public void QuitGame()
    {
        GetComponent<GameManager>().QuitGame();
    }

    // Menu button. DON'T remove.
    public void DisplayDropDownMenu()
    {
        // Unity doesn't support ONE option in Dropdown menu, so just present the time.
        if (GetComponent<GameManager>().GetCurrentScene() == 1 && !GetComponent<GameManager>().HasLevelStarted()) 
        {
            string recordTimeText;
            float levelTime = GetComponent<StoreManager>().GetTimeRecordOfLevel("SokoLevel1");

            recordTimeText = GetComponent<Timer>().ElapsedTimeFloatToString(levelTime);
            RecordTimeText.SetActive(true);
            RecordTimeText.GetComponent<Text>().text = recordTimeText;

            return;
        }

        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        List<string> dropOptions = new List<string>();

        if (DropDownMenu.activeSelf == false) DropDownMenu.SetActive(true);
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

    public void RemoveAllDropDownListners()
    {
        DropDownMenu.GetComponent<Dropdown>().onValueChanged.RemoveAllListeners();
    }

    public void SetNextLevelButtonActive(bool active)
    {
        NextLevelButton.SetActive(active);
    }

    public void SetLevelButtonActive(bool active)
    {
        NextLevelButton.SetActive(active);
    }

    public void SetGameOverTextActive(bool active)
    {
        NextLevelButton.SetActive(active); 
    }

    public void HideButtonsAndMenus(bool hide)
    {
        if (hide)
        {
            RecordTimeText.SetActive(false);
            DropDownMenu.SetActive(false);
            NextLevelButton.SetActive(false);
            GameOverText.SetActive(false);
        }
    }

    public void HideGameOverText(bool hide)
    {
        if (hide)
        {
            GameOverText.SetActive(false);
        }
        else
        {
            GameOverText.SetActive(false);
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
