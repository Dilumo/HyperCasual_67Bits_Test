using System.Collections.Generic;
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

    [System.Serializable]
    public class ColorOption
    {
        public string colorName;
        public Color color;
        public int price;
    }

    [SerializeField] private List<ColorOption> colorOptions;
    [SerializeField] private GameObject colorButtonPrefab; // Prefab do botão de cor
    [SerializeField] private Transform buttonContainer; // Contêiner onde os botões serão instanciados
    [SerializeField] private float buttonSpacing = 100f; // Espaçamento entre os botões

    private List<string> unlockedColors = new List<string>(); // Lista de cores desbloqueadas

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        upgradeCapacityButton.onClick.AddListener(UpgradeCapacity);
        CreateColorButtons(); // Instancia os botões ao iniciar
    }

    private void CreateColorButtons()
    {
        float containerWidth = buttonContainer.GetComponent<RectTransform>().rect.width; // Largura do container
        float totalWidth = 0f; // Largura total necessária para acomodar todos os botões
        for (int i = 0; i < colorOptions.Count; i++)
        {
            ColorOption colorOption = colorOptions[i];
            GameObject buttonObj = Instantiate(colorButtonPrefab, buttonContainer);

            // Obtém o tamanho do botão para ajustar o posicionamento
            RectTransform buttonRectTransform = buttonObj.GetComponent<RectTransform>();
            float buttonWidth = buttonRectTransform.rect.width;

            // Calcula a posição considerando o índice, largura do botão e espaçamento
            // Posiciona os botões de forma que eles se ajustem ao container
            totalWidth = (buttonWidth + buttonSpacing) * i;

            // Verifica se a largura total não excede o container
            if (totalWidth + buttonWidth > containerWidth)
            {
                // Se ultrapassar a largura do container, redefine o espaçamento
                buttonSpacing = Mathf.Max(0.5f, (containerWidth - buttonWidth * colorOptions.Count) / (colorOptions.Count - 1));
                totalWidth = (buttonWidth + buttonSpacing) * i; // Recalcula a largura total com o novo espaçamento
            }

            buttonObj.transform.localPosition = new Vector3(totalWidth, 0, 0);

            // Configura as informações do botão
            Button colorButton = buttonObj.GetComponent<Button>();
            colorButton.onClick.AddListener(() => BuyColor(colorOption.colorName));

            Text nameLabel = buttonObj.transform.Find("lblColorName").GetComponent<Text>();
            Text priceLabel = buttonObj.transform.Find("lblColorPrice").GetComponent<Text>();
            Image lockedImage = buttonObj.transform.Find("LockedImage").GetComponent<Image>();

            nameLabel.text = colorOption.colorName;
            priceLabel.text = $"{colorOption.price} $";

            // Define a cor de fundo do botão de acordo com a cor da opção
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = colorOption.color; // Define a cor do botão
            }

            // Verifica se a cor está desbloqueada e atualiza o botão
            bool isUnlocked = unlockedColors.Contains(colorOption.colorName);
            lockedImage.gameObject.SetActive(!isUnlocked);
            priceLabel.gameObject.SetActive(!isUnlocked);
        }
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
            PlayUpgradeSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    private void BuyColor(string colorName)
    {
        ColorOption colorOption = colorOptions.Find(c => c.colorName == colorName);
        if (colorOption == null) return;

        // Verifica se a cor já foi comprada
        if (!unlockedColors.Contains(colorName))
        {
            // Verifica se o jogador tem dinheiro suficiente para comprar a cor
            if (GameManager.Instance.GetMoney() >= colorOption.price)
            {
                GameManager.Instance.AddMoney(-colorOption.price); // Deduz o dinheiro
                unlockedColors.Add(colorName); // Adiciona à lista de cores desbloqueadas
                PlayUpgradeSound(); // Toca o som de upgrade

                // Atualiza o botão para agora aplicar a cor
                UpdateColorButton(colorOption); // Atualiza o botão para aplicar a cor
            }
            else
            {
                PlayErrorSound(); // Toca o som de erro se não tiver dinheiro suficiente
            }
        }
        else
        {
            // Se a cor já foi comprada, apenas a aplica
            ApplyColorToPlayer(colorOption.color);
        }
    }

    private void UpdateColorButton(ColorOption colorOption)
    {
        // Atualiza os botões de cor
        foreach (Transform buttonTransform in buttonContainer)
        {
            string colorName = buttonTransform.Find("lblColorName").GetComponent<Text>().text;
            if (colorName == colorOption.colorName)
            {
                Button colorButton = buttonTransform.GetComponent<Button>();
                Text nameLabel = buttonTransform.Find("lblColorName").GetComponent<Text>();
                Text priceLabel = buttonTransform.Find("lblColorPrice").GetComponent<Text>();
                Image lockedImage = buttonTransform.Find("LockedImage").GetComponent<Image>();

                // Altera a função do botão para aplicar a cor
                colorButton.onClick.RemoveAllListeners(); // Remove os listeners antigos
                colorButton.onClick.AddListener(() => ApplyColorToPlayer(colorOption.color)); // Adiciona o listener para aplicar a cor

                // Atualiza o texto do botão para "Aplicar"
                nameLabel.text = "Apply " + colorOption.colorName;
                priceLabel.gameObject.SetActive(false); // Esconde o preço
                lockedImage.gameObject.SetActive(false); // Esconde a imagem de bloqueio
            }
        }
    }

    private void ApplyColorToPlayer(Color color)
    {
        // Aplica a cor no personagem
        playerController.SetColor(color);
    }

    private void RefreshColorButtons()
    {
        // Atualiza todos os botões conforme o status de desbloqueio
        foreach (Transform buttonTransform in buttonContainer)
        {
            string colorName = buttonTransform.Find("lblColorName").GetComponent<Text>().text;
            bool isUnlocked = unlockedColors.Contains(colorName);

            Image lockedImage = buttonTransform.Find("LockedImage").GetComponent<Image>();
            Text priceLabel = buttonTransform.Find("lblColorPrice").GetComponent<Text>();

            lockedImage.gameObject.SetActive(!isUnlocked);
            priceLabel.gameObject.SetActive(!isUnlocked);
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
