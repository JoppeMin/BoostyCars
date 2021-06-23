using UnityEngine;
public class FollowCam : MonoBehaviour
{
    private Transform Player;
    public Vector3 Offset;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void LateUpdate()
    {
        if (Player != null)
            transform.position = Player.position + Offset;
    }
}
