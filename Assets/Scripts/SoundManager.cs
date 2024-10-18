using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private AudioSource effectSource;
    private AudioSource BGM;

    private void Awake()
    {
        Instance = this;
        effectSource = GetComponent<AudioSource>();
        if (transform.childCount > 0) 
            BGM = transform.GetChild(0).GetComponent<AudioSource>();
        else BGM = GetComponent<AudioSource>();

        // Keep instance from being destroyed
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Avoid duplicate sound instance
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        //ChangeEffectVolume(0);
        //ChangeBGMVolume(0);
    }

    public void PlaySound(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }
}
