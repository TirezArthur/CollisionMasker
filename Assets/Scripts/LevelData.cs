using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [Serializable]
    public struct ToggleRule {
        public LayerMask layer1;
        public LayerMask layer2;
    }

    [SerializeField] private bool _completed;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private LayerMask _lockedLayers;
    [SerializeField] private List<ToggleRule> _toggleConstraints = new();

    public GameObject Prefab => _prefab;
    public bool Completed 
    { 
        get => _completed; 
        set => _completed = value;
    }
    public List<ToggleRule> ToggleConstraints => _toggleConstraints;
    public LayerMask LockedLayers => _lockedLayers;
}
