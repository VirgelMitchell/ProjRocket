using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    GameObject player;

    private void Start() {
        player = GameObject.FindWithTag("Player");
    }
    private void Update() {
        transform.position = player.transform.position;
    }
}
