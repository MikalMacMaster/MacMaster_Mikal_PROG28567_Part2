using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    void LateUpdate()
    {
        // I snap to the target every frame so any physics stutter shows up on screen
        transform.position = target.position + offset;
    }
}