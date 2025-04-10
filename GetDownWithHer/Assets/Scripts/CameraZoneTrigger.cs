using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    public CinemachineCamera nextVCam;
    public CinemachineCamera previousVCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            previousVCam.Priority = 0;
            nextVCam.Priority = 10;
        }
    }
}