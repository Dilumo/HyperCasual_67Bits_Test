using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private int rewardPerEnemy = 10; // Valor de recompensa por cada inimigo entregue
    private PlayerController playerCollector;

    // Áudios
    [SerializeField] private AudioClip loadingSound; // Som de carregando
    [SerializeField] private AudioClip rewardSound; // Som de ganhar dinheiro
    private AudioSource audioSource;

    [SerializeField] private float depositDelay = 1f; // Tempo entre cada inimigo
    [SerializeField] private float collectionDuration = 2f; // Tempo de coleta de cada inimigo na zona (tempo de "carregamento")


    private bool isDepositing = false;
    private bool inZone = false;


    private void Start()
    {
        playerCollector = FindObjectOfType<PlayerController>(); // Acessa o script do jogador
        audioSource = GetComponent<AudioSource>(); // Obtém o AudioSource do objeto (se você tiver um no próprio objeto da zona)
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!inZone)
        {
            isDepositing = false;
            StopCoroutine(DepositEnemiesGradually());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDepositing) return; // Impede que a coleta seja iniciada novamente enquanto estiver em andamento
        if (other.CompareTag("Player") && playerCollector != null)
        {
            StartCoroutine(DepositEnemiesGradually()); // Inicia a coleta gradual
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isDepositing)
        {
            // Interrompe a coleta se o jogador sair da zona
            isDepositing = false;
            inZone = false;
            StopCoroutine(DepositEnemiesGradually()); // Interrompe a corrotina de coleta
        }
    }

    private IEnumerator DepositEnemiesGradually()
    {
        int enemyCount = playerCollector.CollectedEnemyCount();
        int reward = enemyCount * rewardPerEnemy;
        int enemiesDeposited = 0;

        isDepositing = true;
        inZone = true;

        List<GameObject> enemiesToRemove = new List<GameObject>();

        while (enemyCount > 0)
        {
            if (isDepositing == false)
            {
                inZone = false;
                yield break;
            }

            GameObject enemy = playerCollector.GetFirstEnemy();
            if (enemy != null)
            {
                // Toca o som de carregando quando entra na zona
                PlayLoadingSound();
                // Simula o tempo de coleta do inimigo
                yield return new WaitForSeconds(collectionDuration);


                // Toca o som de ganhar dinheiro após a coleta
                PlayRewardSound();

                // Adiciona recompensa ao jogador
                GameManager.Instance.AddMoney(rewardPerEnemy);
                enemiesDeposited++;
                enemy.SetActive(false);
                enemyCount--;
            }

            playerCollector.RemoveEnemy(enemy);
            Destroy(enemy); // Remove o inimigo da cena
        }

        // Toca o som de recompensa final e atualiza a recompensa total
        playerCollector.ClearEnemies(); // Limpa os inimigos do jogador após o depósito
    }

    // Função para tocar o som de carregando
    private void PlayLoadingSound()
    {
        if (audioSource != null && loadingSound != null)
        {
            audioSource.PlayOneShot(loadingSound); // Toca o som de carregando
        }
    }

    // Função para tocar o som de ganhar dinheiro
    private void PlayRewardSound()
    {
        if (audioSource != null && rewardSound != null)
        {
            audioSource.PlayOneShot(rewardSound); // Toca o som de ganhar dinheiro
        }
    }
}
