using UnityEngine;
using UnityEngine.InputSystem;


public class CleanTheSludge : MonoBehaviour
{
    [Header("References")]
    public Transform sprayPoint;          // nozzle tip
    public float sprayDistance = 3f;

    public Texture2D dirtMaskBase;
    public Texture2D brush;
    public Material targetMaterial;

    private Texture2D runtimeMask;
    [Header("XR Input")]
    public InputActionProperty rightTrigger;

    void Start()
    {
        CreateTexture();
    }

    void Update()
    {
        if (rightTrigger.action.ReadValue<float>() > 0.1f)
        {
            Spray();
        }
    }


    void Spray()
    {
        Ray ray = new Ray(sprayPoint.position, sprayPoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, sprayDistance))
        {
            Vector2 uv = hit.textureCoord;

            int pixelX = (int)(uv.x * runtimeMask.width);
            int pixelY = (int)(uv.y * runtimeMask.height);

            ApplyBrush(pixelX, pixelY);
        }
    }

    void ApplyBrush(int centerX, int centerY)
    {
        for (int x = 0; x < brush.width; x++)
        {
            for (int y = 0; y < brush.height; y++)
            {
                int px = centerX + x - brush.width / 2;
                int py = centerY + y - brush.height / 2;

                // ✅ IMPORTANT: Bounds check
                if (px < 0 || px >= runtimeMask.width || py < 0 || py >= runtimeMask.height)
                    continue;

                Color maskPixel = runtimeMask.GetPixel(px, py);
                Color brushPixel = brush.GetPixel(x, y);

                // 👉 Gradual cleaning (not instant erase)
                float strength = brushPixel.g * 0.1f;

                maskPixel.g = Mathf.Lerp(maskPixel.g, 0, strength);

                runtimeMask.SetPixel(px, py, maskPixel);
            }
        }

        runtimeMask.Apply();
    }

    void CreateTexture()
    {
        runtimeMask = new Texture2D(dirtMaskBase.width, dirtMaskBase.height);
        runtimeMask.SetPixels(dirtMaskBase.GetPixels());
        runtimeMask.Apply();

        targetMaterial.SetTexture("DirtTexture", runtimeMask);
    }
}
