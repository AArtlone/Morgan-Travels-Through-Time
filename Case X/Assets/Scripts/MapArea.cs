using UnityEngine;

public class MapArea : MonoBehaviour
{
    public string Name;
    public Area.AreaStatus AreaStatus;
    public GameObject AreaEnvironment;
    public GameObject PlayerEnterStartPosition;

    private void Start()
    {
        Avatar.Instance.transform.SetParent(InterfaceManager.Instance.transform);

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
            Avatar.Instance.gameObject.transform.position = PlayerEnterStartPosition.transform.position;

            Avatar.Instance.gameObject.SetActive(true);
            Animator characterAnimator = Avatar.Instance.gameObject.GetComponent<Animator>();
            characterAnimator.SetBool("Idle", true);
            characterAnimator.SetBool("Right", false);
            characterAnimator.SetBool("Left", false);

            AreaEnvironment.SetActive(true);
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
        Avatar.Instance.gameObject.SetActive(false);
    }
}