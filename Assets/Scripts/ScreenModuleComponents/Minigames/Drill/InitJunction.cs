using UnityEngine;
using UnityEngine.Splines;

public class InitJunction : MonoBehaviour
{
    [Tooltip("InitJunctions are at the start of each drill map, and snap the drill to them from its reset position.")]
    [SerializeField] public SplineContainer switchSpline;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}