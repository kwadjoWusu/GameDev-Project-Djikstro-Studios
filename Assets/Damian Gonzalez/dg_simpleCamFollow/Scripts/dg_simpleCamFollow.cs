using System.Collections;
using UnityEngine;

public class dg_simpleCamFollow : MonoBehaviour
{
    public Transform target;
    [Range(1f, 40f)] public float laziness = 10f;
    public bool lookAtTarget = true;
    public bool takeOffsetFromInitialPos = true;
    public Vector3 generalOffset;
    
    private Vector3 whereCameraShouldBe;
    private Quaternion initialRotation;
    private bool warningAlreadyShown = false;

    private void Start()
    {
        if (target != null)
        {
            if (takeOffsetFromInitialPos)
            {
                generalOffset = transform.position - target.position;
            }
            
            // Store the initial rotation between camera and target
            initialRotation = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            whereCameraShouldBe = target.position + generalOffset;
            transform.position = Vector3.Lerp(transform.position, whereCameraShouldBe, 1 / laziness);
            
            if (lookAtTarget)
            {
                // Option 1: Use the initial relative rotation
                transform.rotation = initialRotation;
                
                // Option 2: Look at a point that's offset from the target
                // Vector3 lookAtPoint = target.position + new Vector3(0, generalOffset.y, 0);
                // transform.LookAt(lookAtPoint);
            }
        }
        else
        {
            if (!warningAlreadyShown)
            {
                Debug.Log("Warning: You should specify a target in the simpleCamFollow script.", gameObject);
                warningAlreadyShown = true;
            }
        }
    }
}