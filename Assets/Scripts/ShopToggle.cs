using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopToggle : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    private bool isShopOpen = false;

    public void ToggleShop()
    {
        if (isShopOpen)
        {
            shopManager.CloseShop();
        }
        else
        {
            shopManager.OpenShop();
        }
        isShopOpen = !isShopOpen;
    }
}
