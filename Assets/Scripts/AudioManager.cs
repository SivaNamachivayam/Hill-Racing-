using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Button musicButton;  // Reference to the Music Button
    public Button soundButton;  // Reference to the Sound Button

    private int switchStatus = 1;

    public AudioSource musicSource; // Background Music AudioSource
    public AudioSource[] soundEffects; // Array for Sound Effects

    private bool isMusicOn;
    private bool isSoundOn;

    private void Start()
    {
        // Load saved settings from PlayerPrefs
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSoundOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        // Apply saved settings
        ToggleMusic(isMusicOn);
        ToggleSound(isSoundOn);

        // Add button click listeners
        musicButton.onClick.AddListener(() => ToggleMusic(!isMusicOn));
        soundButton.onClick.AddListener(() => ToggleSound(!isSoundOn));
    }

    public void ToggleMusic(bool isOn)
    {
        isMusicOn = isOn;

        if (musicSource != null)
            musicSource.mute = !isMusicOn;

        // Save the setting
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        // Update button UI (optional)
        //UpdateButtonUI();
    }

    public void ToggleSound(bool isOn)
    {
        isSoundOn = isOn;

        foreach (var sound in soundEffects)
        {
            if (sound != null)
                sound.mute = !isSoundOn;
        }

        // Save the setting
        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        // Update button UI (optional)
        //UpdateButtonUI();
    }

   /* private void UpdateButtonUI()
    {
        // Change button colors or images to indicate ON/OFF state
        if (isMusicOn)
        {
            musicButton.GetComponentInChildren<Text>().text = isMusicOn ? "ON" : "OFF";

        }
        else
        {
            soundButton.GetComponentInChildren<Text>().text = isSoundOn ? "ON" : "OFF";

        }
    }*/

    public void SwitchMusicButtonClick()
    {
        musicButton.transform.localPosition = new Vector3(-musicButton.transform.localPosition.x, musicButton.transform.localPosition.y,0f);
        switchStatus = (int)Mathf.Sign(musicButton.transform.localPosition.x);
        Debug.Log("switch status" + switchStatus);
    }
    
    public void SwitchSoundButtonClick()
    {
        soundButton.transform.localPosition = new Vector3(-soundButton.transform.localPosition.x, soundButton.transform.localPosition.y,0f);
        switchStatus = (int)Mathf.Sign(soundButton.transform.localPosition.x);
        Debug.Log("switch status" + switchStatus);
    }
}