using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    // reference to volume mixer
    public AudioMixer audioMixer;

    //Dropdown menu for assistance/hindrance
    public Dropdown assistanceHindranceDropdown;
    string selectedDropDown;
    public bool debuggerMode = false;

    // List to populate the dropdown
    List<string> AH_dropdown = new List<string>();

    // boolean for DayLightToggle
    public bool dayLightToggle = false;

    private void Start()
    {
        // Default Game Settings
        PlayerPrefs.SetString("DebuggerMode", debuggerMode.ToString());
        PlayerPrefs.SetString("DayLight", dayLightToggle.ToString());
        SetGameSupportOptions();
    }
    private void Update()
    {
        if(GameObject.Find("BackgroundMusicController") != null)
        {
            Destroy(GameObject.Find("BackgroundMusicController"));
        }
    }
    public void PlayGame()
    {
        BackButtonPressed();
        StartScreenScript.gameInitialised = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }	
    public void QuitGame()
    {
        //Debug.Log("Game quit!");
        Application.Quit();
    }
    public void SetVolume(float volume)
    {
        //Debug.Log(volume);
        audioMixer.SetFloat("masterVolume", volume);
    }
    private void SetGameSupportOptions()
    {
        assistanceHindranceDropdown.ClearOptions();
        AH_dropdown.Add("High Assistance");
        AH_dropdown.Add("Low Assistance");
        AH_dropdown.Add("Neutral");
        AH_dropdown.Add("Low Hindrance");
        AH_dropdown.Add("High Hindrance");
        assistanceHindranceDropdown.AddOptions(AH_dropdown);
    }
    public void DropDownSeletion()
    {
        assistanceHindranceDropdown.RefreshShownValue(); // refresh the value in the dropdown before selection
        selectedDropDown = assistanceHindranceDropdown.GetComponentInChildren<Text>().text.ToString(); // get the selected dropdown in string
        PlayerPrefs.SetString("GameSupport", selectedDropDown);
    }
    public void DebuggerCheckMark()
    {
        debuggerMode = !debuggerMode;
        PlayerPrefs.SetString("DebuggerMode", debuggerMode.ToString());
        //Debug.Log(debuggerMode);
    }
    public void DayLightTogglePressed()
    {
        dayLightToggle = !dayLightToggle;
        PlayerPrefs.SetString("DayLight", dayLightToggle.ToString());
        //Debug.Log(PlayerPrefs.GetString("DayLight"));
    }
    public void BackButtonPressed()
    {
        DropDownSeletion();
    }
}
