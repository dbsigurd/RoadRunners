using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Sprite[] _sprites;

    internal void SwapSprite()
        => _renderer.sprite =
            _sprites[Random.Range(0, _sprites.Length)];
}
