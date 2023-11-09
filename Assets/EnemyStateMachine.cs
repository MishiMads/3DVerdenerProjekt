using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateManager<EnemyStateMachine.EnemyState>
{
  public enum EnemyState
  {
    Idle,
    Stalking,
    Attacking,
  }

  private void Awake()
  {
    CurrentState = States[EnemyState.Idle];
  }
  
  
  
}
