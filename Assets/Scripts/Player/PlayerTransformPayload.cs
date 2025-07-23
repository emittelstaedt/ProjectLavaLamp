using UnityEngine;
using UnityEngine.UIElements;

public struct PlayerTransformPayload
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Quaternion CameraRotation;
    
    public PlayerTransformPayload(Vector3 position, Quaternion rotation, Quaternion cameraRotation)
    {
        Position = position;
        Rotation = rotation;
        CameraRotation = cameraRotation;

    }
}
