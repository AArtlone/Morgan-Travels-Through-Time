using UnityEngine;

public class MapArea : MonoBehaviour
{
    public string Name;
    public Area.AreaStatus AreaStatus;
    public GameObject AreaEnvironment;
    public GameObject PlayerEnterStartPosition;
    public GameObject Icons;

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
            Icons.SetActive(false);
        }
        else
        {
            InterfaceManager.Instance.OpenPopup(InterfaceManager.Instance.AreaLockedErrorPopup);
        }
    }

    public void ExitArea()
    {
        AreaEnvironment.transform.GetChild(3).GetComponent<WalkablePathController>().HasPlayerReachedDestination = true;
        AreaEnvironment.SetActive(false);
        Icons.SetActive(true);
    }
}