using UnityEngine;

public class PlayerCannon : Cannon
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanShoot)
        {
            Shoot();
        }

        if (Input.touchCount > 0 && CanShoot)
        {
            Shoot();
        }
    }
}
