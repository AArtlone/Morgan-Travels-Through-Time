using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Checkpoint CheckpointLink;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = new Ray(touch.position, Vector3.forward);
            
            if (Physics.Raycast(ray, Mathf.Infinity, 9))
            {
            }
        }
        if(Input.GetKeyUp(KeyCode.M))
        {
            ToggleObstacle();
        }
    }

    public void ToggleObstacle()
    {
        CheckpointLink.Passable = !CheckpointLink.Passable;
    }
}
