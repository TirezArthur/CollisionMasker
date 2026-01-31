using UnityEngine;

public class Elevator : MonoBehaviour
{
    Vector3 _origPosition;
    private float _startTime;
    
    void Start()
    {
        _origPosition = transform.localPosition;
        _startTime = Time.time;
    }

    void FixedUpdate()
    {
        Vector3 position = _origPosition;
        position.y += 2 * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * 90 * (Time.time-_startTime) / 4));
        transform.localPosition = position;
    }
}
