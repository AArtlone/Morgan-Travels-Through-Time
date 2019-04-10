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
        Vector3 QueueElementPos = new Vector3(transform.position.x + (QueueElementInstance.GetComponent<QueueElement>().OffSet * QueueElements.Count), transform.position.y, 0f);
        QueueElementInstance.transform.position = QueueElementPos;
        QueueElements.Add(QueueElementInstance.GetComponent<QueueElement>());
    }

    public void RemoveQueueElement(int index)
    {
        QueueElements.Remove(QueueElements[index]);
    }
}
