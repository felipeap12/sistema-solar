using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public float acceleration = 5f;
    public float maxSpeed = 15f;
    public float damping = 0.95f;

    [Header("Rotação (Botão Esquerdo)")]
    public float mouseSensitivity = 0.15f;
    private float rotationX = 0f;

    [Header("Zoom (Scroll no Mouse)")]
    public float zoomSensitivity = 30f;
    public float minFOV = 10f;
    public float maxFOV = 60f;
    public float zoomSmoothness = 10f;

    [Range(0.1f, 2f)]
    public float zoomPushForce = 1.2f;

    private float targetFOV;
    private Vector3 velocity;
    private Transform xrOrigin;
    private Camera cam;

    void Start()
    {
        // Tenta pegar o XR Origin. Se não houver pai, usa o próprio objeto.
        if (transform.parent != null && transform.parent.parent != null)
            xrOrigin = transform.parent.parent;
        else
            xrOrigin = transform;

        cam = GetComponent<Camera>();
        targetFOV = cam.fieldOfView;

        // Inicializa a rotação X com a rotação atual da câmera para evitar pulos
        rotationX = transform.localEulerAngles.x;
        if (rotationX > 180) rotationX -= 360;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard == null || mouse == null || xrOrigin == null || cam == null) return;

        // 1. MOVIMENTAÇÃO
        HandleStandardMovement(keyboard);

        // 2. ROTAÇÃO (Agora com Vertical e Horizontal garantidos)
        if (mouse.leftButton.isPressed)
        {
            HandleRotation(mouse);
        }

        // 3. SMART ZOOM
        HandleSmartZoom(mouse);
    }

    void HandleStandardMovement(Keyboard k)
    {
        Vector3 input = Vector3.zero;
        if (k.wKey.isPressed) input += transform.forward;
        if (k.sKey.isPressed) input -= transform.forward;
        if (k.aKey.isPressed) input -= transform.right;
        if (k.dKey.isPressed) input += transform.right;
        if (k.eKey.isPressed) input += transform.up;
        if (k.qKey.isPressed) input -= transform.up;

        velocity += input * acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        xrOrigin.position += velocity * Time.deltaTime;
        velocity *= damping;
    }

    void HandleRotation(Mouse m)
    {
        Vector2 delta = m.delta.ReadValue();

        // Rotação Vertical (Pitch) - Afeta apenas a Câmera localmente
        rotationX -= delta.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -85f, 85f); // Limite para não virar o pescoço demais

        // Aplica a rotação vertical na câmera
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotação Horizontal (Yaw) - Gira o corpo (xrOrigin)
        xrOrigin.Rotate(Vector3.up * (delta.x * mouseSensitivity));
    }

    void HandleSmartZoom(Mouse m)
    {
        float scroll = m.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.1f)
        {
            targetFOV -= scroll * (zoomSensitivity * 0.05f);
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);

            if (scroll > 0)
            {
                Ray ray = cam.ScreenPointToRay(new Vector3(m.position.ReadValue().x, m.position.ReadValue().y, 0));
                velocity += ray.direction * zoomPushForce;
            }
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSmoothness);
    }
}