using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.IsCrouching = true;
        player.TargetHeight = player.PlayerStats.CrouchHeight;
    }

    public override void Update()
    {
        Vector2 moveValue = player.PlayerInputs.moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = player.CalculateMoveDirection(moveValue);
        bool hasRoomToUncrouch = HasRoomToUncrouch();

        if (!player.PlayerInputs.crouchAction.IsPressed() && hasRoomToUncrouch)
        {
            player.SwitchState(moveValue.magnitude > 0.1f ? new WalkState(player) : new IdleState(player));
            return;
        }

        if (player.PlayerInputs.jumpAction.IsPressed() && player.IsGrounded() && hasRoomToUncrouch)
        {
            player.SwitchState(new JumpState(player));
        }

        player.ApplyGravity();
        player.MovePlayer(moveDirection * (player.PlayerStats.CrouchSpeed));
    }

    public override void ExitState()
    {
        player.IsCrouching = false;
        player.TargetHeight = player.PlayerStats.NormalHeight;
    }
    
    private bool HasRoomToUncrouch()
    {
        float radius = player.CharacterController.radius;
        
        float distanceToBottomSphere = (player.CharacterController.height / 2) - radius;
        float worldDistanceToBottomSphere = distanceToBottomSphere * player.transform.localScale.y;
        
        float distanceToTopSphere = player.PlayerStats.NormalHeight - (player.CharacterController.height / 2) - radius;
        float worldDistanceToTopSphere = distanceToTopSphere * player.transform.localScale.y;
        
        Vector3 bottomSphereCenter = player.transform.position - Vector3.up * worldDistanceToBottomSphere;
        Vector3 topSphereCenter = player.transform.position + Vector3.up * worldDistanceToTopSphere;
        
        return !Physics.CheckCapsule(bottomSphereCenter, topSphereCenter, radius);
    }
}
