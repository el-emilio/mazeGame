using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject enemy;
    public int clicksNeeded = 3;
    public float maxDistance = 10f;

    private int clicks = 0;
    private bool enemyStopped = false;

    void Update()
    {
        if (enemyStopped) return;

        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.transform.IsChildOf(enemy.transform))
                {
                    clicks++;

                    if (clicks >= clicksNeeded)
                    {
                        StopEnemy();
                    }
                }
            }
        }
    }

    void StopEnemy()
    {
        MonoBehaviour[] scripts = enemy.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        enemyStopped = true;
    }
}