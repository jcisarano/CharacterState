# CharacterState

A simple, real-time state machine intended for use on a game character or similar in-game entity. It comes set up with a few standard states common to a lot of characters, but it is easily extensible to cover many most cases. It uses a system of Unity delegates to allow you to register your state management functions, and it calls the delegate for the current stgate on each tick.

## Usage
The state machine consists of one file with two classes: `CharacterStateMachine` and `CharacterState`. You should attach a `CharacterStateMachine` component to your character game object and initialize the state machine in your character class. The main thing you should do is register your delegates with the state machine. For instance, here is the initialization from the `Mover` class in our example:

```
   protected virtual void InitStateMachine()
    {
        _stateMachine = gameObject.AddComponent<CharacterStateMachine>();
        _stateMachine.SetDelegateForState(CharacterState.PRE_INIT, Init);
        _stateMachine.SetDelegateForState(CharacterState.INIT_MOVE, InitMove);
        _stateMachine.SetDelegateForState(CharacterState.MOVE_TO, MoveTo);
        _stateMachine.SetDelegateForState(CharacterState.MOVE_COMPLETE, OnMoveComplete);
    }
```

As you can see, this sets the `Init()` function to be called during `CharacterState.PRE_INIT`, `InitMove()` during `CharacterState.INIT_MOVE`, and so on. The functions called should have a way of moving to the next state if necessary. For instance, `Mover` needs to have a way to switch from `CharacterState.MOVE_TO` to `CharacterState.MOVE_COMPLETE` when it reaches its destination:

```
    protected virtual void MoveTo()
    {
        if (ArrivedAtDestination())
        {

            _stateMachine.SetNextState();
            return;
        }

        ...
        update position and stuff here
        ...
    }
```

There are a couple of ways to determine which state to set next. If your state machine uses a standard state organization, `CharacterState.SetNextState()` can handle the transition as above. Or you can set a specific state: 

```
    _stateMachine.SetState(CharacterState.SEARCHING_FOR_TARGET);
```

## Extending the state machine
The main thing you'll probably change is the `SetNextState()` function. If you take a look at that, you'll see that it is a switch that defines transitions between states. You can override this function to change the transitions or add new ones.

## Adding new states
If you extend the `CharacterState` class, you can define new constants to define new states:

```
    public const int MOVE_COMPLETE = 8;
```

You should choose an unused `int` value for your state, since the value is used as an index for the delegates array. The states are defined as constants and not as an enum because you can't extend an enum in your child class--this seemed the best way to allow you to add new states.
## Other Features
`CharacterStateMachine` includes a Debug setting that displays the object's complete state history on the `CharacterStateMachine` component in the inspector.

## Example
An example scene is included that shows the state machine in use on two different objects.
