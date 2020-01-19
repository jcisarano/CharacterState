using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace jcisarano.CharacterStateMachine
{
    public class CharacterStateMachine : MonoBehaviour
    {
        public int _currentState = CharacterState.PRE_INIT;
        int CurrentState
        {
            get { return _currentState; }
            set
            {
                if (Debug)
                {
                    _stateHistory.Add(CharacterState.StateValToName(value));
                }

                _currentState = value;
            }
        }
        Dictionary<int, List<StateDelegate>> _delegates = new Dictionary<int, List<StateDelegate>>();

        public delegate void StateDelegate();

        public bool Debug { get; set; }
        public List<string> _stateHistory = new List<string>();

        public void SetDelegateForState(int state, StateDelegate @delegate)
        {
            if(!_delegates.ContainsKey(state))
            {
                _delegates[state] = new List<StateDelegate>();
            }

            _delegates[state].Add( @delegate );
        }

        void Update()
        {
            if (_delegates.ContainsKey(CurrentState))
            {
                foreach(StateDelegate @delegate in _delegates[CurrentState])
                {
                    @delegate?.Invoke();
                }
            }
        }

        public void SetState(int state)
        {
            CurrentState = state;
        }

        public void UnsetStateDelegate(int state, StateDelegate @delegate)
        {
            if(_delegates.ContainsKey(state))
            {
                _delegates[state].Remove(@delegate);
            }
        }

        public virtual void SetNextState()
        {
            switch (CurrentState)
            {
                case CharacterState.PRE_INIT:
                    CurrentState = CharacterState.SEARCHING_FOR_TARGET;
                    break;

                case CharacterState.SEARCHING_FOR_TARGET:
                    CurrentState = CharacterState.INIT_ATTACK;
                    break;
                case CharacterState.INIT_ATTACK:
                    CurrentState = CharacterState.ATTACKING;
                    break;
                case CharacterState.ATTACKING:
                    CurrentState = CharacterState.ATTACK_COMPLETE;
                    break;
                case CharacterState.ATTACK_COMPLETE:
                    CurrentState = CharacterState.SEARCHING_FOR_TARGET;
                    break;

                case CharacterState.INIT_MOVE:
                    CurrentState = CharacterState.MOVE_TO;
                    break;
                case CharacterState.MOVE_TO:
                    CurrentState = CharacterState.MOVE_COMPLETE;
                    break;
                case CharacterState.MOVE_COMPLETE:
                    CurrentState = CharacterState.INIT_MOVE;
                    break;
            }
        }
    }

    public class CharacterState
    {
        public const int PRE_INIT = 1;

        public const int SEARCHING_FOR_TARGET = 2;
        public const int INIT_ATTACK = 3;
        public const int ATTACKING = 4;
        public const int ATTACK_COMPLETE = 5;

        public const int INIT_MOVE = 6;
        public const int MOVE_TO = 7;
        public const int MOVE_COMPLETE = 8;

        public static string StateValToName(int state)
        {
            System.Reflection.FieldInfo[] props = typeof(CharacterState).GetFields();
            System.Reflection.FieldInfo wantedProp = props.FirstOrDefault(prop => (int)prop.GetValue(null) == state);
            return wantedProp.Name;
        }
    }
}