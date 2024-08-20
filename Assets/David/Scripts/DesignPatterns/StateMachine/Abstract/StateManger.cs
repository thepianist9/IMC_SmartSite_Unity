using System.Collections.Generic;
using UnityEngine;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : System.Enum
    {

        protected Dictionary<EState, BaseState<EState>> states;
        protected BaseState<EState> currentState;

        private bool isTransitioningState = false;
        private void Awake()
        {
            states = new Dictionary<EState, BaseState<EState>>();
        }
        private void Update()
        {
            EState nextKey = currentState.GetNextState();

            if (!isTransitioningState && nextKey.Equals(currentState.stateKey)) 
            {
                currentState.UpdateState();
            }
            else
            {
                TransitionToState(nextKey); 
            }
             
        }
        protected void TransitionToState(EState nextStateKey, bool useTransition = false)
        {
            isTransitioningState = true;
            currentState.ExitState();
            currentState = states[nextStateKey];
            currentState.EnterState();
            isTransitioningState = false;
        }

    }
}