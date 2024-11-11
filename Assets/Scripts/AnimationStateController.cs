using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _isWalking = false;

    public void SetWalking(bool isWalking)
    {
        _isWalking = isWalking;
        _animator.SetBool("isWalking", _isWalking);
    }

    public void SetRunning(bool isRunning)
    {
        _animator.SetBool("isRunning", isRunning);
    }

    public void SetPunching()
    {
        _animator.SetTrigger("isPunch");
    }
}
