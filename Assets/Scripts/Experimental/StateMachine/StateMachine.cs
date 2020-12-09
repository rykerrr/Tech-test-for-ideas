using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Serializable = System.SerializableAttribute;
using Type = System.Type;
using Activator = System.Activator;

[Serializable]
public class StateMachine
{
    [SerializeField] private List<State> currentStatesForEditing = new List<State>();
    
    private Dictionary<Type, State> availableStates = new Dictionary<Type, State>(); // type of the state, and the state object itself
    [SerializeField] private SerializedDropOutStateStack historyStates; // initialized in StartFrom
    [SerializeField] private State initialState; // we keep track of it cause of restart
    [SerializeField] private State currentState;

    private EnemyAI stateMachineOwner = null;

    public EnemyAI StateMachineOwner
    {
        get { return stateMachineOwner; }
    }
    
    public StateMachine(EnemyAI stateMachineOwner)
    {
        this.stateMachineOwner = stateMachineOwner;
    }
    
    public StateMachine(State initialState, int historyDepth)
    {
        this.initialState = initialState;
        StartFrom(initialState, historyDepth);
    }

    public int Tick() // returns int sort of like a code (main in c)
    {
        currentState?.Tick();
        return 1;
    }

    public void Restart() // but history is kept
    {
        ChangeState(initialState);    
    }
    
    public void StartFrom(State initialState, int historyDepth)
    {
        historyStates = new SerializedDropOutStateStack(historyDepth);
        
        ChangeState(initialState);
    }

    public void ChangeState(State nextState) // is this how the history stack would be implemented?
    {
        State poppedState = historyStates.Pop();
        
        currentState?.Exit();
        currentState = nextState;
        currentState?.Enter();
        
        historyStates.Push(currentState);
    }
    
    public void ChangeStateByKey(Type nextStateKey)
    {
        State nextState = GetStateFromDictionary(nextStateKey);
        
        ChangeState(nextState);
    }

    public void AddStates(params (Type stateKey, State stateObject)[] stateTuples)
    {
        foreach (var tuple in stateTuples)
        {
            if (availableStates.ContainsKey(tuple.stateKey))
            {
                // do nothing
                return;
            }
            else
            {
                availableStates.Add(tuple.stateKey, tuple.stateObject);

                // EnemyState enemyStateObj = tuple.stateObject as EnemyState;
                
                currentStatesForEditing.Add(/*enemyStateObj*/ tuple.stateObject);
            }
        }
    }

    private State GetStateFromDictionary(Type stateKey)
    {
        if (availableStates.ContainsKey(stateKey))
        {
            return availableStates[stateKey];
        }
        else
        {
            return CreateStateInstance(stateKey);
        }
    }

    private State CreateStateInstance(Type typeOfState)
    {
        State newState = Activator.CreateInstance(typeOfState) as State;

        return newState;
    }
}
