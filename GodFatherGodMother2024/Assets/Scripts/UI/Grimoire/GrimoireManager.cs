using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrimoireManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image _darkPanel;
    [SerializeField] private RectTransform _grimoireTransform;
    [SerializeField] private RectTransform _pauseMenuTransform;

    private bool _grimoireOpened;
    private bool _pauseMenuOpened;

    private GrimoireWord[] _grimoireWords;

    private static GrimoireManager _instance;

    private bool _grimoireAnimation;
    private bool _pauseMenuAnimation;
    
    #endregion

    #region Properties

    public static GrimoireManager Instance => _instance;

    public bool PauseMenuOpened => _pauseMenuOpened;
    
    public bool GrimoireOpened => _grimoireOpened;
    
    #endregion

    #region Public Methods

    public void ToggleGrimoire()
    {
        if (_grimoireAnimation) return;
        
        _grimoireOpened = !_grimoireOpened;
        _grimoireAnimation = true;

        if (_grimoireOpened)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _darkPanel.gameObject.SetActive(true);
            _darkPanel.DOFade(0.75f, 0.5f);
            _grimoireTransform.DOAnchorPosX(0, 0.5f);
            StartCoroutine(WaitEndAnimation());
        }
        else
        {
            _grimoireTransform.DOAnchorPosX(1700, 0.5f);
            if (!_pauseMenuOpened)
            {
                _darkPanel.DOFade(0f, 0.5f);
            }
            StartCoroutine(WaitEndAnimation());
        }

        IEnumerator WaitEndAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            
            _darkPanel.gameObject.SetActive(_grimoireOpened || _pauseMenuOpened);
            _grimoireAnimation = false;
        }
    }
    
    public void TogglePause()
    {
        if (_pauseMenuAnimation) return;

        if (_grimoireOpened)
        {
            ToggleGrimoire();
        }
        
        _pauseMenuOpened = !_pauseMenuOpened;
        _pauseMenuAnimation = true;

        if (_pauseMenuOpened)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _darkPanel.gameObject.SetActive(true);
            _darkPanel.DOFade(0.75f, 0.5f);
            _pauseMenuTransform.DOAnchorPosY(-20f, 0.5f);
            StartCoroutine(WaitEndAnimation());
        }
        else
        {
            _pauseMenuTransform.DOAnchorPosY(-1070f, 0.5f);
            if (!_grimoireOpened)
            {
                _darkPanel.DOFade(0f, 0.5f);
            }
            StartCoroutine(WaitEndAnimation());
        }

        IEnumerator WaitEndAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            
            _darkPanel.gameObject.SetActive(_grimoireOpened || _pauseMenuOpened);
            _pauseMenuAnimation = false;
        }
    }

    public void CheckGrimoireLetters(List<string> words)
    {
        for (var i = 0; i < _grimoireWords.Length; i++)
        {
            for (var j = 0; j < words.Count; j++)
            {
                if (_grimoireWords[i].Word != words[j]) continue;
                
                _grimoireWords[i].UpdateImage();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (_pauseMenuOpened) return;
        
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleGrimoire();
        }
    }

    #endregion
}
