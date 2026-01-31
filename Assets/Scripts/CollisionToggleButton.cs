using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    private List<GameObject> m_VisualObjects1 = new List<GameObject>();
    private List<GameObject> m_VisualObjects2 = new List<GameObject>();
    [SerializeField] private float m_FlickerTimer = 0f;
    [SerializeField] private float m_FlickerCooldown = 0.2f;
    [SerializeField] private int m_MaxFlickers = 3;

    private int m_FlickerCount = 0;
    bool m_IsFlickering = false;

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

        m_VisualObjects1 = FindObjectsByLayer(layerNumber);
        m_VisualObjects2 = FindObjectsByLayer(layerNumber2);

        if (m_VisualObjects1.Count > 0 || m_VisualObjects2.Count > 0)
        {
            m_IsFlickering = true;
        }
    }

    private void Update()
    {
        if (m_IsFlickering)
        {
            m_FlickerTimer += Time.deltaTime;

            if (m_FlickerTimer >= m_FlickerCooldown)
            {
                m_FlickerCount++;
                m_FlickerTimer = 0f;

                // Flicker the visuals
                foreach (GameObject go in m_VisualObjects1)
                {
                    Renderer renderer = go.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        renderer.enabled = !renderer.enabled;
                    }
                }

                foreach (GameObject go in m_VisualObjects2)
                {
                    Renderer renderer = go.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        renderer.enabled = !renderer.enabled;
                    }
                }

                // Stop flickering after max flickers
                if (m_FlickerCount >= m_MaxFlickers)
                {
                    m_IsFlickering = false;
                    m_FlickerCount = 0;

                    foreach (GameObject go in m_VisualObjects1)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            renderer.enabled = true;
                        }
                    }

                    foreach (GameObject go in m_VisualObjects2)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();

                        if (renderer != null)
                        {
                            renderer.enabled = true;
                        }
                    }
                }
            }
        }
    }

    private List<GameObject> FindObjectsByLayer(int layer)
    {
        GameObject[] all = FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None) as GameObject[];
        List<GameObject> results = new List<GameObject>();

        foreach (GameObject go in all)
        {
            if (go.layer == layer)
            {
                if (go.GetComponent<Renderer>() != null)
                {
                    results.Add(go);
                }
            }
        }

        return results;
    }
}
