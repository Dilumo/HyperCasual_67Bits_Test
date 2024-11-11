using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private int capacityUpgradeCost = 100;
    [SerializeField] private Button upgradeCapacityButton;
    [SerializeField] private AudioClip upgradeSound;
    [SerializeField] private AudioClip errorSound;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        upgradeCapacityButton.onClick.AddListener(UpgradeCapacity);
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
    }

    private void UpgradeCapacity()
    {
        if (GameManager.Instance.GetMoney() >= capacityUpgradeCost)
        {
            GameManager.Instance.AddMoney(-capacityUpgradeCost);
            playerController.IncreaseStackLimit();
            Debug.Log("Capacidade de empilhamento aumentada para: " + playerController.GetStackLimit().ToString());
            PlayUpgradeSound();
        }
        else
        {
            PlayErrorSound();
            Debug.Log("Dinheiro insuficiente para o upgrade.");
        }
    }

    private void PlayUpgradeSound()
    {
        if (audioSource != null && upgradeSound != null)
        {
            audioSource.PlayOneShot(upgradeSound);
        }
    }

    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

}
