using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool IsDead { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Atajo: presiona R para reiniciar cuando estes muerto
        if (IsDead && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            Restart();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log("Jugador vida: " + currentHealth);

        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        IsDead = true;
        Debug.Log("El jugador murio");

        // Desactiva movimiento, camara y demas scripts del jugador
        foreach (MonoBehaviour mb in GetComponentsInChildren<MonoBehaviour>())
            if (mb != this) mb.enabled = false;

        CreateGameOverUI();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // congela el juego (tambien al enemigo)
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ---------- UI creada por codigo ----------
    void CreateGameOverUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasGO = new GameObject("GameOverCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Fondo oscuro rojizo
        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        Stretch(bg.GetComponent<RectTransform>());
        bg.GetComponent<Image>().color = new Color(0.4f, 0f, 0f, 0.75f);

        // Titulo
        CreateText("Title", canvasGO.transform, font, "GAME OVER", 90, new Vector2(0, 80), new Vector2(900, 150));

        // Boton
        GameObject btn = new GameObject("RestartButton", typeof(RectTransform), typeof(Image), typeof(Button));
        btn.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(320, 80);
        rt.anchoredPosition = new Vector2(0, -60);
        btn.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
        btn.GetComponent<Button>().onClick.AddListener(Restart);
        CreateText("Label", btn.transform, font, "Reiniciar", 40, Vector2.zero, new Vector2(320, 80)).color = Color.black;

        // Ayuda
        CreateText("Hint", canvasGO.transform, font, "(o presiona R)", 28, new Vector2(0, -140), new Vector2(400, 50));
    }

    Text CreateText(string name, Transform parent, Font font, string content, int size, Vector2 pos, Vector2 boxSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = boxSize;

        Text t = go.GetComponent<Text>();
        t.font = font;
        t.text = content;
        t.fontSize = size;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        return t;
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
