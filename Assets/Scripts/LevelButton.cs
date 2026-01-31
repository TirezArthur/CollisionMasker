using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public int _level;
    public Game _game;
    public Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _game.LoadLevel(_level));
        _game.OnLevelComplete += OnLevelComplete;
        if (_level != 0) _button.interactable = false;
    }

    private void OnLevelComplete(int index)
    {
        if (index == _level) _button.Select();
        if (index == (_level - 1)) _button.interactable = true;
    }

    public void Reset()
    {
        if (_level != 0) _button.interactable = false;
    }
}
