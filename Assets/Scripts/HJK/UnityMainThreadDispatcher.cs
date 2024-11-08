using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class UnityMainThreadDispatcher : Singleton<UnityMainThreadDispatcher>
{
    private readonly Queue<Action> executionQueue = new Queue<Action>();

    public void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
} 