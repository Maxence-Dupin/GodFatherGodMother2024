using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<Spell> _spellsList;
    
    [Header("Start Parameters")]
    [SerializeField] private bool _playerTurn;
    [SerializeField] private float _gameTimer;

    [Header("Entity Set Up")] 
    [SerializeField] private Player _player;
    [SerializeField] private Dragon _enemy;
    
    private float _currentTurnDuration;

    private bool _gameOver;

    private static GameManager _instance;

    #endregion

    #region Properties

    public List<Spell> SpellsList => _spellsList;

    public static GameManager Instance => _instance;

    #endregion

    #region Public Methods

    public string CallSpellEvent(string actionWord, string elementWord)
    {
        for (var i = 0; i < _spellsList.Count; i++)
        {
            var spell = _spellsList[i];
            
            if (actionWord == spell.Action.ToString() && elementWord == spell.Element.ToString())
            {
                spell.onEventTriggered.Invoke();
                return spell.Message;
            }
        }

        return null;
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
        _spellsList[0].onEventTriggered += ThrowFireSpellAction;
        
        NextTurn();
    }

    private void Update()
    {
        if (_gameOver) return;
        
        _gameTimer -= Time.deltaTime;

        if (_gameTimer <= 0 && !_gameOver)
        {
            _gameOver = true;
            //Debug.Log("Game Over");
        }
    }

    #endregion

    #region Private Methods
    
    private void NextTurn()
    {
        if (_gameOver) return;
        
        if (_enemy.Health > 0)
        {
            if (_playerTurn)
            {
               //Debug.Log("Tour joueur");

               StartCoroutine(WaitEndOfTurn());
            }
            else
            { 
                //Debug.Log("Tour ennemi");
                
                StartCoroutine(WaitEndOfTurn());
            }
        }
        else
        {
            //Debug.Log("Victoire");
        }
    }

    private void ThrowFireSpellAction()
    {
        Debug.Log("Throw fire");
    }

    private IEnumerator WaitEndOfTurn()
    {
        _currentTurnDuration = 1f;

        yield return new WaitForSeconds(_currentTurnDuration);

        _playerTurn = !_playerTurn;
        NextTurn();
    }

    #endregion
}
