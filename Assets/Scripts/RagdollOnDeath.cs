using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    public Animator animator;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    // Configuração para saber se o ragdoll está ativado
    public bool isDead = false;

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        // Desativa a física do ragdoll no início
        SetRagdollState(false);
        GetComponent<Collider>().enabled = true;
    }

    void Update()
    {
        if (isDead)
        {
            // Quando morrer, ativa o ragdoll
            SetRagdollState(true);
        }
    }

    // Função para ativar ou desativar o ragdoll
    public void SetRagdollState(bool state)
    {
        animator.enabled = !state;  // Desativa o Animator para parar as animações

        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !state;  // Ativa ou desativa a física
        }

        foreach (var col in colliders)
        {
            col.enabled = state;  // Habilita ou desabilita os colliders
        }
    }

    // Função para redefinir o estado do ragdoll e preparar o inimigo para ser empilhado
    public void ResetRagdoll()
    {
        isDead = false;
        SetRagdollState(false);
    }
}
