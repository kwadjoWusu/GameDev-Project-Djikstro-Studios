using UnityEngine;

//Tracking the target
public class CameraFollow : MonoBehaviour
{
    private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX,transform.position.y,transform.position.z),
            ref velocity, speed * Time.deltaTime);
    }
    
}