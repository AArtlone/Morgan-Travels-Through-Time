using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public int HitPoints = 3;
    [System.NonSerialized]
    public bool CanShoot = true;
    public int CoolDown = 2;
    public GameObject BallPrefab;
    [Range(0, 150)]
    public int BallSpeed;

    // Used to determine from where the cannon/boulder ball
    // will be launched from.
    private Transform _cannonExit;
    [System.NonSerialized]
    public Transform CannonTurret;
    private GameObject _canvas;

    private void Start()
    {
        _cannonExit = transform.GetChild(0).transform.GetChild(0).transform;
        CannonTurret = transform.GetChild(0).gameObject.transform;
        _canvas = GameObject.FindGameObjectWithTag("Interface Manager");
    }

    public void Damage()
    {
        HitPoints--;

        if (HitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Rotate(Quaternion newRotation)
    {
        if (CannonTurret.rotation != newRotation)
        {
            CannonTurret.rotation = Quaternion.Slerp(
                CannonTurret.rotation,
                newRotation, 3f * Time.fixedDeltaTime);
        } else if (CannonTurret.rotation == newRotation)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (CanShoot)
        {
            GameObject ball = Instantiate(BallPrefab, _cannonExit);
            if (gameObject.transform.tag == "Player" || gameObject.transform.tag == "AI Cannon")
            {
                Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), ball.GetComponent<CircleCollider2D>());
            }
            ball.transform.forward = _cannonExit.forward;

            if (gameObject.transform.tag == "Player")
            {
                ball.GetComponent<Rigidbody2D>().AddRelativeForce(ball.transform.right * BallSpeed * 300);
            } else if (gameObject.transform.tag == "AI Cannon")
            {
                ball.GetComponent<Rigidbody2D>().AddRelativeForce((ball.transform.right * BallSpeed * 450) * -1);
            }

            ball.transform.SetParent(_canvas.transform);

            Destroy(ball, 8f);
            StartCoroutine(CountDown());

            CanShoot = false;
        }
    }

    public IEnumerator CountDown()
    {
        yield return new WaitForSeconds(CoolDown);
        CanShoot = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            Damage();
            Destroy(collision.gameObject);
        }
    }
}
