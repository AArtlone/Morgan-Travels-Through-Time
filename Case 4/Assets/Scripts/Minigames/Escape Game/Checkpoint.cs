using UnityEngine;
using System.Collections.Generic;

public class Checkpoint: MonoBehaviour
{
    public bool Passable;
    public List<QueueElement> QueueElements;
    public Obstacle ObstacleLink;

    public void CreateNewQueueElement()
    {
        GameObject QueueElementInstance = new GameObject();
        QueueElementInstance.AddComponent<QueueElement>();
        QueueElementInstance.name = "QueueElement";
        QueueElementInstance.transform.parent = transform;
        float randomQueueElementPos = Random.Range(transform.position.x, QueueElementInstance.GetComponent<QueueElement>().maximumDistance);
        Vector3 QueueElementPos = new Vector3(transform.position.x + randomQueueElementPos, transform.position.y, 0f);
        QueueElementInstance.transform.position = QueueElementPos;
        QueueElements.Add(QueueElementInstance.GetComponent<QueueElement>());
    }

    public void RemoveQueueElement(int index)
    {
        QueueElements.Remove(QueueElements[index]);
    }
}
