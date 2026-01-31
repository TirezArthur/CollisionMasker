using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	enum State
	{
		MenuMain, MenuLevels, Playing
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
    [SerializeReference] Button _quitButton;

    private State _state = State.MenuMain;
	private PlayerMovement _player;
	private int _currentLevel = 0;

	private State GameState
	{
		set
		{
            _gameCanvas.gameObject.SetActive(value == State.Playing);
            _menuCanvas.gameObject.SetActive(value == State.MenuMain);
			_menuCamera.enabled = value != State.Playing;

			_state = value;
		}
		get => _state;
	}

	void Start()
	{
		GameState = State.MenuMain;
		
		_menuButton.onClick.AddListener(() =>
		{
			UnloadLevel();
			GameState = State.MenuMain;
		});
		_restartButton.onClick.AddListener(() => LoadLevel(_currentLevel));

		_playButton.onClick.AddListener(() => LoadLevel(FirstUncompletedLevel()));
        _quitButton.onClick.AddListener(() => Application.Quit());
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

	void LoadLevel(int index)
	{
		Debug.Log("Loading level " + index);

		UnloadLevel();

		if (index >= _levels.Count)
		{
			GameState = State.MenuMain;
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
		_collisionUI.ResetAll();
    }
}