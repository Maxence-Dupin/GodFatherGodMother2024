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

    private ActionWords _currentActionWord;
    private ElementWords _currentElementWords;

    #endregion

    #region Properties

    public List<Spell> SpellsList => _spellsList;

    #endregion

    #region Unity Event Functions

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
            Debug.Log("Game Over");
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
               Debug.Log("Tour joueur");

               StartCoroutine(WaitEndOfTurn());
            }
            else
            { 
                Debug.Log("Tour ennemi");
                
                StartCoroutine(WaitEndOfTurn());
            }
        }
        else
        {
            Debug.Log("Victoire");
        }
    }

    private void CallSpellEvent()
    {
        for (var i = 0; i < _spellsList.Count; i++)
        {
            var spell = _spellsList[i];
            
            if (_currentActionWord == spell.Action && _currentElementWords == spell.Element)
            {
                spell.onEventTriggered.Invoke();
            }
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
