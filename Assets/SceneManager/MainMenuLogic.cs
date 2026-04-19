using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _volumeSlider;

    [SerializeField] TMP_Dropdown _resDropDown;
    [SerializeField] Toggle _fullScreenToggle;

    Resolution[] AllResolutions;
    bool IsFullScreen;
    int SelectedResolution = 0;
    List<Resolution> SelectedResolutionList = new List<Resolution>();
    
     private void Start()
    {

        ParseResolution();
        LoadSettings();

    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("musicVolume")){ LoadVolume(); }
        else { SetMusicVolume(); }
        if (PlayerPrefs.HasKey("IsFullScreen") && PlayerPrefs.HasKey("SelectedResolution")) { LoadResolution(); }
        else { ChangeResolution(); }
    }

    private void ParseResolution()
    {
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + 'x' + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                SelectedResolution += 1;
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
        }
        SelectedResolution -= 1;

        _resDropDown.AddOptions(resolutionStringList);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(EventSystem.current.currentSelectedGameObject.name);
    }

    public void LoadLevel(string levelName) //string levelName
    {
        SceneManager.LoadScene(levelName);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SetMusicVolume()
    {
        float volume = _volumeSlider.value;
        _audioMixer.SetFloat("Master", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume",  volume);
    }

    public void LoadVolume()
    {
        _volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();
    }

    public void LoadResolution()
    {
        _resDropDown.value = PlayerPrefs.GetInt("SelectedResolution");
        _fullScreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("IsFullScreen"));
        //Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);

    }

    public void ChangeResolution()
    {
        SelectedResolution = _resDropDown.value;

        IsFullScreen = _fullScreenToggle.isOn;

        PlayerPrefs.SetInt("SelectedResolution", SelectedResolution);
        PlayerPrefs.SetInt("IsFullScreen", IsFullScreen ? 1 : 0);

        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen); 
    }

}
