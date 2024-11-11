using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Arraste o transform do personagem no Inspector
    [SerializeField] private Vector3 offset = new Vector3(-5, 12, -10); // Ajuste para uma visão elevada e lateral
    [SerializeField] private float smoothSpeed = 0.125f; // Velocidade de suavização do movimento

    void LateUpdate()
    {
        if (target == null) return;

        // Calcula a posição desejada da câmera com o offset ajustado
        Vector3 desiredPosition = target.position + offset;

        // Suaviza o movimento entre a posição atual e a posição desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Mantém a câmera focada no personagem
        transform.LookAt(target.position + Vector3.up * 2); // Ajuste leve na posição de foco
    }
}
