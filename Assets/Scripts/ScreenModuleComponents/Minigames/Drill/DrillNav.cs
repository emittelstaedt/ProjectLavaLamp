using UnityEngine;
using UnityEngine.Splines;

public class DrillNav : MonoBehaviour
{
    private SplineVerticalClamper verticalClamper;
    [SerializeField] private VoidEventChannelSO goalCollision;
    [SerializeField] private VoidEventChannelSO obstacleCollision;
    [SerializeField] public float targetMovementSpeed = 1f;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        verticalClamper = GetComponent<SplineVerticalClamper>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     // Automatically called by Unity when entering an "Is Trigger" collider
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DrillGoal")){
            goalCollision.RaiseEvent();
            Debug.Log("Hit the goal!");
        }
        else if(other.CompareTag("DrillObstacle")){
            obstacleCollision.RaiseEvent();
            Debug.Log("Hit an Obstacle!");
        }
        // 1. Try to find the junction script on the object we just bumped into
        else if (other.TryGetComponent<Junction>(out var junction))
        {
            // 2. verify that a target track was actually assigned in the inspector
            if (junction.switchSpline != null && !junction.isInTruePosition)
            {
                // Change: Update the vertical clamper instead of SplineAnimate
                verticalClamper.ChangeActiveSpline(junction.switchSpline, 0);

                // Debug.Log("Switched Spline via Clamper!");
            }
        }
        else if (other.TryGetComponent<InitJunction>(out var initJunction))
        {
            if (initJunction.switchSpline != null)
            {
                // Change: Update the vertical clamper instead of SplineAnimate
                verticalClamper.ChangeActiveSpline(initJunction.switchSpline, 0);
                
                // Note: You no longer need NormalizedTime = 0f or Restart()!
                // Your custom horizontal movement script will naturally keep pushing the object forward,
                // and the clamper will instantly calculate the vertical height on the new spline.

                // Debug.Log("Successfully loaded onto new preset via Clamper!");
            }
        }
    }
}
