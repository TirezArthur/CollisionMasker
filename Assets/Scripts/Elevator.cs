using UnityEngine;

public class Elevator : MonoBehaviour
{
    Vector3 _origPosition;
    
    void Start()
    {
        _origPosition = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 position = _origPosition;
        position.y += 2 * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * 90 * Time.time / 4));
        transform.localPosition = position;
    }
}
