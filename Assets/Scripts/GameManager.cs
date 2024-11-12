using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Text moneyText;
    [SerializeField] private Text stacksText;
    [SerializeField] private AudioSource backgroundMusic; // Referência ao AudioSource
    [SerializeField] private AudioClip musicClip; // Música do jogo

    private int money = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoneyText();

        // Toca a música de fundo se não estiver tocando
        if (backgroundMusic != null && musicClip != null)
        {
            backgroundMusic.clip = musicClip;
            backgroundMusic.loop = true; // Faz a música tocar em loop
            backgroundMusic.Play();
        }
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void UpdateStacksText(int currentStacks, int maxStacks)
    {
        if (stacksText != null)
        {
            stacksText.text = $"{currentStacks}/{maxStacks}";
        }
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money.ToString()} $";
        }
    }
}
