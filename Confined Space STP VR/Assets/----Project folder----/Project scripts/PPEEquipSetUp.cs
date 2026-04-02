using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PPEEquipSetUp : MonoBehaviour
{
    public StepManager stepManager;

    [Header("Audio")]
    public AudioManager audioManager;
    public AudioClip instructionAudio;
    public AudioClip equipSFX;
    public AudioClip equippedPPE;
    [Header("All PPE Items")]
    public List<PPEItem> ppeItems = new List<PPEItem>();

    private int equippedCount = 0;

    public GameObject[] controllerVisuals;
    public GameObject[] HandVisuals;
    public GameObject[] HandVisualsMesh;

    [Header("Glove Visual Change")]
    public Material glovesMaterial; // assign in inspector


    void OnEnable()
    {
        equippedCount = 0;

        foreach (GameObject controller in controllerVisuals)
        {
            controller.SetActive(false);
        }

        foreach (GameObject hands in HandVisuals)
        {
            hands.SetActive(true);
        }

        // ✅ PLAY INSTRUCTION AUDIO
        if (instructionAudio != null)
        {
            audioManager?.PlayInstruction(instructionAudio);
        }

        foreach (var item in ppeItems)
        {
            item.step = this;
            item.isEquipped = false; 
        }
    }



    public void OnPPEEquipped(PPEItem item)
    {
        //if (item.isEquipped) return;

        item.isEquipped = true;
        equippedCount++;
      
        Debug.Log("PPE Equipped: " + item.name);

        if (equipSFX != null)
        {
            audioManager?.PlaySFX(equipSFX);
        }
        // 🔥 CHECK FOR GLOVES
        if (item.ppeName.Contains("Gloves"))
        {
            ApplyGloveMaterial();
        }

        if (equippedCount >= ppeItems.Count)
        {
            Debug.Log("🎯 All PPE Equipped");


            // ✅ Destroy specific PPE
            foreach (var items in ppeItems)
            {
                items.DestroyIfMarked();
            }
            StartCoroutine(PPECompleted());
            stepManager.CompleteCurrentStep();
        }

    }
    void ApplyGloveMaterial()
    {
        if (glovesMaterial == null) return;

        foreach (GameObject hand in HandVisualsMesh)
        {
            if (hand == null) continue;

            Renderer[] renderers = hand.GetComponents<Renderer>();

            foreach (Renderer r in renderers)
            {
                r.material = glovesMaterial;
            }
        }

        Debug.Log("🧤 Gloves material applied to hands");
    }

    public IEnumerator PPECompleted()
    {
        audioManager.PlayInstruction(equippedPPE);
        yield return new WaitForSeconds(3f);
    }
}
