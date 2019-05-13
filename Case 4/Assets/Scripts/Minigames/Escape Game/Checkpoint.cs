using UnityEngine;
using System.Collections.Generic;

public class Checkpoint: MonoBehaviour
{
    public bool Passable;
    public bool Occupied;
    public List<QueueElement> QueueElements;
    public Obstacle ObstacleLink;
    public GameObject FirstQueueElement;
    public GameObject LastQueueElement;

    public void CreateNewQueueElement()
    {
        GameObject QueueElementInstance = new GameObject();
        QueueElementInstance.AddComponent<QueueElement>();
        QueueElementInstance.name = "QueueElement";
        QueueElementInstance.transform.parent = transform;
        float randomQueueElementPosX = Random.Range(FirstQueueElement.transform.position.x, LastQueueElement.transform.position.x);
        Vector3 QueueElementPos = new Vector3(randomQueueElementPosX, transform.position.y, 0f);
        QueueElementInstance.transform.position = QueueElementPos;
        QueueElements.Add(QueueElementInstance.GetComponent<QueueElement>());
    }

    public void RemoveQueueElement(int index)
    {
        QueueElements.Remove(QueueElements[index]);
    }
}
