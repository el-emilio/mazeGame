using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject panelCharacters;

    public FPSMovement fpsMovement;
    public MonoBehaviour mouseLook;

    public TorchLight torchLight;
    public Attack attack;
    public Map map;

    public void Start()
    {
        fpsMovement.enabled = false;
        mouseLook.enabled = false;

        torchLight.enabled = false;
        if (torchLight.torchLight != null)
            torchLight.torchLight.enabled = false;

        attack.enabled = false;

        if (map != null)
            map.enabled = false;

        panelCharacters.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelectTorch()
    {
        attack.enabled = false;
        if (map != null) map.enabled = false;

        torchLight.enabled = true;
        if (torchLight.torchLight != null)
            torchLight.torchLight.enabled = true;

        StartGame();
    }

    public void SelectAttack()
    {
        if (torchLight.torchLight != null)
            torchLight.torchLight.enabled = false;
        torchLight.enabled = false;

        if (map != null) map.enabled = false;

        attack.enabled = true;
        StartGame();
    }

    public void SelectMap()
    {
        if (torchLight.torchLight != null)
            torchLight.torchLight.enabled = false;
        torchLight.enabled = false;

        attack.enabled = false;

        if (map != null)
            map.enabled = true;

        StartGame();
    }

    private void StartGame()
    {
        panelCharacters.SetActive(false);

        fpsMovement.enabled = true;
        if (mouseLook != null) mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}