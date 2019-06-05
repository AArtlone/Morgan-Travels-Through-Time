using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private Escape _gameInterface;
    private CannonBallShooting _canonInterface;
    private List<Collider2D> _objectsHit = new List<Collider2D>();

    private void Start()
    {
        _gameInterface = FindObjectOfType<Escape>();
        _canonInterface = FindObjectOfType<CannonBallShooting>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Escape Ground")
        {
            GetComponent<SpriteRenderer>().sprite = null;
            Collider2D[] objHit = Physics2D.OverlapCircleAll(col.transform.position, _gameInterface.ExplosionRadius, _canonInterface.LayerMask);

            for (int i = 0; i < objHit.Length; i++)
            {
                float distanceToTheCenterOfExplosion;
                distanceToTheCenterOfExplosion = Vector2.Distance(objHit[i].transform.position, col.transform.position);
            }
        }
        Invoke("DestroyCannonBall", 1f);
    }

    private void DestroyCannonBall()
    {
        Destroy(gameObject);
    }
}
