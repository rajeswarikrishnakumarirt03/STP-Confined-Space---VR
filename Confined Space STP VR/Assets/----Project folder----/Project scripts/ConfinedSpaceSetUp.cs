

using GogoGaga.OptimizedRopesAndCables;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ConfinedSetupStep : MonoBehaviour
{

    public GameObject TeleportAnchor1;
    public GameObject TeleportAnchor2;
    [Header("Manager")]
    public StepManager stepManager;

    [Header("Sockets")]
    public XRSocketInteractor barricadeSocket;
    public XRSocketInteractor tripodSocket;
    public XRSocketInteractor ventSocket;

    private bool barricadePlaced = false;
    private bool tripodPlaced = false;
    private bool ventPlaced = false;

    [Header("Rope")]
    public Transform HookedToPlayer;
    public Rope ropescript;

    public AudioManager audiomanager;

    [Header("Audio Clips")]
    public AudioClip BarricadeClip;
    public AudioClip TripodClip;
    public AudioClip AirVentClip;
    public AudioClip GasCheckClip;
    public AudioClip CarabinerHook;
    public AudioClip ClimbDownManHoleClip;
    public AudioClip ManHoleExitReachedClip;

    [Header("Manhole")]
    public Transform ManHoleEntry;
    public Transform ManHoleExit;
    public Transform player;
    public float exitCheckDistance = 1.5f;

    [Header("Gas Detector")]
    public XRGrabInteractable gasDetector;

    private bool isHoldingDetector = false;
    private bool gasCheckStarted = false;

    public GameObject HarnessAttachToPlayer;

    private int subStep = 0;
    private bool hasReachedExit = false;

    public PermitSpawner permitSpawner;

    public GameObject Oxygen;
    public GameObject SulphurContent;
    public GameObject AreaSecured;
    public GameObject Lifeline;
    public GameObject Guard;
    public GameObject AirVent;
    public GameObject GasDetectorReadings;
    void OnEnable()
    {
        TeleportAnchor1.SetActive(true);
        SetupListeners();
        permitSpawner.SpawnAtStep(1);
    }

    void Update()
    {
        CheckExitDistance();
    }

    // ---------------------------
    // 🔥 EXIT CHECK
    // ---------------------------
    void CheckExitDistance()
    {
        if (subStep != 5 || hasReachedExit) return;

        if (player == null || ManHoleExit == null) return;

        float dist = Vector3.Distance(player.position, ManHoleExit.position);
        Debug.DrawLine(player.position, ManHoleExit.position, Color.green);

        if (dist <= exitCheckDistance)
        {
            hasReachedExit = true;
            OnReachedExit();
        }
    }

    void SetupListeners()
    {
        barricadeSocket.selectEntered.AddListener(OnBarricadePlaced);
        tripodSocket.selectEntered.AddListener(OnTripodPlaced);
        ventSocket.selectEntered.AddListener(OnVentPlaced);

        if (gasDetector != null)
        {
            gasDetector.selectEntered.AddListener(OnDetectorGrabbed);
            gasDetector.selectExited.AddListener(OnDetectorReleased);
        }
    }

    // ---------------------------
    // 🔷 START → GAS CHECK FIRST
    // ---------------------------
    public void OnEnterManholeArea()
    {
        Debug.Log("Start → Gas Check First");
        SubStep_GasCheck();
    }

    // ---------------------------
    // 🔷 STEP 1 - GAS CHECK
    // ---------------------------
    public void SubStep_GasCheck()
    {
        subStep = 1;
        Debug.Log("Grab the Gas Detector to Check Air Quality");
        audiomanager.PlayInstruction(GasCheckClip);
    }

    void OnDetectorGrabbed(SelectEnterEventArgs args)
    {
        if (subStep != 1) return;

        Debug.Log("Gas Detector Grabbed");

        isHoldingDetector = true;

        if (!gasCheckStarted)
        {
            gasCheckStarted = true;
            Invoke(nameof(GasCheckComplete), 3f);
        }
    }

    void OnDetectorReleased(SelectExitEventArgs args)
    {
        isHoldingDetector = false;
    }

    void GasCheckComplete()
    {
        if (!isHoldingDetector)
        {
            Debug.Log("Detector released too early - retry");
            gasCheckStarted = false;
            return;
        }
        GasDetectorReadings.SetActive(true);
        Debug.Log("Air is SAFE ✅");
        audiomanager.PlayInstruction(BarricadeClip);
        Oxygen.SetActive(true);
        SulphurContent.SetActive(true);
        subStep = 2; // 👉 Move to setup
    }

    // ---------------------------
    // 🔷 STEP 2 - SETUP AREA
    // ---------------------------
    void OnBarricadePlaced(SelectEnterEventArgs args)
    {
        if (subStep != 2) return;

        barricadePlaced = true;
        audiomanager.PlayInstruction(TripodClip);
        AreaSecured.SetActive(true);
        CheckAllPlaced();
    }

    void OnTripodPlaced(SelectEnterEventArgs args)
    {
        if (subStep != 2) return;

        tripodPlaced = true;
        audiomanager.PlayInstruction(AirVentClip);
        CheckAllPlaced();
    }

    void OnVentPlaced(SelectEnterEventArgs args)
    {
        if (subStep != 2) return;

        ventPlaced = true;
        audiomanager.PlayInstruction(CarabinerHook);
        AirVent.SetActive(true);
        CheckAllPlaced();
    }

    void CheckAllPlaced()
    {
        if (barricadePlaced && tripodPlaced && ventPlaced)
        {
            Debug.Log("All Setup Done");
            TeleportAnchor1.SetActive(false);
            TeleportAnchor2.SetActive(true);
            //SubStep_HookPlayer();
        }
    }

    // ---------------------------
    // 🔷 STEP 3 - HOOK PLAYER
    // ---------------------------
    public void OnPlayerReachedHookPoint()
    {
        //if (subStep != 3) return;

        Debug.Log("Player ready for hook");
        Guard.SetActive(true);
        SubStep_HookPlayer();
    }

    void SubStep_HookPlayer()
    {
        subStep = 3;

        Debug.Log("Hooking Player...");
        Invoke(nameof(SubStep_HookDone), 2f);
    }

    void SubStep_HookDone()
    {
        ropescript.SetEndPoint(HookedToPlayer);
        Debug.Log("Player Hooked");
        Lifeline.SetActive(true);
        audiomanager.PlayInstruction(ClimbDownManHoleClip);

        subStep = 4;
    }

    // ---------------------------
    // 🔷 STEP 4 - DESCEND
    // ---------------------------
    public void OnPlayerStartDescending()
    {
        if (subStep != 4) return;

        Debug.Log("Player Descending");
        subStep = 5;
    }

    // ---------------------------
    // 🔷 STEP 5 - EXIT REACHED
    // ---------------------------
    void OnReachedExit()
    {
        Debug.Log("Reached Manhole Exit");

        audiomanager.PlayInstruction(ManHoleExitReachedClip);

        HarnessAttachToPlayer.GetComponent<AttachToBody>().enabled = true;

        CompleteStep();
    }

    // ---------------------------
    // 🔷 COMPLETE STEP
    // ---------------------------
    void CompleteStep()
    {
        Debug.Log("Confined Setup Completed");

        stepManager.CompleteCurrentStep();
        TeleportAnchor2.SetActive(false);
        subStep = -1;
    }

    // ---------------------------
    // CLEANUP
    // ---------------------------
    private void OnDestroy()
    {
        barricadeSocket.selectEntered.RemoveListener(OnBarricadePlaced);
        tripodSocket.selectEntered.RemoveListener(OnTripodPlaced);
        ventSocket.selectEntered.RemoveListener(OnVentPlaced);

        if (gasDetector != null)
        {
            gasDetector.selectEntered.RemoveListener(OnDetectorGrabbed);
            gasDetector.selectExited.RemoveListener(OnDetectorReleased);
        }
    }
}
