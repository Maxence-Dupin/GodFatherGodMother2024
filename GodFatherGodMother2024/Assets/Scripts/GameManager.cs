using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<Spell> _spellsList;
    [Header("Start Parameters")]
    [SerializeField] private bool _playerTurn;

    [Header("Entity Set Up")] 
    [SerializeField] private Entity _player;
    [SerializeField] private Entity _enemy;

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

    #endregion

    #region Private Methods
    
    private void NextTurn()
    {
        if (_player.Health > 0 && _enemy.Health > 0)
        {
            if (_playerTurn)
            {
               Debug.Log("Tour joueur");
            }
            else
            { 
                Debug.Log("Tour ennemi");
            }
        }
        else if (_player.Health <= 0)
        {
            // game over joueur
        }
        else
        {
            // victoire
        }
    }

    private void CallSpellEvent()
    {
        for (var i = 0; i < _spellsList.Count; i++)
        {
            var spell = _spellsList[i];
            
            if (_currentActionWord == spell.action && _currentElementWords == spell.element)
            {
                spell.onEventTriggered.Invoke();
            }
        }
    }

    private void ThrowFireSpellAction()
    {
        Debug.Log("Throw fire");
    }

    #endregion
}
