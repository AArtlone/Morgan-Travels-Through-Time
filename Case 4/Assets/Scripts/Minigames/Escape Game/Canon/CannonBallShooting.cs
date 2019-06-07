using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallShooting: MonoBehaviour
{
    private Vector3 _targetPos;
    public Rigidbody2D CannonBallPrefab;
    public GameObject CheckpoinToShootAt;
    public float TimeToFly;
    public int TimeBetweenSalvos;
    public int NumberOfSalvos; // The number of times that the cannon will shoot before making the obstacle passable 
    [System.NonSerialized]
    public int CurrentNumberOfsalvosShot = 0; // Current number of salvos shot for current wave
    public float ExplosionRadius;
    public bool CanShoot = true;
    public LayerMask LayerMask;
    private Rigidbody2D _cannonBall;
    public List<GameObject> TempList = new List<GameObject>();

    /*private void Start()
    {
        Invoke("Shoot", 3f);
    }*/

    public void Shoot()
    {
        if (CanShoot == true && CurrentNumberOfsalvosShot < NumberOfSalvos)
        {
            float FirstQueueElementXPos = CheckpoinToShootAt.GetComponent<Checkpoint>().FirstQueueElement.transform.position.x;
            float LastQueueElementXPos = CheckpoinToShootAt.GetComponent<Checkpoint>().LastQueueElement.transform.position.x;

            float randomXPos = Random.Range(FirstQueueElementXPos, LastQueueElementXPos + 1f);
            _targetPos = new Vector3(randomXPos, CheckpoinToShootAt.transform.position.y, CheckpoinToShootAt.transform.position.z);
            Vector3 Vo = CalculateVelocity(_targetPos, transform.position, TimeToFly);
            _cannonBall = Instantiate(CannonBallPrefab, transform.position, Quaternion.identity);
            _cannonBall.velocity = Vo;
            CurrentNumberOfsalvosShot++;
            CanShoot = false;
            StartCoroutine(ShootAgainCO());
        }
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
        yield return new WaitForSeconds(TimeBetweenSalvos);
        Shoot();
    }

    public void HitCannonBallAway()
    {
        _cannonBall.velocity *= -5;
        CanShoot = true;
    }
}
