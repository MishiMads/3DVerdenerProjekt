using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    // Start is called before the first frame update
    protected BaseState<EState> CurrentState;
    protected bool IsTransitionState = false;

    void Start()
    {
        CurrentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (!IsTransitionState && Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState(nextStateKey);
        }
        CurrentState.UpdateState();
    }
    
    public void TransitionToState(EState StateKey)
    {
        IsTransitionState = true;
        CurrentState.ExitState();
        CurrentState = States[StateKey];
        CurrentState.EnterState();
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OntriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OntriggerStay(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        CurrentState.OntriggerExit(other);
    }
}
