using TMPro;
using UnityEngine;

public class GrimoireLetter : MonoBehaviour
{
    #region Fields

    [SerializeField] private char _letter;
    [SerializeField] private TextMeshProUGUI _letterText;

    #endregion

    #region Properties

    public char Letter => _letter;

    public TextMeshProUGUI LetterText => _letterText;

    #endregion

    #region Unity Event Functions

    private void Start()
    {
        _letterText.text = _letter.ToString();
    }

    #endregion
}
