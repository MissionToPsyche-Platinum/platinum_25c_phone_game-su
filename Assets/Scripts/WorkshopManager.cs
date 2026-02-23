using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image previewImage;
    public Button equipButton;
    public Text equipButtonText;

    [Header("Skin Buttons")]
    public Button defaultSkinButton;
    public Button blueSkinButton;

    [Header("Skin Colors")]
    public Color defaultSkinColor = Color.white;
    public Color blueSkinColor = Color.blue;

    private int previewedSkin = 0; // Currently previewed skin (0=default, 1=blue)
    private int equippedSkin = 0; // Currently equipped skin

    void Start()
    {
        // Load equipped skin from GameStateManager
        equippedSkin = GameStateManager.Instance.GetProbeSkin();
        previewedSkin = equippedSkin;

        // Connect skin selection buttons
        if (defaultSkinButton != null)
            defaultSkinButton.onClick.AddListener(() => PreviewSkin(0));
        if (blueSkinButton != null)
            blueSkinButton.onClick.AddListener(() => PreviewSkin(1));

        // Connect equip button
        if (equipButton != null)
            equipButton.onClick.AddListener(EquipPreviewedSkin);

        UpdateUI();
    }

    void PreviewSkin(int skinIndex)
    {
        previewedSkin = skinIndex;
        Debug.Log("Previewing skin: " + skinIndex);
        UpdateUI();
    }

    void EquipPreviewedSkin()
    {
        equippedSkin = previewedSkin;
        GameStateManager.Instance.SetProbeSkin(equippedSkin);
        Debug.Log("Equipped skin: " + equippedSkin);
        UpdateUI();
    }

    void UpdateUI()
    {
        // Update preview image with probe sprite AND color
        if (previewImage != null)
        {
            // Find the probe to get its sprite
            GameObject probe = GameObject.Find("Probe");
            if (probe != null)
            {
                SpriteRenderer probeRenderer = probe.GetComponent<SpriteRenderer>();
                if (probeRenderer != null)
                {
                    // Copy the probe's sprite to preview
                    previewImage.sprite = probeRenderer.sprite;
                }
            }

            // Apply the skin color
            switch (previewedSkin)
            {
                case 0:
                    previewImage.color = defaultSkinColor;
                    break;
                case 1:
                    previewImage.color = blueSkinColor;
                    break;
            }
        }

        // Update equip button state
        if (equipButton != null && equipButtonText != null)
        {
            if (previewedSkin == equippedSkin)
            {
                equipButton.interactable = false;
                equipButtonText.text = "EQUIPPED";
            }
            else
            {
                equipButton.interactable = true;
                equipButtonText.text = "EQUIP";
            }
        }
    }
}