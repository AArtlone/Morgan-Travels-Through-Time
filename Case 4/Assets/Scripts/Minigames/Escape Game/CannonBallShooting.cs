using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallShooting : MonoBehaviour
{
    private Vector3 _targetPos;
    public Rigidbody2D CannonBallPrefab;
    public GameObject CheckpoinToShootAt;
    public float TimeToFly;
    private Rigidbody2D _cannonBall;

    private void Start()
    {
        Shoot();
    }

    public void Shoot()
    {
        float FirstQueueElementXPos = CheckpoinToShootAt.GetComponent<Checkpoint>().FirstQueueElement.transform.position.x;
        float LastQueueElementXPos = CheckpoinToShootAt.GetComponent<Checkpoint>().LastQueueElement.transform.position.x;

        float randomXPos = Random.Range(FirstQueueElementXPos, LastQueueElementXPos);
        _targetPos = new Vector3(randomXPos, CheckpoinToShootAt.transform.position.y, CheckpoinToShootAt.transform.position.z);
        Vector3 Vo = CalculateVelocity(_targetPos, transform.position, TimeToFly);
        _cannonBall = Instantiate(CannonBallPrefab, transform.position, Quaternion.identity);
        //obj.AddForce(Vector2.right * 10);
        _cannonBall.velocity = Vo;
        StartCoroutine(ShootAgainCO());
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

    private IEnumerator ShootAgainCO()
    {
        yield return new WaitForSeconds(1f);
        Shoot();
    }

    public void HitCannonBallAway()
    {
        _cannonBall.velocity *= -5;
    }
}
