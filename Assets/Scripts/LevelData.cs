using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private bool _completed;
    [SerializeField] private GameObject _prefab;

    public GameObject Prefab => _prefab;
    public bool Completed 
    { 
        get => _completed; 
        set => _completed = value;
    }
}
