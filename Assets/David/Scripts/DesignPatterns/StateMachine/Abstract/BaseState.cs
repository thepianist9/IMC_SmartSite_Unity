using System;
using System.Collections;
using UnityEngine;

namespace Assets.David.Scripts.DesignPatterns
{
    public abstract class BaseState<Estate> where Estate : Enum
    {

        public BaseState(Estate stateKey)
        {
            this.stateKey = stateKey;
        }
       
        public Estate stateKey { get; private set; }

        public abstract void InitState(); // This is a new method that is not in the original code
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract Estate GetNextState();

    }
}