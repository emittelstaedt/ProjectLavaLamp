using UnityEngine;

public class CameraBobble : MonoBehaviour
{
    [Header("Breathing Rotation Settings")]
    public float baseRotationAmount = 0.2f; // Base degrees
    public float baseBreathingSpeed = 0.5f; // Base cycles per second

    [Header("Breathing Position Settings")]
    public float basePositionAmount = 0.02f; // Base Y bob
    public bool useLocalPosition = true;

    [Header("Randomness Settings")]
    public float amplitudeVariation = 0.02f; // How much amplitude can vary
    public float speedVariation = 0.1f;      // How much speed can vary
    public float variationChangeSpeed = 0.3f; // How fast variations change over time

    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private float currentRotationAmount;
    private float currentPositionAmount;
    private float currentSpeed;

    private float targetRotationAmount;
    private float targetPositionAmount;
    private float targetSpeed;

    void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = useLocalPosition ? transform.localPosition : transform.position;

        // Initialize current values
        currentRotationAmount = baseRotationAmount;
        currentPositionAmount = basePositionAmount;
        currentSpeed = baseBreathingSpeed;

        SetNewTargets();
    }

    void Update()
    {
        // Smoothly interpolate towards new random targets
        currentRotationAmount = Mathf.Lerp(currentRotationAmount, targetRotationAmount, Time.deltaTime * variationChangeSpeed);
        currentPositionAmount = Mathf.Lerp(currentPositionAmount, targetPositionAmount, Time.deltaTime * variationChangeSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * variationChangeSpeed);

        // Apply breathing effect
        float time = Time.time * currentSpeed * Mathf.PI * 2;
        float xRotation = Mathf.Sin(time) * currentRotationAmount;
        float zRotation = Mathf.Cos(time) * currentRotationAmount * 0.5f;

        transform.localRotation = initialRotation * Quaternion.Euler(xRotation, 0f, zRotation);

        float yOffset = Mathf.Sin(time) * currentPositionAmount;
        if (useLocalPosition)
            transform.localPosition = initialPosition + new Vector3(0f, yOffset, 0f);
        else
            transform.position = initialPosition + new Vector3(0f, yOffset, 0f);

        // Occasionally pick new random targets
        if (Random.value < 0.01f) // ~1% chance per frame (~once per second)
        {
            SetNewTargets();
        }
    }

    void SetNewTargets()
    {
        targetRotationAmount = baseRotationAmount + Random.Range(-amplitudeVariation, amplitudeVariation);
        targetPositionAmount = basePositionAmount + Random.Range(-amplitudeVariation * 0.5f, amplitudeVariation * 0.5f);
        targetSpeed = baseBreathingSpeed + Random.Range(-speedVariation, speedVariation);
    }
}
