using Simulation;
using UnityEngine;

public class ThreadedUpdateTree : ThreadedJob
{
    public Manager manager;

    protected override void ThreadFunction()
    {
        manager.UpdateAffordanceTree();
    }

    protected override void OnFinished()
    {
        Debug.Log("Done");
    }

}