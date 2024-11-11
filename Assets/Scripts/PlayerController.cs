using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int stackLimit = 3;
    [SerializeField] private Transform backStackPosition; // Ponto nas costas do jogador
    [SerializeField] private float stackSpacing = 0.01f;
    [SerializeField] private int maxStackSize = 5;
    [SerializeField] private float positionSmoothSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 5f;

    private List<GameObject> collectedEnemies = new List<GameObject>();
    [SerializeField] private Vector3 initialPositionOffset = new Vector3(0.68f, 0.96f, -0.59f);
    [SerializeField] private Vector3 initialRotation = new Vector3(0f, 0f, 90f);

    private void Update()
    {
        CheckForEnemiesToCollect();
    }

    private void CheckForEnemiesToCollect()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider col in nearbyEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                AttemptCollect(col.gameObject);
            }
        }
    }

    private void AttemptCollect(GameObject enemy)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null && enemyController.CanBeCollected() && collectedEnemies.Count < maxStackSize && !StackLimitReached())
        {
            collectedEnemies.Add(enemy);
            ResetToTPose(enemy);
            PositionInStack(enemy);
            enemyController.Collect();
        }
    }

    private bool StackLimitReached()
    {
        return collectedEnemies.Count >= stackLimit;
    }

    private void PositionInStack(GameObject enemy)
    {
        Transform pivot = enemy.transform.Find("PivôCentralizado");

        Vector3 stackPosition = backStackPosition.position + initialPositionOffset + Vector3.up * stackSpacing * collectedEnemies.Count;
        // Mantendo a rotação alinhada com a rotação do jogador, ajustando para a "costa"
        Quaternion stackRotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);  // 90 graus no eixo X, rotação do jogador no eixo Y

        if (pivot != null)
        {
            pivot.position = stackPosition;
            pivot.rotation = stackRotation;
        }
        else
        {
            Debug.LogWarning("O pivô centralizado não foi encontrado no inimigo.");
        }

        AdjustColliderSize(enemy, 0.001f);
        enemy.transform.SetParent(backStackPosition);
    }

    public void UpdateStackInertia()
    {
        Vector3 targetBasePosition = backStackPosition.position;
        Quaternion targetBaseRotation = backStackPosition.rotation;

        float timeFactor = Time.time * positionSmoothSpeed;

        for (int i = 0; i < collectedEnemies.Count; i++)
        {
            GameObject enemy = collectedEnemies[i];

            // Calcule a posição vertical desejada
            Vector3 stackPosition = targetBasePosition + Vector3.up * stackSpacing * i;

            // Adicione uma oscilação horizontal, com amplitude maior quanto mais alto o item estiver na pilha
            float oscillationAmount = Mathf.Sin(timeFactor + i * 0.5f) * 0.1f * (i + 1); // Oscilação aumenta com a altura
            Vector3 oscillationOffset = new Vector3(oscillationAmount, 0, 0);

            // Ajuste a posição final adicionando a oscilação
            Vector3 targetPosition = stackPosition + oscillationOffset;

            // Suaviza a transição para a posição desejada com oscilação
            enemy.transform.position = Vector3.Lerp(enemy.transform.position, targetPosition, Time.deltaTime * positionSmoothSpeed);

            // A rotação de 90 graus inicial no eixo X é fixa
            float oscillationRotationZ = Mathf.Sin(timeFactor + i * 0.5f) * 10f; // Oscilação de -10 a 10 graus no eixo Z
            float oscillationRotationY = Mathf.Sin(timeFactor + i * 0.5f) * 10f; // Oscilação de -10 a 10 graus no eixo Y

            // A rotação final no eixo X é sempre 90 graus, e no eixo Z ela oscila
            float targetXRotation = 90f; // Fixo no eixo X
            float targetZRotation = 90f + oscillationRotationZ; // Variando no eixo Z entre 80 e 100 graus
            float targetYRotation = transform.eulerAngles.y + oscillationRotationY; // Baseando a rotação Y na rotação do jogador

            // Suaviza a rotação para dar o efeito de balanço
            Quaternion targetRotation = Quaternion.Euler(targetXRotation, targetYRotation, targetZRotation);

            // Aplica a rotação suavizada no inimigo
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }
    }



    private void ResetToTPose(GameObject enemy)
    {
        Animator animator = enemy.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("T-Pose Animation Name"); // Substitua pelo nome exato da animação T-Pose
        }

        RagdollOnDeath ragdoll = enemy.GetComponent<RagdollOnDeath>();
        if (ragdoll != null)
        {
            ragdoll.SetRagdollState(false);
            ragdoll.isDead = false;
        }
    }

    private void AdjustColliderSize(GameObject enemy, float scaleMultiplier)
    {
        Collider enemyCollider = enemy.GetComponent<Collider>();
        if (enemyCollider != null)
        {
            if (enemyCollider is BoxCollider boxCollider)
            {
                boxCollider.size *= scaleMultiplier;
            }
            else if (enemyCollider is SphereCollider sphereCollider)
            {
                sphereCollider.radius *= scaleMultiplier;
            }
        }
    }

    public void ClearEnemies()
    {
        foreach (var enemy in collectedEnemies)
        {
            Destroy(enemy);
        }
        collectedEnemies.Clear();
    }

    public int CollectedEnemyCount()
    {
        return collectedEnemies.Count;
    }

    public void IncreaseStackLimit()
    {
        stackLimit++;
        Debug.Log("Novo limite de empilhamento: " + stackLimit);
    }

    internal int GetStackLimit()
    {
        return stackLimit;
    }

    // Pega o inimigo na posição do índice
    public GameObject GetEnemyAt(int index)
    {
        if (index >= 0 && index < collectedEnemies.Count)
        {
            return collectedEnemies[index];
        }
        else
        {
            Debug.LogWarning("Índice de inimigo inválido: " + index);
        }
        return null;
    }

    public GameObject GetFirstEnemy()
    {
        if (collectedEnemies.Count > 0)
        {
            return collectedEnemies[0];
        }
        return null;
    }

    // Remove o inimigo na posição do índice
    public void RemoveEnemyAt(int index)
    {
        if (index >= 0 && index < collectedEnemies.Count)
        {
            collectedEnemies.RemoveAt(index);
        }
    }

    internal void RemoveEnemy(GameObject enemyToRemove)
    {
        collectedEnemies.Remove(enemyToRemove);
    }
}
