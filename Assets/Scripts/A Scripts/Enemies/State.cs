using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class State
{
    private List<Tuple<string, float>> stateHistory;
    private float stateLength=0;
    private float maxLength = 100;
    private int maxStates = 5;

    public State()
    {
        stateHistory = new List<Tuple<string, float>>() {new Tuple<string, float>("stand",1)};
    }

    private void assertMaxLength()
    {
        while (stateLength > maxLength)
        {
            stateLength -= stateHistory[0].Item2;
            stateHistory.RemoveAt(0);
        }
    }
    private void assertMaxStates()
    {
        while (stateHistory.Count > maxStates)
        {
            stateHistory.RemoveAt(0);
        }
    }
    public void assertConstraints()
    {
        assertMaxLength();
        assertMaxStates();
    }

    public void addState(string state)
    {
        if (state == stateHistory.Last().Item1)
        {
            incrementState();
        }
        else
        {
            stateHistory.Add(new Tuple<string,float>(state, 1));
            assertConstraints();
        }
    }
    public Tuple<string,float> getCurState()
    {
        return stateHistory.Last();
    }
    public void incrementState()
    {
        Tuple<string,float> curState = stateHistory.Last();
        stateHistory[stateHistory.Count - 1] = new Tuple<string, float>(curState.Item1, curState.Item2 + 1);
        assertConstraints();
    }

}
