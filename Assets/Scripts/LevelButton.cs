using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public int _level;
    public Game _game;
    public Button _button;
    private Image _image;
    [SerializeField] private Color _completedColor;
    private Color _originalColor;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _originalColor = _image.color;
        _button.onClick.AddListener(() => _game.LoadLevel(_level));
        _game.OnLevelComplete += OnLevelComplete;
    }

    private void OnLevelComplete(int index)
    {
        if (index == _level) _image.color = _completedColor;
    }

    public void Reset()
    {
        _image.color = _originalColor;
    }
}
