using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private CannonBallShooting _canonInterface;
    private Refugee _refugeeToInjure;
    private Vector3 _placeOfExplosion; // Position where the cannonball hit the ground

    private void Start()
    {
        _canonInterface = FindObjectOfType<CannonBallShooting>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Escape Ground")
        {
            _placeOfExplosion = transform.position;
            _canonInterface.ExplosionParticlePrefab = Instantiate(_canonInterface.ExplosionParticlePrefab, _placeOfExplosion, Quaternion.identity);
            _canonInterface.ExplosionParticlePrefab.GetComponent<ParticleSystem>().Play();
            AudioManager.Instance.PlaySound(AudioManager.Instance.Explosion);
            _canonInterface.DestroyExplosion();

            GetComponent<SpriteRenderer>().sprite = null;
            _canonInterface.TempList.Clear();
            Collider2D[] objectsHit = Physics2D.OverlapCircleAll(col.transform.position, _canonInterface.ExplosionRadius, _canonInterface.LayerMask);

            float smallestDistance = 200f;
            for (int i = 0; i < objectsHit.Length; i++)
            {
                float distance = Vector3.Distance(_placeOfExplosion, objectsHit[i].transform.position);
                if (i == 0)
                {
                    smallestDistance = distance;
                    _refugeeToInjure = objectsHit[i].transform.gameObject.GetComponent<Refugee>();
                } else
                {
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        _refugeeToInjure = objectsHit[i].transform.gameObject.GetComponent<Refugee>();
                    }
                }
            }

            for (int i = 0; i < objectsHit.Length; i++)
            {
                _canonInterface.TempList.Add(objectsHit[i].transform.gameObject);
            }
            if (objectsHit.Length > 0)
            {
                if (_refugeeToInjure != null && _refugeeToInjure.Status != Refugee.RefugeeStatus.Injured)
                {
                    _refugeeToInjure.InjureRefugee();
                    _canonInterface.CanShoot = false;
                }
                /*if (objectsHit[0].GetComponent<Refugee>().Status != Refugee.RefugeeStatus.Injured)
                {
                    objectsHit[0].GetComponent<Refugee>().InjureRefugee();
                    _canonInterface.CanShoot = false;
                }*/
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
