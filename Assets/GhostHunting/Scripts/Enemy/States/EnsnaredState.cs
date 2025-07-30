using UnityEngine;

public class EnsnaredState : EnemyState
{
    private EnemyController _controller;
    private Transform _playerTransform;

    public EnsnaredState(EnemyController controller) : base(controller)
    {
        this._controller = controller;

        if (EnemyManager.IsInitialized)
        {
            _playerTransform = EnemyManager.Instance.playerTransform;
        }
        else
        {
            // Fallback to player tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
          
        }
    }
    public override void UpdateState()
    {
        // Do nothing for testing
    }

    public override void OnEnter()
    {
        _controller.isEnsnared = true;
        FacePlayer();
    }

    private void FacePlayer()
    {
        _controller.movement.RotateToPoint(_playerTransform.position);
    }
}
