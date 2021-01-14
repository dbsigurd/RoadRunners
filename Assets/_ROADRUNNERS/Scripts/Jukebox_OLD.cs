using System.Collections;
using UnityEngine;

// This version of the Jukebox caches all of the Instance objects' variables to their static counterparts, which are used in Static methods.
// The Instance is kept around as a singleton to ensure these static variables aren't overriden between scene reloads, allowing for uninterrupted music playback.

public class Jukebox_OLD : MonoBehaviour
{
    internal static Jukebox_OLD Instance { get; set; }

    [SerializeField] bool AutoplayOnStart;

    internal enum Side { aSide, bSide }
    internal Side CurrentSide = Side.aSide;
    static Side _currentSide;

    [SerializeField] AudioSource aSide;
    static AudioSource _aSide;

    [SerializeField] AudioSource bSide;
    static AudioSource _bSide;

    [SerializeField]
    [Range(0.001f, 0.05f)]
    float CrossfadeSpeed;
    static float _crossfadeSpeed;

    static AudioClip[] _playlist;
    static int _trackIndex = 0;
    [SerializeField] AudioClip[] Playlist;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        _currentSide = Instance.CurrentSide;
        _aSide = Instance.aSide;
        _bSide = Instance.bSide;
        _crossfadeSpeed = Instance.CrossfadeSpeed;
        _playlist = Instance.Playlist;

        _aSide.clip = _playlist[Random.Range(0, _playlist.Length - 1)];

        if (AutoplayOnStart)
            QueueTrack(_aSide.clip);
    }

    static internal void PlayNext()
    {
        if (_trackIndex == _playlist.Length - 1)
            _trackIndex = 0;
        else _trackIndex++;

        QueueTrack(_playlist[_trackIndex]);
    }

    static internal void PlayPrevious()
    {
        if (_trackIndex == 0)
            _trackIndex = _playlist.Length - 1;
        else _trackIndex--;

        QueueTrack(_playlist[_trackIndex]);
    }

    static internal void QueueTrack(AudioClip clip)
    {
        switch (_currentSide)
        {
            case Side.aSide:
                _bSide.clip = clip;
                _bSide.Play();
                _currentSide = Side.bSide;
                break;

            case Side.bSide:
                _aSide.clip = clip;
                _aSide.Play();
                _currentSide = Side.aSide;
                break;
        }

        Instance.StartCoroutine(Instance.CrossfadeRoutine());
    }

    internal IEnumerator CrossfadeRoutine()
    {
        switch (_currentSide)
        {
            case Side.aSide:
                while (_aSide.volume < 1 && _bSide.volume > 0)
                {
                    _aSide.volume += _crossfadeSpeed;
                    _bSide.volume -= _crossfadeSpeed;

                    yield return new WaitForFixedUpdate();
                }
                break;

            case Side.bSide:
                while (_bSide.volume < 1 && _aSide.volume > 0)
                {
                    _bSide.volume += _crossfadeSpeed;
                    _aSide.volume -= _crossfadeSpeed;

                    yield return new WaitForFixedUpdate();
                }
                break;
        }
    }
}