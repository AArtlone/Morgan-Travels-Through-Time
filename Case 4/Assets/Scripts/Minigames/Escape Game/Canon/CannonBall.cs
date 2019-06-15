using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private CannonBallShooting _canonInterface;

    private void Start()
    {
        _canonInterface = FindObjectOfType<CannonBallShooting>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Escape Ground")
        {
            GetComponent<SpriteRenderer>().sprite = null;
            _canonInterface.TempList.Clear();
            Collider2D[] objectsHit = Physics2D.OverlapCircleAll(col.transform.position, _canonInterface.ExplosionRadius, _canonInterface.LayerMask);

            for (int i = 0; i < objectsHit.Length; i++)
            {
                _canonInterface.TempList.Add(objectsHit[i].transform.gameObject);
            }
            if (objectsHit.Length > 0)
            {
                if (objectsHit[0].GetComponent<Refugee>().Status != Refugee.RefugeeStatus.Injured)
                {
                    objectsHit[0].GetComponent<Refugee>().InjureRefugee();
                    _canonInterface.CanShoot = false;
                }
            } else
            {
                _canonInterface.CanShoot = true;
            }
            Invoke("DestroyCannonBall", 1f);
        }
    }

    private void DestroyCannonBall()
    {
        Destroy(gameObject);
    }
}
