using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
                spell.onEventTriggered?.Invoke();
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

    private void LancerFeu()
    {
        Debug.Log("Throw fire");
    }
    
    private void LancerEau()
    {
        Debug.Log("Throw water");
    }
    
    private void LancerPlante()
    {
        Debug.Log("Throw plant");
    }
    
    private void BoirePotion()
    {
        Debug.Log("Drink Potion");
    }
    
    private void ChargerFeu()
    {
        Debug.Log("Charge Fire");
    }
    
    private void ChargerPlante()
    {
        Debug.Log("Charge Plant");
    }
    
    private void AnalyserFeu()
    {
        Debug.Log("Analyse fire");
    }
    
    private void AnalyserEau()
    {
        Debug.Log("Analyse water");
    }
    
    private void AnalyserPlante()
    {
        Debug.Log("Analyse plant");
    }
    
    private void AnalyserTete()
    {
        Debug.Log("Analyse head");
    }
    
    private void DefendreEau()
    {
        Debug.Log("Defend water");
    }
    
    private void DefendreFeu()
    {
        Debug.Log("Defend feu");
    }
    
    private void DefendrePlante()
    {
        Debug.Log("Defend plant");
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
