using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandConsole : MonoBehaviour
{
    #region Fields

    [SerializeField] private TMP_InputField _inputFieldPrefab;
    [SerializeField] private TextMeshProUGUI _textPrefab;

    [Space(10)] private Transform _viewport;

    private TMP_InputField _currentInputField;
    private TextMeshProUGUI _currentText;

    #endregion

    #region Public Methods

    public void EnterCommand()
    {
        var words = new List<string>(_currentInputField.text.Split(" "));

        for (var i = 0 ; i < words.Count ; i++)
        {
            if (words[i] != "" && words[i] != " ") continue;
            
            words.Remove(words[i]);
            i--;
        }
        
        GrimoireManager.Instance.CheckGrimoireLetters(words);

        if (words.Count == 2)
        {
            var consoleMessage = GameManager.Instance.CallSpellEvent(words[0], words[1]);
            _currentText.text = consoleMessage ?? "Commande inconnue.";

            if (consoleMessage == null)
            {
                GameManager.Instance.onBadCommand.Invoke();
            }
        }
        else
        {
            _currentText.text = "Commande inconnue.";
            GameManager.Instance.onBadCommand?.Invoke();
        }

        _currentInputField.text = "";
        
        _currentInputField.Select();
        _currentInputField.ActivateInputField();

    }

    #endregion

    #region Unity Event Functions

    private void Start()
    {
        _currentInputField = _inputFieldPrefab;
        _currentText = _textPrefab;
        
        _currentInputField.Select();
        _currentInputField.ActivateInputField();
    }

    private void Update()
    {
        if (GrimoireManager.Instance.PauseMenuOpened) return;

            if (EventSystem.current.currentSelectedGameObject != _currentInputField.gameObject)
        {
            _currentInputField.Select();
            _currentInputField.ActivateInputField();
        }
    }

    #endregion
}
