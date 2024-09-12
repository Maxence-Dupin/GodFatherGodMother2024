using TMPro;
using UnityEngine;

public class GrimoireWord : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _word;
    [SerializeField] private TextMeshProUGUI[] _letterTexts;

    #endregion

    #region Properties

    public string Word => _word;

    public TextMeshProUGUI[] LetterTexts => _letterTexts;

    #endregion

    #region Unity Event Functions

    private void Start()
    {
        for (var i = 0; i < _word.Length; i++)
        {
            _letterTexts[i].text = _word[i].ToString();
        }
    }

    #endregion
}
