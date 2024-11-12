using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPunch : MonoBehaviour
{
    [SerializeField] private InputAction _punchAction;
    [SerializeField] private float punchRange = 2f;
    [SerializeField] private float punchForce = 5f;
    [SerializeField] private AudioClip punchSound;
    [SerializeField] private AudioClip wrongPunchSound;
    private AudioSource audioSource;

    private AnimationStateController _animationStateController;

    private void Awake()
    {
        _animationStateController = GetComponent<AnimationStateController>();
        audioSource = GetComponent<AudioSource>();
        _punchAction.performed += InputHandlePunch;
    }

    private void OnEnable()
    {
        _punchAction.Enable();
    }

    private void OnDisable()
    {
        _punchAction.Disable();
    }

    private void InputHandlePunch(InputAction.CallbackContext ctx)
    {
        _animationStateController.SetPunching();
    }

    public void ApplyPunchDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, punchRange);
        bool hitEnemy = false;

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                hitEnemy = true;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    Vector3 punchDirection = (enemy.transform.position - transform.position).normalized;
                    enemyController.ApplyPunch(punchDirection * punchForce);
                    PlayPunchSound();
                }
            }
        }

        if (!hitEnemy)
        {
            PlayWrongPunchSound();
        }
    }

    private void PlayPunchSound()
    {
        if (audioSource != null && punchSound != null)
        {
            audioSource.PlayOneShot(punchSound);
        }
    }

    private void PlayWrongPunchSound()
    {
        if (audioSource != null && wrongPunchSound != null)
        {
            audioSource.PlayOneShot(wrongPunchSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, punchRange);
    }
}
