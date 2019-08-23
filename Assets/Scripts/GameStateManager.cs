using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance;
    public enum GameState { MENU, GAME, GAME_OVER}
    GameState _gameState;

    #region References
    [SerializeField]
    InputHandler _inputHandler;
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    GameObject UIPrefab;
    

    UIController _uiContoller;
    PlayerController _player;
    List<EnemyController> enemies = new List<EnemyController>();
    #endregion

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _gameState = GameState.MENU;
        _uiContoller = Instantiate<GameObject>(UIPrefab).GetComponent<UIController>();
        _uiContoller.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region State
    public void StartGame()
    {
        _gameState = GameState.GAME;
        _uiContoller.StartGame();
        SpawnPlayer();
        SpawnEnemy(new Vector3(5, -3, 0));
    }

    public void GameOver()
    {
        _gameState = GameState.GAME_OVER;
        _uiContoller.GameOver();
    }

    public void ProcessTryAgainInput(bool input)
    {
        if (_gameState == GameState.GAME_OVER && input)
        {
            _player.Despawn();
            foreach(var e in enemies)
            {
                e.Despawn();
            }
            StartGame();
        }
        
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Spawn the player
    /// </summary>
    void SpawnPlayer()
    {
        _player = Instantiate<GameObject>(playerPrefab).GetComponent<PlayerController>();
        _inputHandler.Init(_player);
        _player.Init();
    }

    /// <summary>
    /// Spawns an enemy
    /// </summary>
    /// <param name="pos">position to spawn the enemy</param>
    void SpawnEnemy(Vector3 pos)
    {
        EnemyController e = Instantiate<GameObject>(enemyPrefab, pos, enemyPrefab.transform.rotation).GetComponent<EnemyController>();
        enemies.Add(e);
        e.Init();
    }

    /// <summary>
    /// Deal damage from an enemy to a player
    /// </summary>
    /// <param name="target">target PlayerController</param>
    /// <param name="source">source EnemyController</param>
    public void DealDamage(PlayerController target, EnemyController source)
    {
        target.TakeDamage(source);
    }

    /// <summary>
    /// Deal damage from the player to an enemy
    /// </summary>
    /// <param name="target">target EnemyController</param>
    /// <param name="source">source PlayerController</param>
    public void DealDamage(EnemyController target, PlayerController source)
    {
        target.TakeDamage(source);
    }
    #endregion

    #region Getters
    public PlayerController GetPlayerController()
    {
        return _player;
    }

    public  GameState GetGameState()
    {
        return _gameState;
    }
    #endregion
}
