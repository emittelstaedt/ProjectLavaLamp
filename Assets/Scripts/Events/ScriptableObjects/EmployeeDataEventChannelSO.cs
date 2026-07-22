using UnityEngine;

/// <summary>
/// General event channel that broadcasts and carries EmployeeData payload.
/// </summary>
[CreateAssetMenu(menuName = "Events/Employee Data Event Channel", fileName = "EmployeeDataEventChannel")]
public class EmployeeDataEventChannelSO : GenericEventChannelSO<EmployeeData>
{
}