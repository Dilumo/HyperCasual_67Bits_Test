using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPath : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _minWaitTime = 1f;
    [SerializeField] private float _maxWaitTime = 3f;
    private List<Transform> _patrolPoints = new List<Transform>();
    private int currentPointIndex = 0;

    [SerializeField] private float _repulsionRadius = 1f;
    [SerializeField] private float _repulsionForce = 3f;
    [SerializeField] private float _checkRadius = 1f; // Raio para verificar inimigos próximos do ponto
    [SerializeField] private int _maxEnemiesNearPoint = 1; // Quantidade máxima de inimigos próxima ao ponto
    private bool stopMoving = false;
    private bool isWaiting = false;
    private AnimationStateController _animationStateController;

    void Awake()
    {
        _animationStateController = GetComponent<AnimationStateController>();
    }

    void Start()
    {
        GameObject[] patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint");

        foreach (GameObject point in patrolPoints)
        {
            _patrolPoints.Add(point.transform);
        }
        currentPointIndex = Random.Range(0, _patrolPoints.Count);
    }

    void Update()
    {
        // Se não houver pontos de patrulha ou o inimigo estiver parado ou esperando, ele não faz nada
        if (_patrolPoints.Count == 0 || stopMoving || isWaiting)
        {
            _animationStateController.SetWalking(false);
            return;
        }

        _animationStateController.SetWalking(true);

        Vector3 targetPoint = _patrolPoints[currentPointIndex].position;
        targetPoint.y = transform.position.y;

        // Calcula a direção para o ponto de patrulha
        Vector3 directionToTarget = (targetPoint - transform.position).normalized;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // Move o inimigo em direção ao ponto de patrulha
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, _speed * Time.deltaTime);

        // Verifica a distância até o ponto de patrulha
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
        float marginOfError = _speed * Time.deltaTime * 0.1f;

        // Se o inimigo estiver suficientemente próximo do ponto de patrulha e o ponto estiver congestionado, ele espera
        if (distanceToTarget <= marginOfError)
        {
            if (IsPointCrowded(targetPoint))
            {
                // Se o ponto estiver congestionado, entra em espera
                StartCoroutine(WaitBeforeMovingToNextPoint());
            }
            else
            {
                // Se o ponto não estiver congestionado, escolhe outro ponto aleatório
                StartCoroutine(MoveToNextPoint());
            }
        }

        RepelNearbyEnemies();
    }

    // Método para mover para o próximo ponto após a espera
    private IEnumerator MoveToNextPoint()
    {
        isWaiting = true;

        // Espera por um tempo aleatório antes de mover para o próximo ponto
        float waitTime = Random.Range(_minWaitTime, _maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        int previousIndex = currentPointIndex;
        while (currentPointIndex == previousIndex && _patrolPoints.Count > 1)
        {
            currentPointIndex = Random.Range(0, _patrolPoints.Count);
        }

        isWaiting = false;
    }

    // Método para fazer o inimigo esperar caso o ponto esteja congestionado
    private IEnumerator WaitBeforeMovingToNextPoint()
    {
        isWaiting = true;

        // Espera por um tempo aleatório, mesmo se o ponto estiver congestionado
        float waitTime = Random.Range(_minWaitTime, _maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
    }

    // Verifica se o ponto está congestionado por outros inimigos
    private bool IsPointCrowded(Vector3 point)
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(point, _checkRadius);
        int enemyCount = 0;

        foreach (var collider in nearbyEnemies)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != this.gameObject)
            {
                enemyCount++;
                if (enemyCount >= _maxEnemiesNearPoint)
                    return true; // Se houver mais inimigos do que permitido, considera o ponto congestionado
            }
        }

        return false;
    }

    // Método de repulsão entre inimigos
    void RepelNearbyEnemies()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, _repulsionRadius);

        foreach (var collider in nearbyEnemies)
        {
            if (collider.gameObject != this.gameObject && (collider.CompareTag("Enemy") || collider.CompareTag("Player")))
            {
                Vector3 direction = (transform.position - collider.transform.position).normalized;
                direction.y = 0;

                transform.position += _repulsionForce * Time.deltaTime * direction;
            }
        }
    }
}
