using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /********************************************
     * Enemy Controller 
     * Modifies the enemy visuals and stats based on external actions
    ********************************************/

    #region References
    [SerializeField]
    AIStateMachine _AIStateMachine;
    [SerializeField]
    Animator _anim;
    [SerializeField]
    BoxCollider2D _coll;
    [SerializeField]
    Rigidbody2D _rigidbody;
    [SerializeField]
    PlayerController _player;
    [SerializeField]
    AttackColliderEventHandler rightAttackCollider;
    [SerializeField]
    AttackColliderEventHandler leftAttackCollider;

    #endregion

    #region Stats
    [SerializeField]
    protected int health;
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float detectionDistance;
    [SerializeField]
    protected float hitStunTime;
    [SerializeField]
    protected float attackCooldown;
    [SerializeField]
    protected float visionTolerance;
    [SerializeField]
    protected int minWaitTime;
    [SerializeField]
    protected int maxWaitTime;
    [SerializeField]
    protected int minWanderTime;
    [SerializeField]
    protected int maxWanderTime;

    protected float attackRange;
    #endregion

    #region State
    protected bool alive;
    protected AIStateMachine.AIState currentAIState;
    AIStateMachine.AIState defaultAIState;
    protected bool attacking;
    protected int currentDirection;
    protected Vector2 velocity;
    #endregion

    /// <summary>
    /// Initialize the enemy starting state
    /// </summary>
    public void Init()
    {
        _player = GameStateManager.GetPlayerController();
        _AIStateMachine.AddStateTriggers(AIStateMachine.AIState.WAITING, RollFlipDirection); //Each time we start waiting roll to flip direction
        _AIStateMachine.AddStateTriggers(AIStateMachine.AIState.WANDER, RollFlipDirection); //Each time we start waiting roll to flip direction
        _AIStateMachine.AddStateTriggers(AIStateMachine.AIState.ALERTED, Alert);
        currentAIState = defaultAIState;
        currentDirection = 1;
        attackRange = rightAttackCollider.GetWidth();
        alive = true;
    }

    #region AI
    /// <summary>
    /// Changes current AI state of the enemy
    /// </summary>
    /// <param name="state">State to change to</param>
    virtual public void ChangeAIState(AIStateMachine.AIState state)
    {
        //Debug.Log("Changing AI State: " + currentAIState + " to " + state);
        currentAIState = state;
    }

    /// <summary>
    /// ded
    /// </summary>
    protected void Die()
    {
        alive = false;
        ChangeAIState(AIStateMachine.AIState.DEAD);
        _anim.SetBool("Moving", false);
        _anim.SetBool("Alert", false);
        _anim.SetBool("Attacking", false);
        _anim.SetBool("Dead", true);
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 0;
        _coll.isTrigger = true;
    }

    /// <summary>
    /// Applies damage from a player source
    /// </summary>
    /// <param name="source">PlayerController source</param>
    public void TakeDamage(PlayerController source)
    {
        int incomingDamage = source.GetDamage();

        if (incomingDamage >= health)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= incomingDamage;
        }
    }

    /// <summary>
    /// Move towards the player until they are within attack range (Don't just rub against them)
    /// </summary>
    virtual public void MoveToAttackPlayer()
    {
        if (Vector2.Distance(_player.transform.position, transform.position) >= attackRange) //is the player outside my attack range
        {

            Vector2 movementDirection = (transform.position.x - _player.transform.position.x) > 0 ? Vector2.left : Vector2.right;
            currentDirection = (int)movementDirection.x;
            _rigidbody.velocity = movementDirection * speed;
            _anim.SetBool("Moving", true);
            _anim.SetBool("Attacking", false);
            _anim.SetInteger("Direction", currentDirection);
        }
        else //They are close enough but I can't hit them yet
        {
            Wait();
        }
    }

    /// <summary>
    /// Move in the direction you are facing
    /// </summary>
    virtual public void Wander()
    {
        Vector2 movementDirection = currentDirection > 0 ? Vector2.right : Vector2.left;
        _rigidbody.velocity = movementDirection * speed;

        _anim.SetBool("Moving", true);
        _anim.SetBool("Attacking", false);
        _anim.SetInteger("Direction", currentDirection);
    }

    /// <summary>
    /// Idle around a bit
    /// </summary>
    virtual public void Wait()
    {
        Vector2 movementDirection = currentDirection > 0 ? Vector2.right : Vector2.left;
        _rigidbody.velocity = Vector2.zero;

        _anim.SetBool("Moving", false);
        _anim.SetBool("Attacking", false);
        _anim.SetInteger("Direction", currentDirection);
    }

    /// <summary>
    /// Attempt to hit the player
    /// </summary>
    virtual public void Attack()
    {
        ChangeAIState(AIStateMachine.AIState.ATTACKING);
        _anim.SetBool("Attacking", true);
        _anim.SetBool("Moving", false);
        _anim.SetInteger("Direction", currentDirection);
    }

    /// <summary>
    /// Can I attack the player
    /// </summary>
    /// <returns>true if the player can be attacked</returns>
    virtual public bool CanAttackPlayer()
    {
        if (currentAIState != AIStateMachine.AIState.ATTACKING) //Am I already attacking?
        {
            if (Vector2.Distance(_player.transform.position, transform.position) <= attackRange) //Are they within my attack range?
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Can I see the player
    /// </summary>
    /// <returns>true if the player can be seen</returns>
    virtual public bool CanSeePlayer()
    {
        bool spotted = false;
        if (Vector2.Distance(_player.transform.position, transform.position) <= detectionDistance) //are they close enough?
        {
            float visionAngleToPlayer = Vector2.Dot(_player.transform.position.normalized, transform.position.normalized); //Are they at an angle that makes sense? (not directly above me, or directly behind me)
            if (visionAngleToPlayer >= visionTolerance)
            {

                spotted = true;
            }
        }
        //Debug.Log("Player Spotted: " + spotted);
        return spotted;
    }
    #endregion

    #region Unity
    //TODO: Implement player-enemy collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    #endregion

    #region StateTriggers
    /// <summary>
    /// Roll 50/50 to face the opposite direction I currently am
    /// </summary>
    public void RollFlipDirection()
    {
        if (Random.Range(0, 2) == 1) //Flip a coin to flip me!
        {
            //Flip direction and collider
            currentDirection *= -1;
            _coll.offset = new Vector2(_coll.offset.x * -1, _coll.offset.y);

            //Update Animator
            _anim.SetInteger("Direction", currentDirection);
        }
    }

    /// <summary>
    /// Play the Alerted Animation
    /// </summary>
    public void Alert()
    {
        _anim.SetBool("Alert", true);

    }
    #endregion

    #region EventHandlers
    /// <summary>
    /// Animations are telling me to deal damage
    /// </summary>
    public void HandleDamageFrame()
    {
        if(currentDirection > 0)
        {
            rightAttackCollider.CheckCollisions();
        }
        else
        {
            leftAttackCollider.CheckCollisions();
        }
    }

    /// <summary>
    /// Sets the Alert animator field to false
    /// </summary>
    public void ResetAlert()
    {
        _anim.SetBool("Alert", false);
    }

    public void HandleDeadFrame()
    {
        _anim.speed = 0;
    }
    #endregion

    #region Getters
    virtual public AIStateMachine.AIState GetCurrentAIState()
    {
        return currentAIState;
    }

    virtual public int GetMinWaitTime()
    {
        return minWaitTime;
    }

    virtual public int GetMaxWaitTime()
    {
        return maxWaitTime;
    }

    virtual public int GetMinWanderTime()
    {
        return minWanderTime;
    }

    virtual public int GetMaxWanderTime()
    {
        return maxWanderTime;
    }

    virtual public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    virtual public int GetDamage()
    {
        return damage;
    }

    virtual public bool IsAlive()
    {
        return alive;
    }
    #endregion


}
