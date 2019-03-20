using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Escape : MonoBehaviour
{
    public int CurrentWave;
    public int RefugeesSaved;
    public int DelayBetweenWaves;
    public int TotalPoints;
    public GameObject RefugeePrefab;
    public List<Refugee> CurrentRefugees = new List<Refugee>();
    public List<Checkpoint> Checkpoints = new List<Checkpoint>();
    public List<RefugeeWaves> RefugeeWaves = new List<RefugeeWaves>();

    private void Start()
    {
        StartNextWave();
    }

    private IEnumerator SpawnRefugee()
    {
        if (CurrentWave > 0)
        {
            yield return new WaitForSeconds(DelayBetweenWaves);
        }
        for (int i = 0; i < RefugeeWaves[CurrentWave].Wave.Count; i++)
        {
            GameObject newRefugee = Instantiate(RefugeePrefab, GameObject.FindGameObjectWithTag("Refugees Container").transform);

            CurrentRefugees.Add(newRefugee.GetComponent<Refugee>());
            yield return new WaitForSeconds(2f);
        }
    }

    public void StartNextWave()
    {
        StartCoroutine(SpawnRefugee());
    }
}