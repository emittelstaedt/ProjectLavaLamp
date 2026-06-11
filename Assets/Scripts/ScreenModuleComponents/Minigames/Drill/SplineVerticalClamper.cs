using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineVerticalClamper : MonoBehaviour
{
    [Header("Spline Target")]
    [SerializeField] public SplineContainer splineContainer;
    [SerializeField] private int splineIndex = 0;

    [Header("Settings")]
    [Tooltip("Keep checked to align the object's rotation with the spline's incline.")]
    [SerializeField] private bool updateRotation = true;

    void LateUpdate()
    {
        if (splineContainer == null) return;

        // 1. Get the current position
        Vector3 currentPosition = transform.position;

        // 2. Convert the position to the local space of the spline container
        Vector3 localQuery = splineContainer.transform.InverseTransformPoint(currentPosition);

        // 3. Find the nearest point on the mathematical curve relative to X coordinate
        SplineUtility.GetNearestPoint(
            splineContainer.Splines[splineIndex], 
            localQuery, 
            out float3 localNearestPoint, 
            out float normalizedTime, 
            100, // Iterations (high resolution prevents vertical jitter)
            2    // Subdivisions
        );

        // 4. Convert the matched point back to world space coordinates
        Vector3 worldNearestPoint = splineContainer.transform.TransformPoint(localNearestPoint);

     
        transform.position = new Vector3(currentPosition.x, worldNearestPoint.y, worldNearestPoint.z);

        // 6. Handle slope rotation alignment
        if (updateRotation)
        {
            Vector3 tangent = splineContainer.EvaluateTangent(splineIndex, normalizedTime);
            if (tangent != Vector3.zero)
            {
                // 1. Convert the local spline tangent into a world-space direction
                Vector3 worldForward = splineContainer.transform.TransformDirection(tangent).normalized;
                
                // 2. Create a clean rotation using the world direction and standard global Up
                transform.rotation = Quaternion.LookRotation(worldForward, Vector3.up);
            }
        }
    }

    /// <summary>
    /// Call this from your branching logic script to switch paths instantly.
    /// </summary>
    public void ChangeActiveSpline(SplineContainer newContainer, int newSplineIndex)
    {
        splineContainer = newContainer;
        splineIndex = newSplineIndex;
    }
}
