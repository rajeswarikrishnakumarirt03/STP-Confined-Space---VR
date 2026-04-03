using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PermitSpawner : MonoBehaviour
{
    [Header("Permit Reference")]
    public GameObject generatedPermitPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public XRGrabInteractable grabInteractable;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        //grabInteractable = generatedPermitPrefab.GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    // 🔹 Call this after permit is generated
    public void SpawnAtStep(int stepIndex)
    {
        generatedPermitPrefab.SetActive(true);

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        if (stepIndex < 0 || stepIndex >= spawnPoints.Length)
        {
            Debug.LogWarning("Invalid step index!");
            return;
        }

        Transform targetPoint = spawnPoints[stepIndex];

        // Move permit
        generatedPermitPrefab.transform.position = targetPoint.position;
        generatedPermitPrefab.transform.rotation = targetPoint.rotation;

        // Save original position
        originalPosition = targetPoint.position;
        originalRotation = targetPoint.rotation;

        Debug.Log("Permit moved to step: " + stepIndex);
    }

    // 🔹 Called when user releases grab
    private void OnRelease(SelectExitEventArgs args)
    {
        ResetToOriginalPosition();
    }

    void ResetToOriginalPosition()
    {
        generatedPermitPrefab.transform.position = originalPosition;
        generatedPermitPrefab.transform.rotation = originalRotation;

        Debug.Log("🔁 Permit returned to original position");
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
