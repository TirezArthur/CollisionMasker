using UnityEngine;

public class Elevator : MonoBehaviour
{
    Vector3 _origPosition;
    private float _startTime;

    private float _timer = 0f;
    private int _timerMax = 1;

    void Start()
    {
        _origPosition = transform.localPosition;
        _startTime = Time.time;
    }

    void FixedUpdate()
    {
        // If a full sine wave has passed
        if ((Time.time - _startTime) >= 8)
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= _timerMax)
            {
                _startTime = Time.time;
                _timer = 0f;
            }
            else
            {
                return;
            }
        }

        Vector3 position = _origPosition;
        position.y += 2 * Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * 90 * (Time.time-_startTime) / 4));
        transform.localPosition = position;
    }
}
