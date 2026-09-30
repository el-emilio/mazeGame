using UnityEngine;
using UnityEngine.Rendering;

// Pon este script en cualquier objeto de la escena (por ejemplo en un GameObject vacío llamado "Ambience").
// Deja la escena oscura, con niebla y luz de luna muy tenue. Todo se puede ajustar desde el Inspector.
[ExecuteAlways]
public class HorrorAmbience : MonoBehaviour
{
    [Header("Luz direccional (luna)")]
    public Light directionalLight;
    public Color moonColor = new Color(0.35f, 0.45f, 0.75f);
    [Range(0f, 1f)] public float moonIntensity = 0.08f;

    [Header("Luz ambiental")]
    public Color ambientColor = new Color(0.02f, 0.03f, 0.05f);

    [Header("Niebla")]
    public bool useFog = true;
    public Color fogColor = new Color(0.02f, 0.03f, 0.04f);
    [Range(0f, 0.3f)] public float fogDensity = 0.06f;

    [Header("Reflejos")]
    public bool disableReflections = true;

    void OnEnable() { Apply(); }
    void OnValidate() { Apply(); }

    void Apply()
    {
        if (directionalLight == null)
            directionalLight = FindFirstObjectByType<Light>();

        // Luz de luna
        var lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional)
            {
                l.color = moonColor;
                l.intensity = moonIntensity;
                l.shadows = LightShadows.Soft;
            }
        }

        // Ambiente plano y oscuro (sin skybox brillante)
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;

        // Niebla
        RenderSettings.fog = useFog;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        // Sin reflejos del entorno (evita el brillo en las texturas)
        if (disableReflections)
        {
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.reflectionIntensity = 0f;
        }

        // Fondo negro para la cámara principal
        var cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = fogColor;
        }
    }
}
