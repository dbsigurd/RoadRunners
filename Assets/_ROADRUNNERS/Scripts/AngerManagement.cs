using UnityEngine;

//TODO: In the unlikely event that both players trigger a vehicle explosion almost simultaneously, only one animation will fire, because only one exists.
// Consider switching to a non-static pool of explosions. The alternative is each vehicle gets and fires their own explosion, but that seems a little overkill.

public class AngerManagement : MonoBehaviour
{
    static Animator Explosion;
    static AudioSource Boom;

    Animator _explosion;
    AudioSource _boom;

    private void Start()
    {
        _explosion = GetComponentInChildren<Animator>();
        _boom = GetComponentInChildren<AudioSource>();

        Explosion = _explosion;
        Boom = _boom;
    }

    internal static void Explode(Vector3 position)
    {
        Explosion.transform.position = position;
        Explosion.Play("Explode");
        Boom.Play();
    }
}
