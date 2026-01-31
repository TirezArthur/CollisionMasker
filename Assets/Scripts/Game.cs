using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	enum State
	{
		Menu, Playing
	}
	
	[SerializeReference] GameObject _playerPrefab;
	[SerializeReference] GameObject _levelRoot;
	[SerializeReference] List<GameObject> _levels;
	[SerializeReference] Camera _menuCamera;
	[SerializeReference] Camera _playerCamera;
	[SerializeReference] GameObject _collisionUI;
	[SerializeReference] Button _menuButton;
	[SerializeReference] Button _restartButton;

	private State _state = State.Menu;
	private PlayerMovement _player;
	private int _currentLevel = 0;

	void Start()
	{
		// for now
		LoadLevel(0);
		
		_menuButton.onClick.AddListener(() => LoadLevel(_currentLevel));
		_restartButton.onClick.AddListener(() => LoadLevel(_currentLevel));
	}

	void Update()
	{
		if (_state == State.Playing)
		{
			// check for player death or win
			if (_player.Died)
			{
				LoadLevel(_currentLevel);
			}
			else if (_player.ReachedEnd)
			{
				LoadLevel(_currentLevel + 1);
			}
		}
	}

	void LoadLevel(int index)
	{
		Debug.Log("Loading level " + index);
		if (index >= _levels.Count)
		{
			// TODO back to menu, reloading level 1 for now
			index = 0;
		}
		
		// destroy player
		if (_player != null)
			Destroy(_player.gameObject);
		// destroy previous level
		if (_levelRoot.transform.childCount > 0)
			Destroy(_levelRoot.transform.GetChild(0).gameObject);
		
		_currentLevel = index;
			
		// load level prefab
		GameObject level = _levels[index];
		Instantiate(level, Vector3.zero, Quaternion.identity, _levelRoot.transform);

		_player = Instantiate(_playerPrefab).GetComponent<PlayerMovement>();
		_player.transform.position = Vector3.zero;
		// _player.Reset();

		_menuCamera.enabled = false;
		_playerCamera.enabled = true;
		_collisionUI.SetActive(true);
		_state = State.Playing;
	}
}