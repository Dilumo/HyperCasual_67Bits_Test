using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool isPunchable = true; // Verifica se o inimigo pode ser socado
    private bool canBeCollected = false; // Verifica se o inimigo está pronto para ser coletado
    private Rigidbody rb;
    private MovingPath movingPath;
    private RagdollOnDeath ragdollOnDeath;
    private bool isWaitingToCollect = false;
    private bool isCollected = false;

    public bool IsPunchable()
    {
        return isPunchable;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movingPath = GetComponent<MovingPath>();
        ragdollOnDeath = GetComponent<RagdollOnDeath>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isWaitingToCollect || isCollected) return;
        Debug.Log("Colisão com: " + collision.gameObject.tag);
        // Verifica se o inimigo colidiu com o chão (tag "Ground") e habilita a coleta
        if (!isPunchable && collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(WaitUntilStopped());
        }
    }

    // Coroutine para esperar até que o inimigo pare de se mover antes de permitir a coleta
    private IEnumerator WaitUntilStopped()
    {
        isWaitingToCollect = true;

        // Aguarda até que a velocidade do Rigidbody esteja próxima de zero (parou de se mover)
        while (rb.velocity.magnitude > 0.5f)
        {
            yield return null; // Espera até o próximo frame
        }

        // Define que o inimigo agora pode ser coletado
        canBeCollected = true;
        isWaitingToCollect = false;
    }

    // Função chamada pelo soco do jogador
    public void ApplyPunch(Vector3 punchForce)
    {
        Debug.Log("Inimigo socado!");
        if (!isPunchable) return; // Impede socos múltiplos

        rb.isKinematic = false; // Permite que a física seja aplicada
        movingPath.enabled = false; // Desativa o script MovingPath
        rb.AddForce(punchForce, ForceMode.Impulse); // Aplica a força do soco
        isPunchable = false; // Impede socos múltiplos
        ragdollOnDeath.isDead = true; // Ativa o ragdoll
    }

    public bool CanBeCollected()
    {
        return canBeCollected;
    }

    // Função para ser chamada pelo jogador ao tentar coletar
    public void Collect()
    {
        if (canBeCollected)
        {
            Debug.Log("Inimigo coletado!");

            // Reseta o ragdoll antes de coletar
            ragdollOnDeath.ResetRagdoll();

            if (ragdollOnDeath.animator != null)
            {
                ragdollOnDeath.animator.enabled = false; // Desativa o Animator para manter em T-pose
            }

            rb.isKinematic = true; // Desativa a física ao ser coletado
            canBeCollected = false; // Impede a coleta múltipla
            isCollected = true;
            movingPath.enabled = false; // Desativa o script MovingPath
        }
    }
}
