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
        Dropdown dropdown = DropDownMenu.GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
    }

    // Drop Ddown menu listner function
    void DropdownValueChanged(Dropdown change)
    {
        int selected;
        // There is a blank row with a " " in the drop down because OnChangeSelected needs at least two options in drop down to work.
        if (change.value == 0)
        {
            Debug.Log("change.value == 0");
            selected = change.value + 1;
            return;
        }
        else
        {
            selected = change.value;
        }
        // Use trim to trim away charcters from string like digits. 
        //Debug.Log("The Level scene name before trim: " + SceneManager.GetActiveScene().name);
        char[] charsToTrim = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        string sceneNameWithNoDigits = SceneManager.GetActiveScene().name.TrimEnd(charsToTrim);

        string selectedLevel = sceneNameWithNoDigits + selected.ToString();

        RecordTimeText.SetActive(true);
        string recordTimeText = GetComponent<Timer>().ElapsedTimeFloatToString(GetComponent<StoreManager>().GetTimeRecordOfLevel(selectedLevel));
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

        dropdown.ClearOptions(); // Todo: This function doesn't clear the options.
        if (DropDownMenu.activeSelf == false) DropDownMenu.SetActive(true);
        dropdown.ClearOptions(); // Todo: This function doesn't clear the options.
        dropOptions.Clear();

        foreach (KeyValuePair<string, float> dictLevelTimeRecord in GetComponent<StoreManager>().dictLevelTimeRecords)
        {
            // onValueChanged dont't work with just one level in drop down menu. No options to onValueChanged. So we add " "
            dropOptions.Add(" ");
            dropOptions.Add(dictLevelTimeRecord.Key);
            // Todo: delete this line for test only
        }
        dropdown.AddOptions(dropOptions);
        dropdown.SetValueWithoutNotify(100); // Todo: Try with out 
        dropdown.RefreshShownValue();
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
            RecordTimeText.SetActive(true);
            DropDownMenu.SetActive(true);
            NextLevelButton.SetActive(true);
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
