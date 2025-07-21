using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController controller;

    public EnemyState(EnemyController controller)
    {
        this.controller = controller;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public abstract void UpdateState();
}
