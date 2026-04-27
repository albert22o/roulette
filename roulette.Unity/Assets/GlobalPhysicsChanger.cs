using UnityEngine;

public class GlobalPhysicsChanger : Singleton<GlobalPhysicsChanger>
{
    public void IncreaseAccuracy()
    {
        Time.fixedDeltaTime = 0.01f;
        Physics.defaultContactOffset = 0.002f;
        Physics.defaultSolverIterations = 16;
        Physics.defaultSolverVelocityIterations = 6;
    }

    public void DecreaseAccuracy()
    {
        Time.fixedDeltaTime = 0.02f;
        Physics.defaultContactOffset = 0.01f;
        Physics.defaultSolverIterations = 6;
        Physics.defaultSolverVelocityIterations = 1;
    }
}
