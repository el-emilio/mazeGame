using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleMapAction;
    public GameObject mapPanel;

    private void OnEnable()
    {
        if (toggleMapAction != null)
            toggleMapAction.action.performed += OnToggleMap;
    }

    private void OnDisable()
    {
        if (toggleMapAction != null)
            toggleMapAction.action.performed -= OnToggleMap;
    }

    private void OnToggleMap(InputAction.CallbackContext ctx)
    {
        if (mapPanel == null) return;

        bool isActive = !mapPanel.activeSelf;
        mapPanel.SetActive(isActive);

        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
