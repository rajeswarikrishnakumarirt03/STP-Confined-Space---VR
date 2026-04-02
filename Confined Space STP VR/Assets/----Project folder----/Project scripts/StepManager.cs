using UnityEngine;

public class StepManager : MonoBehaviour
{
    public MonoBehaviour[] steps; 

    private int currentStepIndex = 0;

    void Start()
    {
        ActivateStep(0);
    }

    void ActivateStep(int index)
    {
        if (index >= steps.Length)
        {
            Debug.Log("✅ All Steps Completed");
            return;
        }

        for (int i = 0; i < steps.Length; i++)
            steps[i].gameObject.SetActive(false);

        steps[index].gameObject.SetActive(true);

        Debug.Log("▶ Step Started: " + steps[index].name);
    }

    public void CompleteCurrentStep()
    {
        Debug.Log("✔ Step Completed: " + steps[currentStepIndex].name);

        currentStepIndex++;
        ActivateStep(currentStepIndex);
    }
}
