using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image _darkPanel;
    [SerializeField] private RectTransform _grimoireTransform;

    private bool _grimoireOpened;

    private GrimoireLetter[] _grimoireLetters;

    private static GrimoireManager _instance;

    #endregion

    #region Properties

    public static GrimoireManager Instance => _instance;

    #endregion

    #region Public Methods

    public void ToggleGrimoire()
    {
        _grimoireOpened = !_grimoireOpened;
        
        if (_grimoireOpened)
        {
            _darkPanel.gameObject.SetActive(true);
            _darkPanel.DOFade(0.5f, 0.5f);
            _grimoireTransform.DOAnchorPosX(0, 0.5f);
        }
        else
        {
            _grimoireTransform.DOAnchorPosX(1500, 0.5f);
            _darkPanel.DOFade(0f, 0.5f);
            StartCoroutine(WaitToDisable());
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(0.5f);

            _darkPanel.gameObject.SetActive(false);
        }
    }

    public void CheckGrimoireLetters(string firstWord, string secondWord)
    {
        for (var i = 0; i < firstWord.Length; i++)
        {
            for (var j = 0; j < _grimoireLetters.Length; j++)
            {
                var letter = _grimoireLetters[j];

                if (letter.LetterText.enabled) continue;

                if (letter.Letter == firstWord[i])
                {
                    letter.LetterText.enabled = true;
                }
            }
        }
        
        for (var i = 0; i < secondWord.Length; i++)
        {
            for (var j = 0; j < _grimoireLetters.Length; j++)
            {
                var letter = _grimoireLetters[j];

                if (letter.LetterText.enabled) continue;

                if (letter.Letter == secondWord[i])
                {
                    letter.LetterText.enabled = true;
                }
            }
        }
    }

    #endregion

    #region Unity Event Functions
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }

        _instance = this;
    }

    private void Start()
    {
        _grimoireLetters = GetComponentsInChildren<GrimoireLetter>();
    }

    #endregion
}
