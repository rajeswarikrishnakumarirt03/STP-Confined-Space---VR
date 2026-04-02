//using GogoGaga.OptimizedRopesAndCables;
//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;

//public class ConfinedSetupStep : MonoBehaviour
//{
//    [Header("Manager")]
//    public StepManager stepManager;

//    [Header("Sockets")]
//    public XRSocketInteractor barricadeSocket;
//    public XRSocketInteractor tripodSocket;
//    public XRSocketInteractor ventSocket;

//    private bool barricadePlaced = false;
//    private bool tripodPlaced = false;
//    private bool ventPlaced = false;

//    public Transform HookedToPlayer;
//    public Rope ropescript;

//    public AudioClip ManHoleDesend;
//    public Transform ManHoleEntry;
//    public Transform ManHoleExit;

//    [Header("Gas Detector")]
//    public XRGrabInteractable gasDetector; 

//    private bool isHoldingDetector = false;
//    private bool gasCheckStarted = false;

//    private int subStep = 0;

//    void OnEnable()
//    {
//        SetupListeners();
//    }

//    void SetupListeners()
//    {
//        barricadeSocket.selectEntered.AddListener(OnBarricadePlaced);
//        tripodSocket.selectEntered.AddListener(OnTripodPlaced);
//        ventSocket.selectEntered.AddListener(OnVentPlaced);

//        // 🔥 Gas detector grab detection
//        if (gasDetector != null)
//        {
//            gasDetector.selectEntered.AddListener(OnDetectorGrabbed);
//            gasDetector.selectExited.AddListener(OnDetectorReleased);
//        }
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 1 - ENTER AREA
//    // ---------------------------
//    public void OnEnterManholeArea()
//    {
//        Debug.Log("Entered Manhole Area");
//        subStep = 1;
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 2 - PLACE OBJECTS
//    // ---------------------------
//    void OnBarricadePlaced(SelectEnterEventArgs args)
//    {
//        barricadePlaced = true;
//        CheckAllPlaced();
//    }

//    void OnTripodPlaced(SelectEnterEventArgs args)
//    {
//        tripodPlaced = true;
//        CheckAllPlaced();
//    }

//    void OnVentPlaced(SelectEnterEventArgs args)
//    {
//        ventPlaced = true;
//        CheckAllPlaced();
//    }

//    void CheckAllPlaced()
//    {
//        if (barricadePlaced && tripodPlaced && ventPlaced)
//        {
//            Debug.Log("All Setup Done");
//            SubStep_GasCheck();
//        }
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 3 - GAS CHECK
//    // ---------------------------
//    public void SubStep_GasCheck()
//    {
//        subStep = 2;

//        Debug.Log("Grab the Gas Detector to Check Air Quality");
//    }

//    void OnDetectorGrabbed(SelectEnterEventArgs args)
//    {
//        if (subStep != 2) return;

//        Debug.Log("Gas Detector Grabbed");

//        isHoldingDetector = true;

//        if (!gasCheckStarted)
//        {
//            gasCheckStarted = true;
//            Invoke(nameof(GasCheckComplete), 5f); // ⏱️ wait 3 seconds
//        }
//    }

//    void OnDetectorReleased(SelectExitEventArgs args)
//    {
//        isHoldingDetector = false;
//    }

//    void GasCheckComplete()
//    {
//        // Ensure still holding (optional check)
//        if (!isHoldingDetector)
//        {
//            Debug.Log("Detector released too early - retry");
//            gasCheckStarted = false;
//            return;
//        }

//        Debug.Log("Air is SAFE ✅");

//        subStep = 3;
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 4 - HOOK PLAYER
//    // ---------------------------
//    public void OnPlayerReachedHookPoint()
//    {
//        if (subStep != 3) return;

//        Debug.Log("Player ready for hook");

//        SubStep_HookPlayer();
//    }

//    void SubStep_HookPlayer()
//    {
//        subStep = 4;

//        Debug.Log("Hooking Player...");

//        Invoke(nameof(SubStep_HookDone), 2f);
//    }

//    void SubStep_HookDone()
//    {
//        ropescript.SetEndPoint(HookedToPlayer); 
//        Debug.Log("Player Hooked");
//        subStep = 5;
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 5 - DESCEND
//    // ---------------------------
//    public void OnPlayerStartDescending()
//    {
//        if (subStep != 5) return;

//        Debug.Log("Player Descending");
//        subStep = 6;
//    }

//    // ---------------------------
//    // 🔷 SUB STEP 6 - REACH BOTTOM
//    // ---------------------------
//    public void OnReachedBottom()
//    {
//        if (subStep != 6) return;

//        Debug.Log("Reached Bottom");

//        CompleteStep();
//    }

//    // ---------------------------
//    // 🔷 COMPLETE STEP
//    // ---------------------------
//    void CompleteStep()
//    {
//        Debug.Log("Confined Setup Completed");
//        stepManager.CompleteCurrentStep();
//    }

//    // ---------------------------
//    // CLEANUP
//    // ---------------------------
//    private void OnDestroy()
//    {
//        barricadeSocket.selectEntered.RemoveListener(OnBarricadePlaced);
//        tripodSocket.selectEntered.RemoveListener(OnTripodPlaced);
//        ventSocket.selectEntered.RemoveListener(OnVentPlaced);

//        if (gasDetector != null)
//        {
//            gasDetector.selectEntered.RemoveListener(OnDetectorGrabbed);
//            gasDetector.selectExited.RemoveListener(OnDetectorReleased);
//        }
//    }
//}
using GogoGaga.OptimizedRopesAndCables;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ConfinedSetupStep : MonoBehaviour
{
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

    [Header("Manhole")]
    public AudioClip BarricadeClip;
    public AudioClip TripodClip;
    public AudioClip AirVentClip;
    public AudioClip GasCheckClip;
    public AudioClip CarabinerHook;
    public AudioClip ClimbDownManHoleClip;
    public AudioClip ManHoleExitReachedClip;


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
    private bool hasReachedExit = false; // 🔥 prevent multiple triggers


    void OnEnable()
    {
        SetupListeners();
    }

    void Update()
    {
        CheckExitDistance();
    }

    // 🔥 CONTINUOUS EXIT CHECK
    void CheckExitDistance()
    {
        if (subStep != 6 || hasReachedExit) return;

        if (player == null || ManHoleExit == null) return;

        float dist = Vector3.Distance(player.position, ManHoleExit.position);

        // Optional debug
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
    // 🔷 SUB STEP 1 - ENTER AREA
    // ---------------------------
    public void OnEnterManholeArea()
    {
        Debug.Log("Entered Manhole Area");
        subStep = 1;
    }

    // ---------------------------
    // 🔷 SUB STEP 2 - PLACE OBJECTS
    // ---------------------------

    public void PlayAudioForBarricade()
    {
        audiomanager.PlayInstruction(BarricadeClip);
    }
    void OnBarricadePlaced(SelectEnterEventArgs args)
    {
        barricadePlaced = true;
        audiomanager.PlayInstruction(TripodClip);
        CheckAllPlaced();
    }

    void OnTripodPlaced(SelectEnterEventArgs args)
    {
        tripodPlaced = true;
        audiomanager.PlayInstruction(AirVentClip);
        CheckAllPlaced();
    }

    void OnVentPlaced(SelectEnterEventArgs args)
    {
        ventPlaced = true;
        audiomanager.PlayInstruction(GasCheckClip);
        CheckAllPlaced();
    }

    void CheckAllPlaced()
    {
        if (barricadePlaced && tripodPlaced && ventPlaced)
        {
            Debug.Log("All Setup Done");
            SubStep_GasCheck();
        }
    }

    // ---------------------------
    // 🔷 SUB STEP 3 - GAS CHECK
    // ---------------------------
    public void SubStep_GasCheck()
    {
        subStep = 2;
        Debug.Log("Grab the Gas Detector to Check Air Quality");
    }

    void OnDetectorGrabbed(SelectEnterEventArgs args)
    {
        if (subStep != 2) return;

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
        audiomanager.PlayInstruction(CarabinerHook);
        Debug.Log("Air is SAFE ✅");
        subStep = 3;
    }

    // ---------------------------
    // 🔷 SUB STEP 4 - HOOK PLAYER
    // ---------------------------
    public void OnPlayerReachedHookPoint()
    {
        if (subStep != 3) return;

        Debug.Log("Player ready for hook");
        SubStep_HookPlayer();
    }

    void SubStep_HookPlayer()
    {
        subStep = 4;

        Debug.Log("Hooking Player...");
        Invoke(nameof(SubStep_HookDone), 2f);
    }

    void SubStep_HookDone()
    {
        ropescript.SetEndPoint(HookedToPlayer);
        Debug.Log("Player Hooked");
        audiomanager.PlayInstruction(ClimbDownManHoleClip);
        subStep = 5;
    }

    // ---------------------------
    // 🔷 SUB STEP 5 - DESCEND
    // ---------------------------
    public void OnPlayerStartDescending()
    {
        if (subStep != 5) return;

        Debug.Log("Player Descending");
        subStep = 6;
    }

    // ---------------------------
    // 🔷 SUB STEP 6 - REACH EXIT
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
