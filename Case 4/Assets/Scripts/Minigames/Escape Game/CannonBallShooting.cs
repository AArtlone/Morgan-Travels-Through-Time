using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallShooting : MonoBehaviour
{
    public Vector3 _targetPos;
    public Rigidbody2D CannonBallPrefab;
    public float TimeToFly;

    private void Start()
    {
        Shoot();
    }

    public void Shoot()
    {
        Vector3 Vo = CalculateVelocity(_targetPos, transform.position, TimeToFly);
        Rigidbody2D obj = Instantiate(CannonBallPrefab, transform.position, Quaternion.identity);
        obj.AddForce(Vector2.right * 10);
        obj.velocity = Vo;
    }
        
    private Vector3 CalculateVelocity(Vector3 _targetPos, Vector3 shootPos, float timeToFly)
    {
        Vector3 distance = _targetPos - shootPos;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float Sy = distance.y;
        float Sxz = distance.magnitude;

        float Vxz = Sxz / timeToFly;
        float Vy = Sy / timeToFly + .5f * Mathf.Abs(Physics.gravity.y) * timeToFly;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }
}
