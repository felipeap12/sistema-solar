using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 5f; // distância mínima

    private Vector3 targetPosition;
    private bool moving = false;
    private Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        var mouse = Mouse.current;

        if (mouse == null || cam == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform planet = hit.collider.transform;

                // direção do planeta até você
                Vector3 dir = (transform.position - planet.position).normalized;

                // distância baseada no tamanho do planeta
                float radius = planet.localScale.x * 0.5f;

                float finalDistance = radius + stopDistance;

                targetPosition = planet.position + dir * finalDistance;

                moving = true;
            }
        }

        if (moving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
            {
                moving = false;
            }
        }
    }
}