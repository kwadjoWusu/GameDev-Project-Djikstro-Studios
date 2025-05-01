using UnityEngine;

public class WorldCanvasPositioner : MonoBehaviour 
{
    [SerializeField] private float offsetFromCamera = 1f;
    private Camera mainCamera;
    
    void Start() 
    {
        mainCamera = Camera.main;
    }
    
    void LateUpdate() 
    {
        if (mainCamera != null)
        {
            // Keep same X,Y coordinates but position in front of camera
            Vector3 newPosition = transform.position;
            newPosition.z = mainCamera.transform.position.z + offsetFromCamera;
            transform.position = newPosition;
        }
    }
}