using UnityEngine;

//BGM,SEÇÃä«óùÇÇ∑ÇÈÅ@èÌíì
public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance {  get; private set; }
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _seSource;
    [SerializeField] private AudioDatabase _audioDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGM(ClipName clipName)
    {
        AudioClip clip = _audioDatabase.GetClip(clipName);
        if (clip != null)
        {
            PlayBGM(clip);
        }
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        _bgmSource.clip = bgmClip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource?.Stop();
    }

    public void PlaySEOneShot(ClipName clipName)
    {
        AudioClip clip = _audioDatabase.GetClip(clipName);
        if (clip != null)
        {
            PlaySEOneShot(clip);
        }
    }

    public void PlaySEOneShot(AudioClip seClip)
    {
        _seSource.clip = seClip;
        _seSource.Play();
    }
}
