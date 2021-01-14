using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleParallax : MonoBehaviour
{
    [SerializeField] Transform _refTransform;
    Vector3 _refPositionLast;

    [SerializeField][Range(-1f, 1f)]
        float _parallaxFactorX, _parallaxFactorY;

    [SerializeField] bool AutoSnapX, AutoSnapY;
    float _spriteSizeX, _spriteSizeY;

    private void Start()
    {
        TrackLastPosition();

        var sprite = GetComponent<SpriteRenderer>().sprite;
        var texture = sprite.texture;

        _spriteSizeX = texture.width / sprite.pixelsPerUnit * transform.localScale.x;
        _spriteSizeY = texture.height / sprite.pixelsPerUnit * transform.localScale.y;
    }

    void LateUpdate()
    {
        if (AutoSnapX)
            if (Mathf.Abs(_refTransform.position.x - transform.position.x) >= _spriteSizeX)
                transform.position = new Vector3(_refTransform.position.x, transform.position.y);

        if (AutoSnapY)
            if (Mathf.Abs(_refTransform.position.y - transform.position.y) >= _spriteSizeY)
                transform.position = new Vector3(transform.position.x, _refTransform.position.y);

        Vector3 deltaPos = _refTransform.position - _refPositionLast;

        transform.position += new Vector3
            (deltaPos.x * _parallaxFactorX, deltaPos.y * _parallaxFactorY, 0f);

        TrackLastPosition();
    }

    void TrackLastPosition()
        => _refPositionLast = _refTransform.position;
}
