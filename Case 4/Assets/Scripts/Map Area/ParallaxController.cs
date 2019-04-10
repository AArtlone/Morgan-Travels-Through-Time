using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public GameObject Camera;
    private float _length;
    private float _startPosition;
    public float ParallaxEffect;

    private MapEnvironmentManager _mapEnvironmentManager;

    void Start()
    {
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
        _startPosition = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    void FixedUpdate()
    {
        Camera = _mapEnvironmentManager.CurrentCamera;
        float distanceFromCamera = Camera.transform.position.x * (1 - ParallaxEffect);
        float distanceToMove = Camera.transform.position.x * ParallaxEffect;

        transform.position = new Vector3(_startPosition + distanceToMove, transform.position.y, transform.position.z);

        if (distanceFromCamera > _startPosition + _length)
        {
            _startPosition += _length;
        } else if (distanceFromCamera < _startPosition - _length)
        {
            _startPosition -= _length;
        }
    }
}
