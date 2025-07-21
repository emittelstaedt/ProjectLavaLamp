using UnityEngine;
using System;
using System.Collections;

public class CameraSwapper : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.05f;
    private GameObject tempCamera;
    
    public static CameraSwapper Instance = null;

    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<CameraSwapper>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
        tempCamera = transform.GetChild(0).gameObject;
    }
    
    // The action delegate code is run when the transition is completed.
    public void SwapCameras(Camera cameraFrom, Camera cameraTo, Action action)
    {
        StartCoroutine(Transition(cameraFrom, cameraTo, action));
    }
    
    public IEnumerator Transition(Camera cameraFrom, Camera cameraTo, Action action)
    {
        Transform from = cameraFrom.gameObject.transform;
        Transform to = cameraTo.gameObject.transform;

        transform.position = from.position;
        Vector3 startPosition = from.position;

        Quaternion startRotation = from.rotation;
        float timer = 0f;

        cameraFrom.gameObject.SetActive(false);
        tempCamera.SetActive(true);
        
        while((transform.position - to.position).magnitude > 0.05f)
        {
            transform.position = Vector3.Slerp(startPosition, to.position, timer / transitionTime);
            transform.rotation = Quaternion.Slerp(startRotation, to.rotation, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        tempCamera.SetActive(false);
        cameraTo.gameObject.SetActive(true);

        action();
    }
}
