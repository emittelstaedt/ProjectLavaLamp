using UnityEngine;
using System.Collections;

public abstract class PlayerState
{
    private Coroutine footstepCoroutine;
    protected PlayerController player;

    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void EnterState() { }
    public virtual void Update() { }
    public virtual void ExitState() { }
    protected void StartFootSteps(SoundType soundType, float minInterval, float maxInterval)
    {
        // Return if footsteps are already playing.
        if (footstepCoroutine != null)
        {
            return;
        }

        footstepCoroutine = player.StartCoroutine(PlayFootSteps(soundType, minInterval, maxInterval));
    }

    protected void StopFootSteps()
    {
        if (footstepCoroutine != null)
        {
            player.StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }
    
    private IEnumerator PlayFootSteps(SoundType soundType, float minInterval, float maxInterval)
    {
        while (true)
        {
            // This acts as a pause on the footsteps coroutine if the player is in the air.
            while (!player.IsGrounded())
            {
                yield return null;
            }

            AudioManager.Instance.PlaySound(MixerType.SFX, soundType, 0.1f, player.transform);

            float loopInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(loopInterval);
        }
    }
}