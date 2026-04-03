
using System.Collections;
using System.Collections.Generic;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.UI;


public class SimpleControllerTraining : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string name;

        public InputActionProperty rightInput;
        public InputActionProperty leftInput;

        public Renderer rightRenderer;
        public Renderer leftRenderer;

        public Material highlightMaterial;

        [HideInInspector] public Material rightOriginalMat;
        [HideInInspector] public Material leftOriginalMat;

        public AnimationType type;

        public Transform rightTarget;
        public Transform leftTarget;

        // ✅ AUDIO
        public AudioClip instructionAudio;
        public AudioClip inputSFX;

        // VIDEO
        public double videoStartTime;
        public double videoEndTime;
    }

    public enum AnimationType
    {
        Trigger,
        Grab,
        Joystick
    }


    public GameObject TeleportAnchor;
    public Image fadeoutImage;

    public GameObject StartPanel;
    public AudioClip WelcomeClip;
    public GameObject VideoPanel;
    public AudioClip TrainingStartClip;

    public bool startGettingInput = false;
    public List<Step> steps = new List<Step>();

    private int index = 0;

    public VideoPlayer videoPlayer;

    private bool highlightRemoved = false;
    private Coroutine demoRoutine;
    private Coroutine videoRoutine;

    private bool rightSfxPlayed = false;
    private bool leftSfxPlayed = false;
    public StepManager stepManager;

    // ✅ AUDIO MANAGER
    public AudioManager audioManager;

    void Start()
    {
        TeleportAnchor.SetActive(true);
        StartCoroutine(StartCoroutineFunc());
        
    }

    public IEnumerator StartCoroutineFunc()
    {
        FadeOut(fadeoutImage);
        yield return new WaitForSeconds(5f);
        audioManager.PlayInstruction(WelcomeClip);
        StartCoroutine(StartControllerTraining());
    }

    public void FadeOut(Image img)
    {
        StartCoroutine(FadeRoutine(img));
    }
    IEnumerator FadeRoutine(Image img)
    {
        float duration = 10f;
        float time = 0f;

        Color c = img.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            img.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        img.color = new Color(c.r, c.g, c.b, 0f);
    }
    public IEnumerator StartControllerTraining()
    {
        yield return new WaitForSeconds(13f);
        StartPanel.SetActive(false);
        audioManager.PlayInstruction(TrainingStartClip);
        VideoPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        startGettingInput = true;
        foreach (var s in steps)
        {
            if(startGettingInput)
            {
                s.rightInput.action?.Enable();
                s.leftInput.action?.Enable();

                if (s.rightRenderer != null)
                    s.rightOriginalMat = s.rightRenderer.material;

                if (s.leftRenderer != null)
                    s.leftOriginalMat = s.leftRenderer.material;
            }
        }

        StartStep(index);
    }

    void Update()
    {
        if (startGettingInput)
        {
            if (index >= steps.Count) return;

            Step step = steps[index];

            float rightVal = 0f;
            float leftVal = 0f;

            Vector2 rightAxis = Vector2.zero;
            Vector2 leftAxis = Vector2.zero;

            if (step.type == AnimationType.Joystick)
            {
                rightAxis = step.rightInput.action.ReadValue<Vector2>();
                leftAxis = step.leftInput.action.ReadValue<Vector2>();
            }
            else
            {
                rightVal = step.rightInput.action.ReadValue<float>();
                leftVal = step.leftInput.action.ReadValue<float>();
            }

            bool rightUsed = step.type == AnimationType.Joystick
                ? rightAxis.magnitude > 0.2f
                : rightVal > 0.1f;

            bool leftUsed = step.type == AnimationType.Joystick
                ? leftAxis.magnitude > 0.2f
                : leftVal > 0.1f;

            // ✅ PLAY SFX ON FIRST INPUT
            if (rightUsed && !rightSfxPlayed)
            {
                audioManager?.PlaySFX(step.inputSFX);
                rightSfxPlayed = true;
            }

            if (leftUsed && !leftSfxPlayed)
            {
                audioManager?.PlaySFX(step.inputSFX);
                leftSfxPlayed = true;
            }

            if (rightUsed && leftUsed && !highlightRemoved)
            {
                Highlight(step, false);
                highlightRemoved = true;

                if (demoRoutine != null)
                    StopCoroutine(demoRoutine);
            }

            Animate(step, rightVal, rightAxis, step.rightTarget);
            Animate(step, leftVal, leftAxis, step.leftTarget);

            bool rightComplete = step.type == AnimationType.Joystick
                ? rightAxis.magnitude > 0.8f
                : step.rightInput.action.IsPressed();

            bool leftComplete = step.type == AnimationType.Joystick
                ? leftAxis.magnitude > 0.8f
                : step.leftInput.action.IsPressed();

            if (rightComplete && leftComplete)
            {
                NextStep();
                ResetTransform(step.rightTarget, step.type);
                ResetTransform(step.leftTarget, step.type);
            }
        }
    }

    public void StartStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= steps.Count)
            return;

        index = stepIndex;
        BeginStep();
    }

    public void NextStep()
    {
        index++;
        BeginStep();
    }

    void BeginStep()
    {
        if (index >= steps.Count)
        {
            Debug.Log("Training Finished");
            TeleportAnchor.SetActive(false);
            // ✅ Notify Step Manager
            stepManager.CompleteCurrentStep();
            VideoPanel.SetActive(false);

            return;
        }


        Step step = steps[index];

        highlightRemoved = false;
        rightSfxPlayed = false;
        leftSfxPlayed = false;

        //ResetAll();
        Highlight(step, true);

        // ✅ PLAY INSTRUCTION AUDIO
        if (step.instructionAudio != null)
        {
            audioManager?.PlayInstruction(step.instructionAudio);
        }

        // VIDEO
        if (videoRoutine != null)
            StopCoroutine(videoRoutine);

        videoRoutine = StartCoroutine(PlayVideoSegment(step, index));

        if (demoRoutine != null)
            StopCoroutine(demoRoutine);

        demoRoutine = StartCoroutine(DemoLoop(step));
    }

    IEnumerator PlayVideoSegment(Step step, int stepIndex)
    {
        videoPlayer.Stop();

        while (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            yield return null;
        }

        double fps = videoPlayer.frameRate;

        long startFrame = (long)(step.videoStartTime * fps);
        long endFrame = (long)(step.videoEndTime * fps);

        videoPlayer.frame = startFrame;
        videoPlayer.Play();

        while (videoPlayer.frame < endFrame)
        {
            if (stepIndex != index) yield break;
            yield return null;
        }

        videoPlayer.Pause();
    }

    IEnumerator DemoLoop(Step step)
    {
        while (!highlightRemoved)
        {
            yield return new WaitForSeconds(3f);

            float t = 0f;

            while (t < 1.2f && !highlightRemoved)
            {
                t += Time.deltaTime;
                float v = Mathf.PingPong(t * 2f, 1f);

                Vector2 fakeAxis = new Vector2(Mathf.Sin(t), Mathf.Cos(t));

                Animate(step, v, fakeAxis, step.rightTarget);
                Animate(step, v, fakeAxis, step.leftTarget);

                yield return null;
            }

            ResetTransform(step.rightTarget, step.type);
            ResetTransform(step.leftTarget, step.type);
        }
    }

    void Animate(Step step, float val, Vector2 axis, Transform target)
    {
        if (target == null) return;

        switch (step.type)
        {
            case AnimationType.Trigger:
                target.localRotation = Quaternion.Euler(Mathf.Lerp(0, -30f, val), 0, 0);
                break;

            case AnimationType.Grab:
                target.localScale = Vector3.one * (1f - val * 0.2f);
                break;

            case AnimationType.Joystick:
                target.localRotation = Quaternion.Euler(-axis.y * 15f, 0, -axis.x * 15f);
                break;
        }
    }

    void Highlight(Step step, bool on)
    {
        if (on)
        {
            if (step.rightRenderer != null)
                step.rightRenderer.material = step.highlightMaterial;

            if (step.leftRenderer != null)
                step.leftRenderer.material = step.highlightMaterial;
        }
        else
        {
            if (step.rightRenderer != null)
                step.rightRenderer.material = step.rightOriginalMat;

            if (step.leftRenderer != null)
                step.leftRenderer.material = step.leftOriginalMat;
        }
    }

    void ResetAll()
    {
        foreach (var s in steps)
        {
            if (s.rightRenderer != null)
                s.rightRenderer.material = s.rightOriginalMat;

            if (s.leftRenderer != null)
                s.leftRenderer.material = s.leftOriginalMat;

            ResetTransform(s.rightTarget, s.type);
            ResetTransform(s.leftTarget, s.type);
        }
    }

    void ResetTransform(Transform target, AnimationType type)
    {
        if (target == null) return;

        if (type == AnimationType.Grab)
            target.localScale = Vector3.one;
        else
            target.localRotation = Quaternion.identity;
    }
}
