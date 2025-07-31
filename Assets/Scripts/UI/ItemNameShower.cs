using System.Text;
using TMPro;
using UnityEngine;

public class ItemNameShower : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro == null )
        {
            Debug.LogWarning("erorr");
        }
    }

    public void UpdateText(string name)
    {
        textMeshPro.text = AddSpacesToString(name);
    }

    private string AddSpacesToString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        StringBuilder newText = new(text.Length * 2);
        newText.Append(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
            {
                newText.Append(' ');
            }
            newText.Append(text[i]);
        }

        return newText.ToString();
    }
}
