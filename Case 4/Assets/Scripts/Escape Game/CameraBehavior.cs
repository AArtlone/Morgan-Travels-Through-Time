using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float MaxLeftPosition;
    public Transform CharacterAvatar;

    private void Update()
    {
        if (CharacterAvatar.position.x >= -42)
        {
            transform.position = new Vector3(CharacterAvatar.position.x, CharacterAvatar.position.y + 1, -10);
        }
    }

}
