using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimationController : MonoBehaviour
{
    // Referência ao objeto principal do personagem
    [SerializeField] private PlayerPunch playerPunch;

    public void TriggerPunchDamage()
    {
        // Chama o método no objeto do personagem
        if (playerPunch != null)
        {
            playerPunch.ApplyPunchDamage();
        }
    }
}

