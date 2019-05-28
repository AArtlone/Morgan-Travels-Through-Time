using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public List<Transform> Backgrounds = new List<Transform>();
    private float[] parallaxScales;
    public float Smoothing = 1;

    private Camera _camera;
    private Vector3 _previousCameraPosition;

    private void Start()
    {
        _camera = FindObjectOfType<CameraBehavior>().GetComponent<Camera>();
        _previousCameraPosition = _camera.transform.position;

        parallaxScales = new float[Backgrounds.Count];

        for (int i = 0; i < parallaxScales.Length; i++)
        {
            parallaxScales[i] = Backgrounds[i].position.z * -1;
        }
    }

    private void Update()
    {
        for (int i = 0; i < Backgrounds.Count; i++)
        {
            float parallax = (_previousCameraPosition.x - _camera.transform.position.x) * parallaxScales[i];

            float backgroundTargetPositionX = Backgrounds[i].position.x + parallax;
            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, Backgrounds[i].position.y, Backgrounds[i].position.z);

            Backgrounds[i].position = Vector3.Lerp(Backgrounds[i].position, backgroundTargetPosition, Smoothing * Time.deltaTime);
        }

        _previousCameraPosition = _camera.transform.position;
    }
}
