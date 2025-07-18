using UnityEngine;
using UnityEngine.UI;
using TMPro;                               // ← add

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

    [Header("Value Labels")]               // ← NEW
    public TextMeshProUGUI txtBGM;
    public TextMeshProUGUI txtCorrect;
    public TextMeshProUGUI txtWrong;
    public TextMeshProUGUI txtLevelComplete;
    public TextMeshProUGUI txtLevelFail;
    public TextMeshProUGUI txtButtonClick;
    public TextMeshProUGUI txtGameCompleted;

    void Start()
    {
        /* Load saved values ------------------------------------------------ */
        sliderBGM.value = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        sliderCorrect.value = PlayerPrefs.GetFloat("Volume_Correct", 1f);
        sliderWrong.value = PlayerPrefs.GetFloat("Volume_Wrong", 1f);
        sliderLevelComplete.value = PlayerPrefs.GetFloat("Volume_Complete", 1f);
        sliderLevelFail.value = PlayerPrefs.GetFloat("Volume_Fail", 1f);
        sliderButtonClick.value = PlayerPrefs.GetFloat("Volume_Button", 1f);
        sliderGameCompleted.value = PlayerPrefs.GetFloat("Volume_GameOver", 1f);

        /* Hook each slider to AudioManager PLUS a value-update ------------- */
        sliderBGM.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeBGM(v); UpdateValue(txtBGM, v); });
        sliderCorrect.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeCorrect(v); UpdateValue(txtCorrect, v); });
        sliderWrong.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeWrong(v); UpdateValue(txtWrong, v); });
        sliderLevelComplete.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeLevelComplete(v); UpdateValue(txtLevelComplete, v); });
        sliderLevelFail.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeLevelFail(v); UpdateValue(txtLevelFail, v); });
        sliderButtonClick.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeButton(v); UpdateValue(txtButtonClick, v); });
        sliderGameCompleted.onValueChanged.AddListener(v => { AudioManager.Instance.SetVolumeGameCompleted(v); UpdateValue(txtGameCompleted, v); });

        /* Show current values on first open -------------------------------- */
        RefreshAllValues();
    }

    /* ---------- helpers -------------------------------------------------- */
    void UpdateValue(TextMeshProUGUI label, float sliderVal)
    {
        if (label) label.text = Mathf.RoundToInt(sliderVal * 100f) + "%";
    }

    void RefreshAllValues()
    {
        UpdateValue(txtBGM, sliderBGM.value);
        UpdateValue(txtCorrect, sliderCorrect.value);
        UpdateValue(txtWrong, sliderWrong.value);
        UpdateValue(txtLevelComplete, sliderLevelComplete.value);
        UpdateValue(txtLevelFail, sliderLevelFail.value);
        UpdateValue(txtButtonClick, sliderButtonClick.value);
        UpdateValue(txtGameCompleted, sliderGameCompleted.value);
    }

    public void CloseSettings() => gameObject.SetActive(false);
    public void OpenSettings() => gameObject.SetActive(true);
}
