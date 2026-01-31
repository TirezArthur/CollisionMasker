using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    static public event UnityAction<Pair> KeyPickedUp = delegate { };

    [Serializable]
    public struct Pair
    {
        public LayerMask layer1;
        public LayerMask layer2;
    }
    [SerializeField] private List<Pair> _pairs = new();

    public void Pickup()
    {
        foreach (Pair pair in _pairs) KeyPickedUp.Invoke(pair);
        Destroy(gameObject);
        //TODO play noise
    }
}
