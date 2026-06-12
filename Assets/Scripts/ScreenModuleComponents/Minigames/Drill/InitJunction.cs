using UnityEngine;
using UnityEngine.Splines;

public class InitJunction : MonoBehaviour
{
    [Tooltip("InitJunctions are at the start of each drill map, and snap the drill to them from its reset position.")]
    [SerializeField] public SplineContainer switchSpline;

}