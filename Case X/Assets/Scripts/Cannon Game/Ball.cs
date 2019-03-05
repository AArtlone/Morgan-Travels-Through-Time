using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ball")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
