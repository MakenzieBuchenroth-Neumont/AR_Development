using UnityEngine;

public class CapturedState : EnemyState
{
    private EnemyController _controller;
    public CapturedState(EnemyController controller) : base(controller)
    {
        _controller = controller;
    }

    public override void OnEnter()
    {
        _controller.isCaptured = true;
        _controller.isEnsnared = false;
        AddGhostToInventory();

        if (EnemyManager.IsInitialized)
        {
            EnemyManager.Instance.EnemyCaptured();
        }
    }
    public override void UpdateState()
    {
        //
    }

    public override void OnExit()
    {
        //
    }

    private void AddGhostToInventory()
    {
        if (PlayerInventory.IsInitialized)
        {
            PlayerInventory._instance.AddGhostToInventory(_controller.gameObject);
        }
        else
        {
            Debug.LogError("PlayerInventory is not initialized. Cannot add ghost to inventory.");
        }

        if (EnemyManager.IsInitialized)
        {
            EnemyManager.Instance.EnemyCaptured();
        }
    }
}
