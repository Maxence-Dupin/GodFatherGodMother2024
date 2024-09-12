using TMPro;
using UnityEngine;

public class UppercaseInput : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(EnforceUppercase);
        }
    }

    public void EnforceUppercase(string input)
    {
        inputField.text = input.ToUpper();
    }
}