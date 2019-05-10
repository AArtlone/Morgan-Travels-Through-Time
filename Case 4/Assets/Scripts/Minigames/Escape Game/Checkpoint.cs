using UnityEngine;
using System.Collections.Generic;

public class Checkpoint: MonoBehaviour
{
    public bool Passable;
    public List<QueueElement> QueueElements;
    public Obstacle ObstacleLink;
    public GameObject FirstQueueElement;
    public GameObject LastQueueElement;

    //private void Start()
    //{
    //    FirstQueueElement = transform.GetChild(0).gameObject;
    //    LastQueueElement= transform.GetChild(1).gameObject;
    //}

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
        //for (int i = 1; i < QueueElements.Count - 2; i++)
        //{
        //    //float randomQueueElementPosX = Random.Range(QueueElements[0].gameObject.transform.position.x, QueueElements[QueueElements.Count - 1].gameObject.transform.position.x);

        //    QueueElements[i].gameObject.transform.position = new Vector3(randomQueueElementPosX, QueueElements[i].gameObject.transform.position.y, QueueElements[i].gameObject.transform.position.z);
        //}
    }

    public void RemoveQueueElement(int index)
    {
        QueueElements.Remove(QueueElements[index]);
    }
}
