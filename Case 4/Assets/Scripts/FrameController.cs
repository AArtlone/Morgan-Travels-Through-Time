using System.Collections;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    public GameObject PictureToDisplay;
    [Range(1, 100)]
    public float FadeInSpeed = 1;
    [Range(1, 100)]
    public float MovementSpeed = 1;

    private SpriteRenderer _pictureSpriteRenderer;
    private Vector3 _startPosition;

    private void Start()
    {
        _pictureSpriteRenderer = PictureToDisplay.GetComponent<SpriteRenderer>();
        _startPosition = PictureToDisplay.transform.localPosition;
    }

    public IEnumerator DisplayFrame()
    {
        while (_pictureSpriteRenderer.color.a < 1 || PictureToDisplay.transform.localPosition.y > 0)
        {
            if (_pictureSpriteRenderer.color.a < 1)
            {
                _pictureSpriteRenderer.color = new Color(255, 255, 255, _pictureSpriteRenderer.color.a + FadeInSpeed / 1000);
            }

            if (PictureToDisplay.transform.localPosition.y > 0)
            {
                PictureToDisplay.transform.localPosition = Vector3.Lerp(PictureToDisplay.transform.localPosition, new Vector3(_startPosition.x, 0, _startPosition.z), MovementSpeed / 1000);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    public void InitiateFrameVisualization()
    {
        StartCoroutine(DisplayFrame());
    }
}
