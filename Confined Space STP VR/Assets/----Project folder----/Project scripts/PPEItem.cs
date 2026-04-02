using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PPEItem : MonoBehaviour
{
    public PPEEquipSetUp step;

    [HideInInspector] public bool isEquipped = false;

    [Header("References")]
    public XRSocketInteractor targetSocket;
    public Transform playerCamera;

    [Header("Settings")]
    public string ppeName;
    public bool disableAfterEquip = false;

    [Header("End Step Settings")]
    public bool destroyAtEnd = false;

    private XRGrabInteractable grab; // ✅ added

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>(); // ✅ cache
    }

    private void Start()
    {
        if (targetSocket != null)
        {
            targetSocket.selectEntered.AddListener(OnAttached);
        }

        if (string.IsNullOrEmpty(ppeName))
        {
            ppeName = gameObject.name;
        }
    }

    void OnAttached(SelectEnterEventArgs args)
    {
        if (isEquipped) return;

        isEquipped = true;

        Debug.Log("Attached PPE: " + ppeName);

        // ✅ INFORM STEP
        step.OnPPEEquipped(this);

        // ✅ 🔥 DELAYED LOCK (prevents weird XR behavior)

        // ✅ OPTIONAL: DISABLE AFTER DELAY (safer)
        if (disableAfterEquip)
        {
            DisableObject();
            //Invoke(nameof(DisableObject), 0.5f);
        }
        RemoveDefaultLayerAfterDelay();
    }

    void RemoveDefaultLayerAfterDelay()
    {

        if (grab != null)
        {
            int defaultLayer = InteractionLayerMask.NameToLayer("Default");

            // 🔥 remove ONLY default layer
            grab.interactionLayers &= ~(1 << defaultLayer);

            Debug.Log("Default interaction layer removed for: " + ppeName);
        }
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }

    // ✅ CALL THIS AT END OF STEP
    public void DestroyIfMarked()
    {
        if (destroyAtEnd)
        {
            Debug.Log("Destroying PPE at end: " + ppeName);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (targetSocket != null)
        {
            targetSocket.selectEntered.RemoveListener(OnAttached);
        }
    }
}
