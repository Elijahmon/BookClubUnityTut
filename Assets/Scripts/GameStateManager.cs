using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public static GameStateManager instance;

    #region References
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject enemyPrefab;

    PlayerController _player;
    List<EnemyController> enemies = new List<EnemyController>();
    #endregion

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SpawnPlayer();
        SpawnEnemy(new Vector3(5, -3, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    /// <summary>
    /// Spawn the player
    /// </summary>
    void SpawnPlayer()
    {
        _player = Instantiate<GameObject>(playerPrefab).GetComponent<PlayerController>();
        _player.Init();
    }

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

    #region Getters
    public static PlayerController GetPlayerController()
    {
        return instance._player;
    }
    #endregion
}
