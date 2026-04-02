//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.IO;

//public class PipelineInspectionSetup : MonoBehaviour
//{
//    [Header("Manager")]
//    public StepManager stepManager;

//    [Header("Player Camera")]
//    public Camera playerCamera;

//    [Header("Detection")]
//    public float detectDistance = 5f;
//    public LayerMask defectLayer;

//    [Header("Targets")]
//    public List<GameObject> defects; 

//    private HashSet<GameObject> capturedDefects = new HashSet<GameObject>();

//    [Header("Photo Settings")] 
//    public int photoWidth = 1920;
//    public int photoHeight = 1080;


//    [Header("UI")]
//    public RawImage previewImage;
//    public GameObject previewPanel;

//    public AudioManager audioManager;

//    public AudioClip captureSound;
//    public AudioClip ExitManHoleClip;

//    [Header("Highlight (Optional)")]
//    public Material highlightMat;
//    private GameObject currentTarget;
//    private Material originalMat;

//    [Header("Settings")]
//    public bool autoComplete = true;

//    void OnEnable()
//    {
//        if (previewPanel != null)
//            previewPanel.SetActive(false);
//    }

//    void Update()
//    {
//        DetectTarget();

//        // 🔥 INPUT (trigger / mouse for testing)
//        //if (Input.GetButtonDown("Fire1"))
//        //{
//        //    TryCapture();
//        //}
//    }

//    // ---------------------------
//    // 🔍 DETECT DEFECT
//    // ---------------------------
//    void DetectTarget()
//    {
//        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, detectDistance, defectLayer))
//        {
//            GameObject hitObj = hit.collider.gameObject;

//            if (currentTarget != hitObj)
//            {
//                ResetHighlight();

//                currentTarget = hitObj;

//                // Highlight
//                Renderer r = currentTarget.GetComponent<Renderer>();
//                if (r != null && highlightMat != null)
//                {
//                    originalMat = r.material;
//                    r.material = highlightMat;
//                }
//            }
//        }
//        else
//        {
//            ResetHighlight();
//            currentTarget = null;
//        }
//    }

//    void ResetHighlight()
//    {
//        if (currentTarget != null)
//        {
//            Renderer r = currentTarget.GetComponent<Renderer>();
//            if (r != null && originalMat != null)
//            {
//                r.material = originalMat;
//            }
//        }
//    }

//    // ---------------------------
//    // 📸 CAPTURE LOGIC
//    // ---------------------------
//    public void TryCapture()
//    {
//        Debug.Log("Capturing picture");
//        if (currentTarget == null) return;

//        if (capturedDefects.Contains(currentTarget))
//        {
//            Debug.Log("Already Captured");
//            return;
//        }

//        StartCoroutine(CapturePhoto());
//        capturedDefects.Add(currentTarget);

//        Debug.Log("Captured: " + currentTarget.name);

//        // Optional sound
//        if (captureSound != null)
//        {
//            audioManager.PlaySFX(captureSound);
//        }

//        //// Disable object or mark as done
//        //currentTarget.SetActive(false);

//        CheckCompletion();
//    }

//    IEnumerator CapturePhoto()
//    {
//        yield return new WaitForEndOfFrame();

//        int width = photoWidth;
//        int height = photoHeight;

//        RenderTexture rt = new RenderTexture(width, height, 24);

//        // Backup
//        RenderTexture prevRT = playerCamera.targetTexture;
//        Rect prevRect = playerCamera.rect;

//        // ✅ ONLY THESE TWO ARE NEEDED
//        playerCamera.targetTexture = rt;
//        playerCamera.rect = new Rect(0, 0, 1, 1);

//        // ❌ DO NOT TOUCH aspect in VR

//        playerCamera.Render();

//        RenderTexture.active = rt;

//        Texture2D photo = new Texture2D(width, height, TextureFormat.RGB24, false);
//        photo.ReadPixels(new Rect(0, 0, width, height), 0, 0);
//        photo.Apply();

//        // Restore
//        playerCamera.targetTexture = prevRT;
//        playerCamera.rect = prevRect;
//        RenderTexture.active = null;

//        Destroy(rt);

//        // Preview
//        if (previewImage && previewPanel)
//        {
//            previewImage.texture = photo;
//            previewPanel.SetActive(true);
//        }

//        // Save
//        byte[] bytes = photo.EncodeToPNG();
//        string path = Path.Combine(Application.persistentDataPath,
//            "Inspection_" + System.DateTime.Now.Ticks + ".png");

//        File.WriteAllBytes(path, bytes);
//        Debug.Log("Saved: " + path);

//        yield return new WaitForSeconds(3f);
//        previewPanel.SetActive(false);
//    }



//    // ---------------------------
//    // ✅ COMPLETION
//    // ---------------------------
//    void CheckCompletion()
//    {
//        if (!autoComplete) return;

//        if (capturedDefects.Count >= defects.Count)
//        {
//            Debug.Log("All Defects Captured ✅");
//            audioManager.PlayInstruction(ExitManHoleClip);
//            Invoke(nameof(CompleteStep), 1f);
//        }
//    }

//    void CompleteStep()
//    {
//        stepManager.CompleteCurrentStep();
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PipelineInspectionSetup : MonoBehaviour
{
    [Header("Manager")]
    public StepManager stepManager;

    [Header("Player Camera")]
    public Camera playerCamera;

    [Header("Detection")]
    public float detectDistance = 5f;
    public LayerMask defectLayer;

    [Header("Targets")]
    public List<GameObject> defects;

    private HashSet<GameObject> capturedDefects = new HashSet<GameObject>();

    [Header("Photo Settings")]
    public int photoWidth = 1920;
    public int photoHeight = 1080;

    [Header("UI")]
    public RawImage previewImage;
    public GameObject previewPanel;

    public AudioManager audioManager;

    public AudioClip captureSound;
    public AudioClip ExitManHoleClip;

    [Header("Highlight (Optional)")]
    public Material highlightMat;
    private GameObject currentTarget;
    private Material originalMat;

    [Header("Exit Check")]
    public Transform exitPoint;         // 🔥 Assign top of manhole
    public float exitDistance = 1.5f;

    private bool allDefectsCaptured = false;

    [Header("Settings")]
    public bool autoComplete = true;

    void OnEnable()
    {
        if (previewPanel != null)
            previewPanel.SetActive(false);
    }

    void Update()
    {
        DetectTarget();
        CheckExitReached(); // 🔥 NEW
    }

    // ---------------------------
    // 🔍 DETECT DEFECT
    // ---------------------------
    void DetectTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectDistance, defectLayer))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (currentTarget != hitObj)
            {
                ResetHighlight();

                currentTarget = hitObj;

                Renderer r = currentTarget.GetComponent<Renderer>();
                if (r != null && highlightMat != null)
                {
                    originalMat = r.material;
                    r.material = highlightMat;
                }
            }
        }
        else
        {
            ResetHighlight();
            currentTarget = null;
        }
    }

    void ResetHighlight()
    {
        if (currentTarget != null)
        {
            Renderer r = currentTarget.GetComponent<Renderer>();
            if (r != null && originalMat != null)
            {
                r.material = originalMat;
            }
        }
    }

    // ---------------------------
    // 📸 CAPTURE LOGIC
    // ---------------------------
    public void TryCapture()
    {
        Debug.Log("Capturing picture");

        if (currentTarget == null) return;

        if (capturedDefects.Contains(currentTarget))
        {
            Debug.Log("Already Captured");
            return;
        }

        StartCoroutine(CapturePhoto());
        capturedDefects.Add(currentTarget);

        Debug.Log("Captured: " + currentTarget.name);

        if (captureSound != null)
        {
            audioManager.PlaySFX(captureSound);
        }

        CheckCompletion();
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        int width = photoWidth;
        int height = photoHeight;

        RenderTexture rt = new RenderTexture(width, height, 24);

        RenderTexture prevRT = playerCamera.targetTexture;
        Rect prevRect = playerCamera.rect;

        playerCamera.targetTexture = rt;
        playerCamera.rect = new Rect(0, 0, 1, 1);

        playerCamera.Render();

        RenderTexture.active = rt;

        Texture2D photo = new Texture2D(width, height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        photo.Apply();

        playerCamera.targetTexture = prevRT;
        playerCamera.rect = prevRect;
        RenderTexture.active = null;

        Destroy(rt);

        // Preview
        if (previewImage && previewPanel)
        {
            previewImage.texture = photo;
            previewPanel.SetActive(true);
        }

        // Save
        byte[] bytes = photo.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath,
            "Inspection_" + System.DateTime.Now.Ticks + ".png");

        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved: " + path);

        yield return new WaitForSeconds(3f);
        previewPanel.SetActive(false);
    }

    // ---------------------------
    // ✅ COMPLETION LOGIC
    // ---------------------------
    void CheckCompletion()
    {
        if (!autoComplete) return;

        if (capturedDefects.Count >= defects.Count)
        {
            Debug.Log("All Defects Captured ✅");

            allDefectsCaptured = true;

            // 🔊 Tell user to exit
            if (ExitManHoleClip != null)
                audioManager.PlayInstruction(ExitManHoleClip);
        }
    }

    // ---------------------------
    // 🚪 EXIT CHECK
    // ---------------------------
    void CheckExitReached()
    {
        if (!allDefectsCaptured || exitPoint == null) return;

        float dist = Vector3.Distance(playerCamera.transform.position, exitPoint.position);

        if (dist <= exitDistance)
        {
            Debug.Log("Player exited manhole ✅");

            allDefectsCaptured = false; // prevent repeat trigger

            Invoke(nameof(CompleteStep), 1f);
        }
    }

    void CompleteStep()
    {
        stepManager.CompleteCurrentStep();
    }
}
