using UnityEngine;
using UnityEngine.UI;

public class CollisionToggleButton : MonoBehaviour
{
    public LayerMask m_IgnoreLayer1;
    public LayerMask m_IgnoreLayer2;

    [SerializeField] private Image m_Background;
    [SerializeField] private Image m_Checkmark;

    [SerializeField] private Sprite m_EnabledBackground;
    [SerializeField] private Sprite m_DisabledBackground;

    private bool m_IsToggleUnlocked = true;

    public void SetToggleUnlocked(bool isUnlocked)
    {
        m_IsToggleUnlocked = isUnlocked;
        m_Background.raycastTarget = isUnlocked;
        m_Checkmark.raycastTarget = m_IsToggleUnlocked;

        if (!isUnlocked)
        {
            m_Background.sprite = m_DisabledBackground;
        }
        else
        {
            m_Background.sprite = m_EnabledBackground;
        }
    }

    public void ToggleCollision(bool isEnabled)
    {
        // Get index of layer from LayerMask
        int layerNumber = 0;
        int layer = m_IgnoreLayer1.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        layerNumber -= 1;

        // Get index of layer from LayerMask2
        int layerNumber2 = 0;
        int layer2 = m_IgnoreLayer2.value;
        while (layer2 > 0)
        {
            layer2 = layer2 >> 1;
            layerNumber2++;
        }
        layerNumber2 -= 1;

        // Disable collision between the two layers
        Physics.IgnoreLayerCollision(layerNumber, layerNumber2, !isEnabled);
    }
}
