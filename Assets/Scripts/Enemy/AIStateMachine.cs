using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    /********************************************
     * AI State Machine
     * Ticks state and updates the player controller to make the enemy behave itself
    ********************************************/

    #region References
    [SerializeField]
    EnemyController _controller; //reference to the controller we want to update
    #endregion

    public enum AIState { WANDER, WAITING, ALERTED, ATTACKING, STUNNED, DEAD } //Global states used for AI States
    protected float stateTimer; //Timer Used for ticking down states (in seconds)

    #region Triggers
    //State Triggers are delegates set by the controller that cause specific updates whenever the AI state changes (i.e. Flip enemy Direction whenever we get a new passive state)

    public delegate void StateTrigger();
    Dictionary<AIState, StateTrigger> stateTriggers = new Dictionary<AIState, StateTrigger>();

    /// <summary>
    /// Default trigger logic for every state change (i.e. debug)
    /// </summary>
    virtual protected void DefaultStateTrigger() { }
    /// <summary>
    /// Resets and then adds trigger actions to a state change 
    /// </summary>
    /// <param name="state">AIState these triggers pop for</param>
    /// <param name="triggers">New triggers to set for this state</param>
    virtual public void AddStateTriggers(AIState state, params Action[] triggers)
    {
        StateTrigger triggerDelegates = DefaultStateTrigger;
        foreach (var trigger in triggers)
        {
            triggerDelegates += new StateTrigger(trigger);
        }
        stateTriggers.Add(state, triggerDelegates);
    }
    #endregion

    #region State
    /// <summary>
    /// Rolls a new passive state (Wait/Wander) using stats from the controller
    /// </summary>
    /// <returns>New AI State</returns>
    virtual protected AIState RollNewPassiveState()
    {
        int roll = UnityEngine.Random.Range(0, 2);
        switch (roll)
        {
            case 0:
                int minWander = _controller.GetMinWanderTime();
                int maxWander = _controller.GetMaxWanderTime();
                stateTimer = UnityEngine.Random.Range(minWander, maxWander);
                return AIState.WANDER;
            case 1:
                int minWait = _controller.GetMinWaitTime();
                int maxWait = _controller.GetMaxWaitTime();
                stateTimer = UnityEngine.Random.Range(minWait, maxWait);
                return AIState.WAITING;
        }

        return AIState.WAITING;
    }

    /// <summary>
    /// Updates the state on the controller and then calls any triggers that have been registered
    /// </summary>
    /// <param name="newState">State we are changing to</param>
    virtual protected void UpdateStateWithTriggers(AIState newState)
    {
        _controller.ChangeAIState(newState);
        if(stateTriggers.ContainsKey(newState))
        {
            stateTriggers[newState].Invoke();
        }
    }

    virtual public void HitReaction()
    {
        _controller.FacePlayer();
        UpdateStateWithTriggers(AIState.ALERTED);
    }
    
    #endregion

    #region Unity
    void Update()
    {

        if(_controller.GetCurrentAIState() == AIState.DEAD)
        {
            return;
        }

        if (_controller.CanSeePlayer()) //I can see the player
        {
            switch (_controller.GetCurrentAIState())
            {
                case AIState.WANDER:
                    UpdateStateWithTriggers(AIState.ALERTED);
                    break;
                case AIState.WAITING:
                    UpdateStateWithTriggers(AIState.ALERTED);
                    break;
                case AIState.ALERTED:
                    if (_controller.CanAttackPlayer())
                    {
                        _controller.Attack();
                        stateTimer = _controller.GetAttackCooldown();
                    }
                    else
                    {
                        _controller.MoveToAttackPlayer();
                    }
                    break;
                case AIState.STUNNED:
                    if (stateTimer <= 0)
                    {
                        UpdateStateWithTriggers(AIState.ALERTED);
                    }
                    return;
                case AIState.ATTACKING:
                    if (!_controller.CanAttackPlayer() && stateTimer <= 0)
                    {
                        _controller.ChangeAIState(AIState.ALERTED);
                    }
                    break;
            }
        }
        else // I can NOT see the player
        {
            switch (_controller.GetCurrentAIState())
            { 
                case AIState.WANDER:
                    if (stateTimer <= 0)
                    {
                        UpdateStateWithTriggers(RollNewPassiveState());
                    }
                    else
                    {
                        _controller.Wander();  
                    }
                    break;
                case AIState.WAITING:
                    if (stateTimer <= 0)
                    {
                        UpdateStateWithTriggers(RollNewPassiveState());
                    }
                    else
                    {
                        _controller.Wait();
                    }
                    break;
                case AIState.ALERTED: //I have lost aggro on the player (I can't see them)
                    UpdateStateWithTriggers(RollNewPassiveState());
                    break;
                case AIState.STUNNED: 
                    if (stateTimer <= 0) //I am no longer stunned
                    {
                        UpdateStateWithTriggers(RollNewPassiveState());
                    }
                    else
                    {
                        return;
                    }
                    break;
                case AIState.ATTACKING: //I have finished my attack but can no longer see the player
                    if (stateTimer <= 0)
                    {
                        UpdateStateWithTriggers(RollNewPassiveState());
                    }
                    break;
            }
        }
        stateTimer -= Time.deltaTime;
    }
    #endregion
}
