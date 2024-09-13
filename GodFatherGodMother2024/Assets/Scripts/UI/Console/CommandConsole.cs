using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandConsole : MonoBehaviour
{
    #region Fields

    
    [SerializeField] private TMP_InputField _inputFieldPrefab;
    [SerializeField] private TextMeshProUGUI _textPrefab;
    [SerializeField] private TMP_InputField _firstInputField;
    [SerializeField] private TextMeshProUGUI _firstText;
    [SerializeField] private RectTransform _content;

    [Space(10)] private Transform _viewport;

    private TMP_InputField _currentInputField;
    private TextMeshProUGUI _currentText;

    private List<TMP_InputField> _inputFields = new();
    private List<TextMeshProUGUI> _texts = new();

    #endregion

    #region Public Methods

    public void EnterCommand()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        
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

        _currentInputField.interactable = false;

        _currentInputField = Instantiate(_inputFieldPrefab, _content);
        _currentText = Instantiate(_textPrefab, _content);

        _currentInputField.onEndEdit.AddListener((arg0 => EnterCommand()));

        _inputFields.Add(_currentInputField);
        _texts.Add(_currentText);

        if (_inputFields.Count > 2)
        {
            var inputField = _inputFields[0];
            _inputFields.Remove(inputField);
            Destroy(inputField.gameObject);
        }
        
        if (_texts.Count > 2)
        {
            var text = _texts[0];
            _texts.Remove(text);
            Destroy(text.gameObject);
        }
        
        _currentInputField.Select();
        _currentInputField.ActivateInputField();
    }

    public void ShowMessage(string message)
    {
        _currentText.text = message;
    }

    #endregion

    #region Unity Event Functions

    private void Start()
    {
        _currentInputField = _firstInputField;
        _currentText = _firstText;
        
        _inputFields.Add(_currentInputField);
        _texts.Add(_currentText);
        
        _currentInputField.Select();
        _currentInputField.ActivateInputField();
    }

    private void Update()
    {
        if (GrimoireManager.Instance.PauseMenuOpened || GrimoireManager.Instance.GrimoireOpened) return;

            if (EventSystem.current.currentSelectedGameObject != _currentInputField.gameObject)
        {
            _currentInputField.Select();
            _currentInputField.ActivateInputField();
        }
    }

    #endregion
}
