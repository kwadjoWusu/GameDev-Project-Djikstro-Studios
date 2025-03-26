using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float smoothSpeed = 5f; // Speed of camera smoothing
    public Vector3 offset; // Offset from the player position

    void LateUpdate()
    {
        if (player == null) return; // Ensure player exists

        // Target position for the camera
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}