using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip levelCompleteSound;
    public AudioClip levelFailSound;
    public AudioClip buttonClickSound;
    public AudioClip bgmClip;
    public AudioClip gameCompletedSound;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sourceSFX;

    [Header("Volumes")]
    public float volumeBGM = 1f;
    public float volumeCorrect = 1f;
    public float volumeWrong = 1f;
    public float volumeLevelComplete = 1f;
    public float volumeLevelFail = 1f;
    public float volumeButton = 1f;
    public float volumeGameCompleted = 1f;


    private void Awake()
    {
        if (bgmSource == null) Debug.LogError("BGM Source not assigned!");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup BGM Source
            //bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = volumeBGM; // Adjust as needed
        }
        else
        {
            Destroy(gameObject);
        }

        // Load volumes
        volumeBGM = PlayerPrefs.GetFloat("Volume_BGM", 1f);
        volumeCorrect = PlayerPrefs.GetFloat("Volume_Correct", 1f);
        volumeWrong = PlayerPrefs.GetFloat("Volume_Wrong", 1f);
        volumeLevelComplete = PlayerPrefs.GetFloat("Volume_Complete", 1f);
        volumeLevelFail = PlayerPrefs.GetFloat("Volume_Fail", 1f);
        volumeButton = PlayerPrefs.GetFloat("Volume_Button", 1f);
        volumeGameCompleted = PlayerPrefs.GetFloat("Volume_GameOver", 1f);

        bgmSource.volume = volumeBGM;
    }


    public void PlayCorrectSound()
    {
        sourceSFX.PlayOneShot(correctSound, volumeCorrect);
    }

    public void PlayWrongSound()
    {
        sourceSFX.PlayOneShot(wrongSound, volumeWrong);
    }

    public void PlayLevelCompleteSound()
    {
        sourceSFX.PlayOneShot(levelCompleteSound, volumeLevelComplete);
    }

    public void PlayLevelFailSound()
    {
        sourceSFX.PlayOneShot(levelFailSound, volumeLevelFail);
    }

    public void PlayButtonSound()
    {
        sourceSFX.PlayOneShot(buttonClickSound, volumeButton);
    }

    public void PlayGameCompletedSound()
    {
        sourceSFX.PlayOneShot(gameCompletedSound, volumeGameCompleted);
    }


    public void PlayBGM()
    {
        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    //update the volume Sliders

    public void SetVolume(string type, float value)
    {
        switch (type)
        {
            case "BGM":
                volumeBGM = value;
                bgmSource.volume = volumeBGM;
                PlayerPrefs.SetFloat("Volume_BGM", value);
                break;
            case "Correct":
                volumeCorrect = value;
                PlayerPrefs.SetFloat("Volume_Correct", value);
                break;
            case "Wrong":
                volumeWrong = value;
                PlayerPrefs.SetFloat("Volume_Wrong", value);
                break;
            case "Complete":
                volumeLevelComplete = value;
                PlayerPrefs.SetFloat("Volume_Complete", value);
                break;
            case "Fail":
                volumeLevelFail = value;
                PlayerPrefs.SetFloat("Volume_Fail", value);
                break;
            case "Button":
                volumeButton = value;
                PlayerPrefs.SetFloat("Volume_Button", value);
                break;
            case "GameOver":
                volumeGameCompleted = value;
                PlayerPrefs.SetFloat("Volume_GameOver", value);
                break;
        }

        PlayerPrefs.Save();
    }

    public void SetVolumeBGM(float value) => SetVolume("BGM", value);
    public void SetVolumeCorrect(float value) => SetVolume("Correct", value);
    public void SetVolumeWrong(float value) => SetVolume("Wrong", value);
    public void SetVolumeLevelComplete(float value) => SetVolume("Complete", value);
    public void SetVolumeLevelFail(float value) => SetVolume("Fail", value);
    public void SetVolumeButton(float value) => SetVolume("Button", value);
    public void SetVolumeGameCompleted(float value) => SetVolume("GameOver", value);



}
