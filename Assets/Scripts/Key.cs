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

    [SerializeField] private AudioClip _pickupSound;

    public void Update()
    {
        transform.Rotate(0f, Time.deltaTime * 40f, 0f);
    }

    public void Pickup()
    {
        foreach (Pair pair in _pairs) KeyPickedUp.Invoke(pair);

        // Play pickup sound
        GetComponent<AudioSource>().PlayOneShot(_pickupSound);

        // Disable collider and visual
        transform.Find("Visual").GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Destroy after 1 second to allow sound to play
        Destroy(gameObject, 1);
    }
}
