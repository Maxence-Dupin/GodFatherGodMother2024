using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandConsole : MonoBehaviour
{
    #region Fields

    [SerializeField] private TMP_InputField _inputFieldPrefab;
    [SerializeField] private TextMeshProUGUI _textPrefab;

    private TMP_InputField _currentInputField;

    #endregion

    #region Public Methods

    public void EnterCommand()
    {
        var words = new List<string>(_currentInputField.text.Split(" "));
        
        for (var i = 0 ; i < words.Count ; i++)
        {
            if (words[i] == "" || words[i] == " ")
            {
                words.Remove(words[i]);
                i--;
            }
            else
            {
                words[i] = words[i].ToUpper();
                Debug.Log(words[i]);
            }
        }

        if (words.Count == 2)
        {
            GameManager.Instance.CallSpellEvent(words[0], words[1]);
        }
    }

    #endregion

    #region Unity Event Functions

    private void Start()
    {
        _currentInputField = _inputFieldPrefab;
    }

    #endregion
}
