//using UnityEngine;

//public class AttachToBody : MonoBehaviour
//{
//    public Transform cameraTransform;

//    Vector3 initialOffset;

//    void Start()
//    {
//        // Store initial offset from camera
//        initialOffset = transform.position - cameraTransform.position;
//    }

//    void LateUpdate()
//    {
//        if (cameraTransform == null) return;

//        // Get Y rotation only
//        float yRotation = cameraTransform.eulerAngles.y;
//        Quaternion yRot = Quaternion.Euler(0f, yRotation, 0f);

//        // Apply position with rotated offset
//        transform.position = cameraTransform.position + yRot * initialOffset;

//        // Apply only Y rotation
//        transform.rotation = yRot;
//    }
//}
using UnityEngine;

public class AttachToBody : MonoBehaviour
{
    public Transform cameraTransform;

    Vector3 initialOffset;
    Quaternion initialRotationOffset;

    void Start()
    {
        if (cameraTransform == null) return;

        // Position offset
        initialOffset = transform.position - cameraTransform.position;

        // Rotation offset (relative to camera Y rotation)
        Quaternion camYRot = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);
        initialRotationOffset = Quaternion.Inverse(camYRot) * transform.rotation;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Only Y rotation from camera
        float yRotation = cameraTransform.eulerAngles.y;
        Quaternion yRot = Quaternion.Euler(0f, yRotation, 0f);

        // Apply position with rotated offset
        transform.position = cameraTransform.position + yRot * initialOffset;

        // Apply rotation with offset
        transform.rotation = yRot * initialRotationOffset;
    }
}
