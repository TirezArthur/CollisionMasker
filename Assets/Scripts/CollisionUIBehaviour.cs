using System.Collections.Generic;
using UnityEngine;

public class CollisionUIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_CanvasObject;
    private Canvas m_Canvas;

    [SerializeField] private RectTransform m_VerticalLayoutGroup;
    [SerializeField] private RectTransform m_HorizontalLayoutGroup;
    [SerializeField] private RectTransform m_GridGroup;

    [SerializeField] private GameObject m_TogglePrefab;
    [SerializeField] private GameObject m_LabelPrefab;

    [SerializeField] private LayerMask m_LockedLayers;

    private List<CollisionToggleButton> m_ToggleButtons = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure a VerticalLayoutGroup is assigned
        if (m_VerticalLayoutGroup == null)
        {
            Debug.LogWarning("No Vertical Layout Group assigned");
            return;
        }

        // Make sure a HorizontalLayoutGroup is assigned
        if (m_HorizontalLayoutGroup == null)
        {
            Debug.LogWarning("No Horizontal Layout Group assigned");
            return;
        }

        // Make sure a GridGroup is assigned
        if (m_GridGroup == null)
        {
            Debug.LogWarning("No Grid Group assigned");
            return;
        }

        // Make sure there is a Toggle prefab assigned
        if (m_TogglePrefab == null)
        {
            Debug.LogWarning("No Toggle Prefab assigned");
            return;
        }

        // Make sure there is a Label prefab assigned
        if (m_LabelPrefab == null)
        {
            Debug.LogWarning("No Label Prefab assigned");
            return;
        }

        InitialiseCanvas();

        InitialiseCollisionGrid();
    }

    private void InitialiseCanvas()
    {
        // Make sure there is a Canvas game object assigned
        if (m_CanvasObject == null)
        {
            Debug.LogWarning("No Canvas object assigned");
            return;
        }

        // Get or add a Canvas component
        m_Canvas = m_CanvasObject.GetComponent<Canvas>();

        if (m_Canvas == null)
        {
            m_Canvas = m_CanvasObject.AddComponent<Canvas>();
        }

        // Set Canvas to use the main camera
        m_Canvas.worldCamera = Camera.main;
    }

    private void InitialiseCollisionGrid()
    {
        // Get amount of Layers defined in the project
        int maxLayers = 32;
        int layerCount = 0;

        int startIdx = 10;

        // Go over all possible layers starting at a certain idx
        for (int leftIdx = startIdx; leftIdx < maxLayers; leftIdx++)
        {
            string defaultLayerName = LayerMask.LayerToName(leftIdx);

            if (string.IsNullOrEmpty(defaultLayerName))
            {
                continue;
            }

            m_GridGroup.GetComponent<RectTransform>().sizeDelta += new Vector2(0, m_TogglePrefab.GetComponent<RectTransform>().sizeDelta.y);

            for (int topIdx = maxLayers - 1; topIdx >= leftIdx; topIdx--)
            {
                string layerName = LayerMask.LayerToName(topIdx);

                // If the layer is implemented
                if (!string.IsNullOrEmpty(layerName))
                {
                    // Create a toggle button for this layer
                    GameObject toggleButton = Instantiate(m_TogglePrefab, m_GridGroup);

                    // TODO: Implement initial grid state if needed

                    // Set layers
                    CollisionToggleButton collisionToggle = toggleButton.GetComponent<CollisionToggleButton>();
                    m_ToggleButtons.Add(collisionToggle);
                    collisionToggle.m_IgnoreLayer1 = LayerMask.GetMask(layerName);
                    collisionToggle.m_IgnoreLayer2 = LayerMask.GetMask(defaultLayerName);

                    if ((m_LockedLayers.value & (1 << leftIdx)) != 0 ||
                        (m_LockedLayers.value & (1 << topIdx)) != 0)
                    {
                        collisionToggle.SetToggleUnlocked(false);
                    }

                    // Adjust grid size to fit new button
                    if (layerCount < 1)
                    {
                        // Create labels for this layer
                        GameObject sideLabel = Instantiate(m_LabelPrefab, m_VerticalLayoutGroup);
                        sideLabel.GetComponent<TMPro.TextMeshProUGUI>().text = layerName;
                        m_VerticalLayoutGroup.sizeDelta += new Vector2(0, sideLabel.GetComponent<RectTransform>().sizeDelta.y);

                        GameObject topLabel = Instantiate(m_LabelPrefab, m_HorizontalLayoutGroup);
                        topLabel.GetComponent<TMPro.TextMeshProUGUI>().text = layerName;
                        m_HorizontalLayoutGroup.sizeDelta += new Vector2(0, sideLabel.GetComponent<RectTransform>().sizeDelta.y);

                        m_GridGroup.GetComponent<RectTransform>().sizeDelta += new Vector2(
                            toggleButton.GetComponent<RectTransform>().sizeDelta.x,
                            0
                        );
                    }
                }
            }

            // For each covered layer, add an empty cell
            for (int idx = 0; idx < layerCount; idx++)
            {
                GameObject emptyCell = new GameObject("EmptyCell", typeof(RectTransform));
                emptyCell.transform.SetParent(m_GridGroup);
            }

            layerCount++;
        }
    }

    public void UnlockByLayer(LayerMask layerMask)
    {
        // Put all locked layers that are not in the layer mask in a variable
        LayerMask layerMaskDiff = m_LockedLayers & (~layerMask);

        foreach (CollisionToggleButton toggle in m_ToggleButtons)
        {
            int layer1 = toggle.m_IgnoreLayer1.value;
            int layer2 = toggle.m_IgnoreLayer2.value;
            if (((layerMask.value & layer1) != 0) || ((layerMask.value & layer2) != 0))
            {
                if ((layerMaskDiff.value & layer1) != 0 || (layerMaskDiff.value & layer2) != 0)
                {
                    continue;
                }
                
                toggle.SetToggleUnlocked(true);
            }
        }
    }

    public void ResetAll()
    {
        foreach(CollisionToggleButton toggle in m_ToggleButtons)
        {
            toggle.ToggleCollision(true, false);
        }
    }

    private void ToggleCanvas(bool isEnabled)
    {
        m_CanvasObject.SetActive(isEnabled);
    }
}
