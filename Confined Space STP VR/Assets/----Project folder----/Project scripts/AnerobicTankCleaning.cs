using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AnerobicTankCleaning : MonoBehaviour
{

    public GameObject TeleportAnchor;
    public Collider tankCollider;
    public XRBaseInteractable climbInteractable;

    [Header("Entry Check")]
    public Transform player;              // XR Rig / Camera
    public Transform entryPoint;          // inside tank point
    public float entryDistance = 0.5f;    // threshold

    private List<IXRSelectInteractor> currentInteractors = new List<IXRSelectInteractor>();

    private bool hasEnteredTank = false; // ✅ stop checking after entry

    private void OnEnable()
    {
        TeleportAnchor.SetActive(true);
        if (climbInteractable != null)
        {
            climbInteractable.selectEntered.AddListener(OnGrab);
            climbInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (climbInteractable != null)
        {
            climbInteractable.selectEntered.RemoveListener(OnGrab);
            climbInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (hasEnteredTank) return; // ✅ STOP AFTER ENTRY

        if (!currentInteractors.Contains(args.interactorObject))
            currentInteractors.Add(args.interactorObject);

        // ✅ BOTH HANDS REQUIRED
        if (currentInteractors.Count >= 2)
        {
            Debug.Log("Both hands detected - entering tank");
            DisableTankCollider();
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (hasEnteredTank) return; // ✅ STOP AFTER ENTRY

        if (currentInteractors.Contains(args.interactorObject))
            currentInteractors.Remove(args.interactorObject);

        if (currentInteractors.Count < 2)
        {
            Debug.Log("One hand released - blocking entry");
            EnableTankCollider();
        }
    }

    private void Update()
    {
        if (hasEnteredTank) return;

        if (player == null || entryPoint == null) return;

        float dist = Vector3.Distance(player.position, entryPoint.position);

        // ✅ PLAYER REACHED INSIDE
        if (dist <= entryDistance)
        {
            Debug.Log("Player entered tank");

            hasEnteredTank = true;

            EnableTankCollider(); 
            currentInteractors.Clear();
        }
    }

    public void DisableTankCollider()
    {
        if (tankCollider != null)
            tankCollider.enabled = false;
    }

    public void EnableTankCollider()
    {
        if (tankCollider != null)
            tankCollider.enabled = true;
    }
}
