// Written by Lee Sandberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Todo: Hook up in editor.
    public GameObject NextLevelButton;  // Todo: use type Button
    public GameObject DropDownMenu;     // Todo: use Dropdown
    public GameObject RecordTimeText;   // Todo: use Text
    public GameObject TimeText;
    public GameObject ButtonMenu;
    public GameObject ResetButton;
    public GameObject GameOverText;

    private void Awake()
    {
        // Todo: add RemoveListner when it quits or loads level.
        // Todo: trying the trick with -1 in value in the inspector on Dropdown when having just one option.
        DropDownMenu.SetActive(false);
        DropDownMenu.GetComponent<Dropdown>().value = -1;
        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        // Todo: Its connected from the editor as well, fix.
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
            DropDownMenu.GetComponent<Dropdown>().value = -1;
        });
    }

    // Drop Ddown menu listner function
    public void DropdownValueChanged(Dropdown change)
    {
        // Todo: Check that we populated the drop down optinons correct.
        string recordTimeText = GetComponent<Timer>().ElapsedTimeFloatToString(GetComponent<StoreManager>().GetTimeRecordOfLevel(change.options.ToArray()[1].text));

        RecordTimeText.SetActive(true);
        RecordTimeText.GetComponent<Text>().text = recordTimeText;
    }

    // Next Level button
    public void NextLevel()
    {
        GetComponent<GameManager>().NextLevel();     
    }

    // Next Level button
    public void ResetLevel()
    {
        GetComponent<GameManager>().ResetLevel();
    }

    // Menu button
    public void DisplayDropDownMenu()
    {
        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        List<string> dropOptions = new List<string>();

        //Todo: Clean upp code bellow.
        dropdown.ClearOptions(); // Todo: This function doesn't clear the options.
        if (DropDownMenu.activeSelf == false) DropDownMenu.SetActive(true);
        dropdown.ClearOptions(); // Todo: This function doesn't clear the options.
        dropOptions.Clear();

        //if (GetComponent<StoreManager>().dictLevelTimeRecords.Count == 1)
        //{
        //    dropOptions.Add(" ");
        //}

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in GetComponent<StoreManager>().dictLevelTimeRecords)
        {
            dropOptions.Add(dictLevelTimeRecord.Key);
        }
        dropdown.AddOptions(dropOptions);
       // dropdown.SetValueWithoutNotify(100); // Todo: Try with out 
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
        else
        {
           // RecordTimeText.SetActive(true);
           // DropDownMenu.SetActive(true);
           // NextLevelButton.SetActive(true);
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

    public void QuitGame()
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
