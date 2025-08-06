using UnityEngine;

public class SirenTestScript : MonoBehaviour
{

    [SerializeField] private VoidEventChannelSO siren2Start;
    [SerializeField] private VoidEventChannelSO siren2Stop;

    [SerializeField] private VoidEventChannelSO siren3Start;
    [SerializeField] private VoidEventChannelSO siren3Stop;

    [SerializeField] private VoidEventChannelSO siren4Start;
    [SerializeField] private VoidEventChannelSO siren4Stop;

    [SerializeField] private VoidEventChannelSO siren5Start;
    [SerializeField] private VoidEventChannelSO siren5Stop;

    private bool playing2;
    private bool playing3;
    private bool playing4;
    private bool playing5;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playing2)
            {
                siren2Stop.RaiseEvent();
            }
            else
            {
                siren2Start.RaiseEvent();
            }
            playing2 = !playing2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (playing3)
            {
                siren3Stop.RaiseEvent();
            }
            else
            {
                siren3Start.RaiseEvent();
            }
            playing3 = !playing3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (playing4)
            {
                siren4Stop.RaiseEvent();
            }
            else
            {
                siren4Start.RaiseEvent();
            }
            playing4 = !playing4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (playing5)
            {
                siren5Stop.RaiseEvent();
            }
            else
            {
                siren5Start.RaiseEvent();
            }
            playing5 = !playing5;
        }
    }
}
