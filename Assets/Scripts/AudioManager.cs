using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource source;  // <-- make sure this is public
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip levelCompleteSound;
    public AudioClip levelFailSound;
    public AudioClip buttonClickSound;
    public AudioClip bgmClip;
    private AudioSource bgmSource;
    public AudioClip gameCompletedSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup BGM Source
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.volume = 0.5f; // Adjust as needed
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayCorrectSound()
    {
        Debug.Log("Playing correct sound");
        source.PlayOneShot(correctSound);
    }

    public void PlayWrongSound()
    {
        Debug.Log("Playing wrong sound");
        source.PlayOneShot(wrongSound);
    }

    public void PlayLevelCompleteSound()
    {
        source.PlayOneShot(levelCompleteSound);
    }

    public void PlayLevelFailSound()
    {
        source.PlayOneShot(levelFailSound);
    }

    public void PlayButtonSound()
    {
        Debug.Log("Playing Button Click Sound");
        source.PlayOneShot(buttonClickSound);
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

    public void PlayGameCompletedSound()
    {
        source.PlayOneShot(gameCompletedSound);
    }

}
