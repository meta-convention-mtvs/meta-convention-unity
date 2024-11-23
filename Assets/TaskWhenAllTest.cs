using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskWhenAllTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Test();
    }

    async Task Test()
    {
        Task<string> task1 = Task.FromResult("task1");
        Task<string> task2 = Task.FromResult("task2");
        Task<string> task3 = Task.FromResult("task3");

        Task<string[]> allTasks = Task.WhenAll(task1, task2, task3);

        string[] results = await allTasks;

        Array.ForEach(results, print);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
