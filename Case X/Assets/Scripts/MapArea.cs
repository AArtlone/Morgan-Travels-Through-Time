using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    public string Name;
    public Area.AreaStatus AreaStatus;
    public GameObject AreaEnvironment;

    private void Start()
    {
        foreach (Area area in Character.Instance.Areas)
        {
            if (area.Name == Name)
            {
                Name = area.Name;
                AreaStatus = area.Status;
            }
        }
    }

    public void EnterArea()
    {
        if (AreaStatus == Area.AreaStatus.Unlocked)
        {
            AreaEnvironment.SetActive(true);
        } else
        {
            InterfaceManager.Instance.OpenPopup(InterfaceManager.Instance.AreaLockedErrorPopup);
        }
    }

    public void ExitArea()
    {
        AreaEnvironment.SetActive(false);
    }
}
