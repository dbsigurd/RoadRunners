using System.Collections;
using UnityEngine;

// This version of the Jukebox just caches the Instance object as a singleton, and pulls information from it for the lifecycle of the game.
// From what I understand, this takes up less memory, but I find it makes it harder to read and comprehend.
// Though it matters very little for a game of this scope, I suspect there's a bit of a performance overhead doing it this way too.
// Needless to say, I'm not sure which is better between this and Jukebox_OLD. Any insight would be appreciated.

public class Jukebox : MonoBehaviour
{
    internal static Jukebox Instance { get; set; }

    [SerializeField] bool _autoplayOnStart;

    internal enum Side { aSide, bSide }
    internal Side CurrentSide = Side.aSide;

    [SerializeField] AudioSource _aSide, _bSide;

    [SerializeField]
    [Range(0.001f, 0.05f)]
    float _crossfadeSpeed;

    [SerializeField] AudioClip[] _playlist;
    int _trackIndex = 0;

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
        _aSide.clip = _playlist[Random.Range(0, _playlist.Length - 1)];

        if (_autoplayOnStart)
            QueueTrack(_aSide.clip);
    }

    static internal void PlayNext()
    {
        if (Instance._trackIndex == Instance._playlist.Length - 1)
            Instance._trackIndex = 0;
        else Instance._trackIndex++;

        QueueTrack(Instance._playlist[Instance._trackIndex]);
    }

    static internal void PlayPrevious()
    {
        if (Instance._trackIndex == 0)
            Instance._trackIndex = Instance._playlist.Length - 1;
        else Instance._trackIndex--;

        QueueTrack(Instance._playlist[Instance._trackIndex]);
    }

    static internal void QueueTrack(AudioClip clip)
    {
        switch (Instance.CurrentSide)
        {
            case Side.aSide:
                Instance._bSide.clip = clip;
                Instance._bSide.Play();
                Instance.CurrentSide = Side.bSide;
                break;

            case Side.bSide:
                Instance._aSide.clip = clip;
                Instance._aSide.Play();
                Instance.CurrentSide = Side.aSide;
                break;
        }

        Instance.StartCoroutine(Instance.CrossfadeRoutine());
    }

    internal IEnumerator CrossfadeRoutine()
    {
        switch (Instance.CurrentSide)
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
