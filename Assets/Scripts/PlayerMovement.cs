using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private InputAction _moveAction;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private AnimationStateController _animaitonStateController;
    private PlayerController playerController;  // Referência para o PlayerController

    private float _initialY; // Armazena a altura inicial do jogador

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _animaitonStateController = GetComponent<AnimationStateController>();
        playerController = GetComponent<PlayerController>();  // Certifique-se de ter uma referência para o PlayerController
        _initialY = transform.position.y; // Salva a altura inicial do jogador
    }

    private void Update()
    {
        Vector2 move = _moveAction.ReadValue<Vector2>();
        Vector3 moveV3 = new(move.x, 0, move.y);

        if (moveV3 != Vector3.zero)
        {
            _animaitonStateController.SetRunning(true);
            transform.forward = moveV3;
        }
        else
        {
            _animaitonStateController.SetRunning(false);
        }

        // Move o personagem
        _controller.Move(_speed * Time.deltaTime * moveV3);

        // Força o jogador a manter a altura inicial para evitar flutuação
        Vector3 position = transform.position;
        position.y = _initialY; // Mantém o valor de y constante
        transform.position = position;

        // Chame o método UpdateStackInertia do PlayerController aqui
        playerController.UpdateStackInertia();  // Certifique-se de ter uma referência para o PlayerController
    }

    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
}
