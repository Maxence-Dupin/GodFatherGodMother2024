using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<Spell> _spellsList;

    [Header("Start Parameters")]
    [SerializeField] private bool _playerTurn;
    [SerializeField] private float _gameTimer;

    [Header("Set Up")] 
    [SerializeField] private Player _player;
    [SerializeField] private Dragon _enemy;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Slider _dragonHealth;

    private int _baseDragonHealth;
    private USBDeviceName _selectedDragonHead;

    private float _currentTurnDuration;
    //Current turn action, hydra or player !
    private ENTITIES_ACTIONS _currentTurnAction;

    private bool _gameOver;

    private static GameManager _instance;

    public Action onBadCommand;

    public Action<USBDeviceName> onHeadChange;

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
                spell.onEventTriggered?.Invoke();
                return spell.Message;
            }
        }

        onBadCommand.Invoke();
        return null;
    }

    public SPELLREACTION CalculateReaction(SPELLSTATE spellStateATK, SPELLSTATE spellStateDEF)
    {
        SPELLREACTION _spellReaction = SPELLREACTION.NO_REACT;
        switch (spellStateATK)
        {
            case SPELLSTATE.EAU:
                if(spellStateDEF == SPELLSTATE.EAU)
                {
                    _spellReaction = SPELLREACTION.NEUTRAL;
                }
                if (spellStateDEF == SPELLSTATE.FEU)
                {
                    _spellReaction = SPELLREACTION.WEAKNESS;
                }
                if (spellStateDEF == SPELLSTATE.PLANTE)
                {
                    _spellReaction = SPELLREACTION.RESIST;
                }
                break;
            case SPELLSTATE.PLANTE:
                if (spellStateDEF == SPELLSTATE.EAU)
                {
                    _spellReaction = SPELLREACTION.WEAKNESS;
                }
                if (spellStateDEF == SPELLSTATE.FEU)
                {
                    _spellReaction = SPELLREACTION.RESIST;
                }
                if (spellStateDEF == SPELLSTATE.PLANTE)
                {
                    _spellReaction = SPELLREACTION.NEUTRAL;
                }
                break;
            case SPELLSTATE.FEU:
                if (spellStateDEF == SPELLSTATE.EAU)
                {
                    _spellReaction = SPELLREACTION.RESIST;
                }
                if (spellStateDEF == SPELLSTATE.FEU)
                {
                    _spellReaction = SPELLREACTION.NEUTRAL;
                }
                if (spellStateDEF == SPELLSTATE.PLANTE)
                {
                    _spellReaction = SPELLREACTION.WEAKNESS;
                }
                break;
            case SPELLSTATE.None:
                break;
            default:
                break;
        }

        return _spellReaction;
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
        _spellsList[0].onEventTriggered += AnalyserEau;
        _spellsList[1].onEventTriggered += AnalyserFeu;
        _spellsList[2].onEventTriggered += AnalyserPlante;
        _spellsList[4].onEventTriggered += AnalyserTete;
        _spellsList[8].onEventTriggered += BoirePotion;
        _spellsList[11].onEventTriggered += ChargerFeu;
        _spellsList[12].onEventTriggered += ChargerPlante;
        _spellsList[15].onEventTriggered += DefendreEau;
        _spellsList[16].onEventTriggered += DefendreFeu;
        _spellsList[17].onEventTriggered += DefendrePlante;
        _spellsList[20].onEventTriggered += LancerEau;
        _spellsList[21].onEventTriggered += LancerFeu;
        _spellsList[22].onEventTriggered += LancerPlante;

        onBadCommand += BadCommand;

        onHeadChange += HeadChange;

        _baseDragonHealth = _enemy.Health;

        NextTurn();
    }

    private void Update()
    {
        if (_gameOver) return;
        
        _gameTimer -= Time.deltaTime;
        var timerValue = (int)_gameTimer + 1;
        _timerText.text = timerValue.ToString();

        if (_gameTimer <= 0 && !_gameOver)
        {
            _gameOver = true;
            _timerText.text = "0";
            //Debug.Log("Game Over");
        }
    }
    
    private void OnDestroy()
    {
        _spellsList[0].onEventTriggered -= AnalyserEau;
        _spellsList[1].onEventTriggered -= AnalyserFeu;
        _spellsList[2].onEventTriggered -= AnalyserPlante;
        _spellsList[4].onEventTriggered -= AnalyserTete;
        _spellsList[8].onEventTriggered -= BoirePotion;
        _spellsList[11].onEventTriggered -= ChargerFeu;
        _spellsList[12].onEventTriggered -= ChargerPlante;
        _spellsList[15].onEventTriggered -= DefendreEau;
        _spellsList[16].onEventTriggered -= DefendreFeu;
        _spellsList[17].onEventTriggered -= DefendrePlante;
        _spellsList[20].onEventTriggered -= LancerEau;
        _spellsList[21].onEventTriggered -= LancerFeu;
        _spellsList[22].onEventTriggered -= LancerPlante;

        onBadCommand -= BadCommand;

        onHeadChange -= HeadChange;
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

               //Animation
               StartCoroutine(WaitEndOfTurn());
            }
            else
            {
                //Debug.Log("Tour ennemi");

                //Animation
                StartCoroutine(WaitEndOfTurn());
            }
        }
        else
        {
            //Debug.Log("Victoire");
        }
    }

    private void LancerFeu()
    {
        Debug.Log("Throw fire");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
    }
    
    private void LancerEau()
    {
        Debug.Log("Throw water");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
    }
    
    private void LancerPlante()
    {
        Debug.Log("Throw plant");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
    }
    
    private void BoirePotion()
    {
        _gameTimer += 20;
        _currentTurnAction = ENTITIES_ACTIONS.BOIRE;
    }
    
    private void ChargerFeu()
    {
        Debug.Log("Charge Fire");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;
    }
    
    private void ChargerPlante()
    {
        Debug.Log("Charge Plant");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;

    }

    private void AnalyserFeu()
    {
        Debug.Log("Analyse fire");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;

    }

    private void AnalyserEau()
    {
        Debug.Log("Analyse water");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;

    }

    private void AnalyserPlante()
    {
        Debug.Log("Analyse plant");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;

    }

    private void AnalyserTete()
    {
        Debug.Log("Analyse head");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;

    }

    private void DefendreEau()
    {
        Debug.Log("Defend water");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;

    }

    private void DefendreFeu()
    {
        Debug.Log("Defend feu");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;

    }

    private void DefendrePlante()
    {
        Debug.Log("Defend plant");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;

    }

    //If a command who's not existing is called
    private void BadCommand()
    {
        Debug.Log("Error");
    }

    //When HeadEvent is called head change
    private void HeadChange(USBDeviceName headSelected)
    {
        _selectedDragonHead = headSelected;
    }

    private IEnumerator WaitEndOfTurn()
    {
        _currentTurnDuration = 1f;

        yield return new WaitForSeconds(_currentTurnDuration);

        //Hydre turn based
        if (!_playerTurn) NextTurn();

        _playerTurn = !_playerTurn;
    }

    #endregion

    #region Enum
    public enum SPELLSTATE
    {
        None,
        EAU,
        FEU,
        PLANTE
    }

    public enum SPELLREACTION
    {
        NO_REACT,
        NEUTRAL,
        WEAKNESS,
        RESIST
    }

    private enum ENTITIES_ACTIONS
    {
        LANCER,
        CHARGER,
        BOIRE,
        DEFENDRE,
        ANALYSER
    }

    #endregion
}
