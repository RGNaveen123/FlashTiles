using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider sliderBGM;
    public Slider sliderCorrect;
    public Slider sliderWrong;
    public Slider sliderLevelComplete;
    public Slider sliderLevelFail;
    public Slider sliderButtonClick;
    public Slider sliderGameCompleted;

    void Start()
    {
        // Load saved values into sliders
        sliderBGM.value = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        sliderCorrect.value = PlayerPrefs.GetFloat("Volume_Correct", 1f);
        sliderWrong.value = PlayerPrefs.GetFloat("Volume_Wrong", 1f);
        sliderLevelComplete.value = PlayerPrefs.GetFloat("Volume_Complete", 1f);
        sliderLevelFail.value = PlayerPrefs.GetFloat("Volume_Fail", 1f);
        sliderButtonClick.value = PlayerPrefs.GetFloat("Volume_Button", 1f);
        sliderGameCompleted.value = PlayerPrefs.GetFloat("Volume_GameOver", 1f);

        // Link sliders to audio manager
        sliderBGM.onValueChanged.AddListener(AudioManager.Instance.SetVolumeBGM);
        sliderCorrect.onValueChanged.AddListener(AudioManager.Instance.SetVolumeCorrect);
        sliderWrong.onValueChanged.AddListener(AudioManager.Instance.SetVolumeWrong);
        sliderLevelComplete.onValueChanged.AddListener(AudioManager.Instance.SetVolumeLevelComplete);
        sliderLevelFail.onValueChanged.AddListener(AudioManager.Instance.SetVolumeLevelFail);
        sliderButtonClick.onValueChanged.AddListener(AudioManager.Instance.SetVolumeButton);
        sliderGameCompleted.onValueChanged.AddListener(AudioManager.Instance.SetVolumeGameCompleted);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        gameObject.SetActive(true);
    }
}
