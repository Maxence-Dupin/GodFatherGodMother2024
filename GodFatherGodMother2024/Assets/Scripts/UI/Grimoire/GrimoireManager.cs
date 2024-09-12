using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image _darkPanel;
    [SerializeField] private RectTransform _grimoireTransform;

    private bool _grimoireOpened;

    private GrimoireWord[] _grimoireWords;

    private static GrimoireManager _instance;

    #endregion

    #region Properties

    public static GrimoireManager Instance => _instance;

    #endregion

    #region Public Methods

    public void ToggleGrimoire()
    {
        _grimoireOpened = !_grimoireOpened;

        _grimoireTransform.DOKill();
        _darkPanel.DOKill();
        
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

    public void CheckGrimoireLetters(List<string> words)
    {
        for (var i = 0; i < _grimoireWords.Length; i++)
        {
            for (var j = 0; j < words.Count; j++)
            {
                if (_grimoireWords[i].Word != words[j]) continue;
                
                for (var k = 0; k < _grimoireWords[i].LetterTexts.Length; k++)
                {
                    _grimoireWords[i].LetterTexts[k].enabled = true;
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
        _grimoireWords = GetComponentsInChildren<GrimoireWord>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            ToggleGrimoire();
        }
    }

    #endregion
}
