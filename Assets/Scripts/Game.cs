using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityUtils;

public class Game : MonoBehaviour
{
	enum State
	{
		Main, Levels, Credits, Playing
	}
	
	[SerializeReference] GameObject _playerPrefab;
	[SerializeReference] GameObject _levelRoot;
	[SerializeReference] List<LevelData> _levels;

	[Header("Game UI")]
	[SerializeReference] Canvas _gameCanvas;
	[SerializeReference] CinemachineCamera _menuCamera;
	[SerializeReference] CollisionUIBehaviour _collisionUI;
	[SerializeReference] Button _menuButton;
	[SerializeReference] Button _restartButton;

    [Header("Menu UI")]
    [SerializeReference] Canvas _menuCanvas;
    [SerializeReference] Button _playButton;
    [SerializeReference] Button _levelsButton;
    [SerializeReference] Button _creditsButton;
    [SerializeReference] Button _quitButton;

	[Header("Level UI")]
	[SerializeReference] Canvas _levelCanvas;
	[SerializeReference] Button _backButton;
    [SerializeReference] Button _clearButton;
	[SerializeReference] GameObject _levelButtonPrefab;
	[SerializeReference] RectTransform _levelsList;

	[Header("Credits UI")]
	[SerializeReference] Canvas _creditsCanvas;
	[SerializeReference] Button _backButtonCred;

    private State _state = State.Main;
	private PlayerMovement _player;
	private int _currentLevel = 0;

	public event UnityAction<int> OnLevelComplete = delegate { };

	private State GameState
	{
		set
		{
            _gameCanvas.gameObject.SetActive(value == State.Playing);
            _menuCanvas.gameObject.SetActive(value == State.Main);
			_levelCanvas.gameObject.SetActive(value == State.Levels);
            _creditsCanvas.gameObject.SetActive(value == State.Credits);
            _menuCamera.enabled = value != State.Playing;

			_state = value;
		}
		get => _state;
	}

	void Start()
	{
		GameState = State.Main;
		
		_menuButton.onClick.AddListener(() =>
		{
			UnloadLevel();
			GameState = State.Main;
		});
		_restartButton.onClick.AddListener(() => LoadLevel(_currentLevel));

		_playButton.onClick.AddListener(() => LoadLevel(FirstUncompletedLevel()));
		_levelsButton.onClick.AddListener(() => GameState = State.Levels);
		_creditsButton.onClick.AddListener(() => GameState = State.Credits);
        _quitButton.onClick.AddListener(() => Application.Quit());

		_backButton.onClick.AddListener(() => GameState = State.Main);
		_clearButton.onClick.AddListener(() =>
		{
			foreach (LevelData level in _levels) level.Completed = false;
			foreach (Transform levelButton in _levelsList.Children()) levelButton.GetComponent<LevelButton>().Reset();
		});

		_backButtonCred.onClick.AddListener(() => GameState = State.Main);

		if (!_levelsList || !_levelButtonPrefab) return;
		for (int levelIndex = 0; levelIndex < _levels.Count; ++levelIndex)
		{
			GameObject levelButton = GameObject.Instantiate(_levelButtonPrefab, _levelsList);
			levelButton.GetComponentInChildren<TextMeshProUGUI>().SetText((levelIndex + 1).ToString());
			levelButton.GetComponent<LevelButton>()._level = levelIndex;
            levelButton.GetComponent<LevelButton>()._game = this;
        }
    }

	void Update()
	{
		if (GameState == State.Playing)
		{
			// check for player death or win
			if (_player.Died)
			{
				LoadLevel(_currentLevel);
			}
			else if (_player.ReachedEnd)
			{
				_levels[_currentLevel].Completed = true;
                OnLevelComplete.Invoke(_currentLevel);
                LoadLevel(_currentLevel + 1);
			}
		}
	}

	private int FirstUncompletedLevel()
	{
		foreach (LevelData level in _levels) 
		{
			if (!level.Completed) return _levels.IndexOf(level);
		}
		return _levels.Count - 1;
	}

	public void LoadLevel(int index)
	{
		Debug.Log("Loading level " + index);

		UnloadLevel();

		if (index >= _levels.Count)
		{
			GameState = State.Main;
			return;
		}
		
		_currentLevel = index;
			
		// load level prefab
		GameObject level = _levels[index].Prefab;
		Instantiate(level, Vector3.zero, Quaternion.identity, _levelRoot.transform);

		_player = Instantiate(_playerPrefab).GetComponent<PlayerMovement>();
		_player.transform.position = Vector3.zero;

		_collisionUI.UnlockByLayer(_levels[index].LockedLayers, false);
		foreach (LevelData.ToggleRule rule in _levels[index].ToggleConstraints)
		{
			_collisionUI.UnlockByPair(rule.layer1, rule.layer2, false);
		}

		GameState = State.Playing;
	}

	private void UnloadLevel()
	{
        // destroy player
        if (_player != null)
            Destroy(_player.gameObject);
        // destroy previous level
        if (_levelRoot.transform.childCount > 0)
            Destroy(_levelRoot.transform.GetChild(0).gameObject);
		foreach (Bullet bullet in FindObjectsByType<Bullet>(FindObjectsSortMode.None)) Destroy(bullet.gameObject);
		_collisionUI.ResetAll();
    }
}