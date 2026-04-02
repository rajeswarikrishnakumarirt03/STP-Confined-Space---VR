using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationController : MonoBehaviour
{
    [System.Serializable]
    public class AnimatorInputBinding
    {
        [Tooltip("XR Input Action (Trigger / Grip etc)")]
        public InputActionReference inputAction;

        [Tooltip("Animator that receives the value")]
        public Animator animator;

        [Tooltip("Animator parameter name")]
        public string parameterName;

        [Tooltip("Parameter Type")]
        public ParameterType parameterType;
    }

    public enum ParameterType
    {
        Float,
        Bool
    }

    [Header("Bindings")]
    public AnimatorInputBinding[] bindings;

    void OnEnable()
    {
        foreach (var binding in bindings)
        {
            if (binding.inputAction == null) continue;

            binding.inputAction.action.performed += ctx =>
            {
                ApplyValue(binding, ctx);
            };

            binding.inputAction.action.canceled += ctx =>
            {
                ApplyValue(binding, ctx);
            };

            binding.inputAction.action.Enable();
        }
    }

    void OnDisable()
    {
        foreach (var binding in bindings)
        {
            if (binding.inputAction == null) continue;

            binding.inputAction.action.Disable();
        }
    }

    void ApplyValue(AnimatorInputBinding binding, InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();

        if (binding.parameterType == ParameterType.Float)
        {
            binding.animator.SetFloat(binding.parameterName, value);
        }
        else if (binding.parameterType == ParameterType.Bool)
        {
            binding.animator.SetBool(binding.parameterName, value > 0.1f);
        }
    }
}
