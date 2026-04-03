
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

public class GeneratePermitScript : MonoBehaviour
{
    [Header("Checkboxes")]
    public Toggle check1;
    public Toggle check2;
    public Toggle check3;
    public Toggle check4;

    [Header("Button")]
    public Button generatePermitButton;

    [Header("UI Panels")]
    public GameObject PermitCanvas;
    public GameObject GeneratedPermit;

    [Header("Input Fields")]
    public string jobSiteInput;
    public string purposeInput;

    [Header("Output Texts")]
    public TMP_Text currentTimeText;
    public TMP_Text expiryTimeText;
    public TMP_Text jobSiteText;
    public TMP_Text purposeText;

    [Header("Settings")]
    public int permitDurationMinutes = 60; // expiry time

    public StepManager stepManager;

    void OnEnable()
    {
        generatePermitButton.interactable = false;
        PermitCanvas.SetActive(true);

        check1.onValueChanged.AddListener(delegate { ValidateChecks(); });
        check2.onValueChanged.AddListener(delegate { ValidateChecks(); });
        check3.onValueChanged.AddListener(delegate { ValidateChecks(); });
        check4.onValueChanged.AddListener(delegate { ValidateChecks(); });
        generatePermitButton.onClick.AddListener(OnGeneratePermitClicked);
    }

    void ValidateChecks()
    {
        generatePermitButton.interactable =
            (check1.isOn && check2.isOn && check3.isOn && check4.isOn);
    }

    public void OnGeneratePermitClicked()
    {
        if (check1.isOn && check2.isOn && check3.isOn && check4.isOn)
        {
            GeneratePermit();
        }
    }

    void GeneratePermit()
    {
        // 🟢 Current Time
        DateTime currentTime = DateTime.Now;
        currentTimeText.text = currentTime.ToString("dd/MM/yyyy HH:mm");

        // 🟢 Expiry Time
        DateTime expiryTime = currentTime.AddMinutes(permitDurationMinutes);
        expiryTimeText.text = expiryTime.ToString("dd/MM/yyyy HH:mm");

        // 🟢 Job Site
        jobSiteText.text = jobSiteInput;

        // 🟢 Purpose of Entry
        purposeText.text = purposeInput;

        // UI Switch
        PermitCanvas.SetActive(false);
        GeneratedPermit.SetActive(true);

        Debug.Log("Permit Generated Successfully ✅");
        StartCoroutine(NextStep());
    }

    public IEnumerator NextStep()
    {
        yield return new WaitForSeconds(3f);
        GeneratedPermit.SetActive(false);
        yield return new WaitForSeconds(1f);
        stepManager.CompleteCurrentStep();
    }
}
