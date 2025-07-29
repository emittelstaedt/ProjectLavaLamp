using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.YVelocity = player.PlayerStats.JumpForce;
    }

    public override void ExitState()
    {
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Land, 0.4f, player.transform);
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            if (player.IsMoving())
            {
                player.SwitchState(new WalkState(player));
            }
            else
            {
                player.SwitchState(new IdleState(player));
            }
        }
    }
}