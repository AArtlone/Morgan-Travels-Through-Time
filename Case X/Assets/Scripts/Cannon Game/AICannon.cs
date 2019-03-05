using UnityEngine;

public class AICannon : Cannon
{
    private bool _pickedNewRotation = false;
    private Quaternion _newRotation;

    private void FixedUpdate()
    {
        if (_pickedNewRotation == false)
        {
            _newRotation = Quaternion.Euler(
                CannonTurret.rotation.x,
                CannonTurret.rotation.y,
                Mathf.Clamp(Random.Range(0f, 65f) * -1, -65, 0));

            Invoke("GetNewRotation", 6f);
            _pickedNewRotation = true;
        } else
        {
            Rotate(_newRotation);
        }
    }

    private void GetNewRotation()
    {
        _pickedNewRotation = false;
    }
}
