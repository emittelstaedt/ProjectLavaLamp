using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;

    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void EnterState() { }
    public virtual void Update() { }
    public virtual void ExitState() { }
}