using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour, IScreen
{
    [SerializeField] private Canvas instructionCanvas;
    [SerializeField] private Image instructionsImage;
    [SerializeField] BuildInstructionsSO instructions;
    private int currentPage;

    public bool isActive()
    {
        if (!instructionCanvas.enabled)
        {
            return false;
        }

        return true;
    }

    public void Awake()
    {
        DeactivateScreen();
    }

    public void ActivateScreen()
    {
        instructionCanvas.enabled = true;
    }

    public void DeactivateScreen()
    {
        instructionCanvas.enabled = false;
    }

    public void NextPage()
    {
        if (!isActive())
        {
            return;
        }
        
        if (currentPage < instructions.Pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (!isActive())
        {
            return;
        }

        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int pageIndex)
    {
        instructionsImage.sprite = instructions.Pages[pageIndex];
    }
}