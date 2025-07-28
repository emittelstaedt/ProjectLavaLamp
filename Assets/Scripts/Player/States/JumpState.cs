using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController player) : base(player) { }

    public override void EnterState()
    {
        player.YVelocity = player.PlayerStats.JumpForce;
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            // Play landing sound once player hits ground.
            AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.Land, 0.5f, player.transform);

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