using UnityEngine;

public class TravelToPlanet : MonoBehaviour
{
    [Header("Configurações de Viagem")]
    public Transform targetPlanet; // Arraste o planeta para cá no Inspector
    public float speed = 2f;       // Velocidade constante
    public float stopDistance = 5f; // Distância para parar antes de colidir
    public bool isTraveling = false; // Controle para ligar/desligar a viagem

    [Header("Suavização")]
    public bool smoothArrival = true; // Se verdadeiro, desacelera ao chegar perto

    void Update()
    {
        // Só se move se houver um alvo e a viagem estiver ativa
        if (targetPlanet == null || !isTraveling) return;

        // 1. Calcula a direção e a distância
        Vector3 direction = (targetPlanet.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPlanet.position);

        // 2. Verifica se já chegou na distância de parada
        if (distance > stopDistance)
        {
            float currentSpeed = speed;

            // 3. Efeito opcional de desaceleração suave (Ease Out)
            if (smoothArrival && distance < stopDistance + 10f)
            {
                currentSpeed = Mathf.Lerp(0.1f, speed, (distance - stopDistance) / 10f);
            }

            // 4. Move o objeto
            transform.position += direction * currentSpeed * Time.deltaTime;

            // 5. Rotaciona para olhar suavemente para o planeta
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
        else
        {
            // Chegou ao destino
            isTraveling = false;
            Debug.Log("Chegamos ao planeta!");
        }
    }

    // Função pública para você chamar via botão ou outro script
    public void StartJourney(Transform newPlanet)
    {
        targetPlanet = newPlanet;
        isTraveling = true;
    }
}