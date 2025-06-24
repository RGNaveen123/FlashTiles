using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameModeSettings : MonoBehaviour
{
    public Toggle toggleSpeedRun;
    public Toggle toggleTimer;
    public Slider sliderTimer;
    public TextMeshProUGUI timerValueText;
    public Slider sliderLives;
    public TextMeshProUGUI livesValueText;

    void Start()
    {
        // Load saved or default values
        toggleSpeedRun.isOn = PlayerPrefs.GetInt("EnableSpeedRun", 1) == 1;
        toggleTimer.isOn = PlayerPrefs.GetInt("EnableTimer", 1) == 1;

        sliderTimer.value = PlayerPrefs.GetFloat("TimerDuration", 15f);
        sliderLives.value = PlayerPrefs.GetInt("NumLives", 3);

        UpdateTimerText(sliderTimer.value);
        UpdateLivesText(sliderLives.value);

        // Hook up changes
        sliderTimer.onValueChanged.AddListener(UpdateTimerText);
        sliderLives.onValueChanged.AddListener(UpdateLivesText);
    }

    public void UpdateTimerText(float val)
    {
        timerValueText.text = val.ToString("F0") + "s";
    }

    public void UpdateLivesText(float val)
    {
        livesValueText.text = ((int)val).ToString();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("EnableSpeedRun", toggleSpeedRun.isOn ? 1 : 0);
        PlayerPrefs.SetInt("EnableTimer", toggleTimer.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("TimerDuration", sliderTimer.value);
        PlayerPrefs.SetInt("NumLives", (int)sliderLives.value);
        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        AudioManager.Instance.PlayButtonSound();
        toggleSpeedRun.isOn = true;
        toggleTimer.isOn = true;
        sliderTimer.value = 15f;
        sliderLives.value = 3;
        UpdateTimerText(15f);
        UpdateLivesText(3);
    }

    public void CloseSettings()
    {
        SaveSettings();
        AudioManager.Instance.PlayButtonSound();
        gameObject.SetActive(false);
    }
}
