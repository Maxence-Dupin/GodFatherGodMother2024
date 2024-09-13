using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private List<Spell> _spellsList;

    [Header("Parameters")]
    [SerializeField] private bool _playerTurn;
    [SerializeField] private float _gameTimer;
    [SerializeField] private int _cooldownTurnsNumber;
    [SerializeField] private int _fireHeadTimerDamage;
    [SerializeField] private int _plantHeadCooldownOnSpell;

    [Header("Set Up")] 
    [SerializeField] private Player _player;
    [SerializeField] private Dragon _enemy;
    [SerializeField] private CommandConsole _console;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Slider _dragonHealth;
    [SerializeField] private Image _gameOverPanel;
    [SerializeField] private Image _exclamation;

    [Header("Sprite")]
    [SerializeField] private List<Image> _listOfDragonHead; 

    private List<int> _DragonHealth;
    private USBDeviceName _selectedDragonHead;
    private Dragon.DragonHead _headClassSelected;
    private bool _isDragonStun = false;

    private float _currentTurnDuration;
    //Current turn action, hydra or player !
    private ENTITIES_ACTIONS _currentTurnAction;
    private SPELLSTATE _currentPlayerSpellState;
    private SPELLSTATE _currentHydraSpellState;
    private SPELLSTATE _currentChargedSpell;
    private int _countChargedTurn;

    private bool _gameOver;

    private static GameManager _instance;

    public Action onBadCommand;

    public Action<USBDeviceName> onHeadChange;
    
    private Dictionary<string, int> _spellsCooldown = new();

    private bool _underWaterEffect;

    #endregion

    #region Properties

    public List<Spell> SpellsList => _spellsList;

    public static GameManager Instance => _instance;

    public Dictionary<string, int> SpellsCooldown => _spellsCooldown;

    #endregion

    #region Public Methods

    public string CallSpellEvent(string actionWord, string elementWord)
    {
        if (_spellsCooldown.ContainsKey(elementWord) || _spellsCooldown.ContainsKey(actionWord))
        {
            _currentPlayerSpellState = SPELLSTATE.None;
            NextTurn();
            return "Le sort n'est pas utilisable pour le moment.";
        }
        
        for (var i = 0; i < _spellsList.Count; i++)
        {
            var spell = _spellsList[i];

            if (!_underWaterEffect)
            {
                if (actionWord == spell.Action.ToString() && elementWord == spell.Element.ToString())
                {
                    _spellsCooldown.Add(elementWord, _cooldownTurnsNumber + 1);
                    spell.onEventTriggered?.Invoke();
                    NextTurn();
                    _underWaterEffect = false;
                    return spell.Message;
                }
            }
            else
            {
                if (elementWord == spell.Action.ToString() && actionWord == spell.Element.ToString())
                {
                    Debug.Log(elementWord + ", " + actionWord);
                    _spellsCooldown.Add(actionWord, _cooldownTurnsNumber + 1);
                    spell.onEventTriggered?.Invoke();
                    NextTurn();
                    _underWaterEffect = false;
                    return spell.Message;
                }
            }
        }

        _underWaterEffect = false;
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

        _spellsList[0].onEventTriggered += AnalyserEau;
        _spellsList[1].onEventTriggered += AnalyserFeu;
        _spellsList[2].onEventTriggered += AnalyserPlante;
        _spellsList[4].onEventTriggered += AnalyserTete;
        _spellsList[6].onEventTriggered += BoireFeu;
        _spellsList[8].onEventTriggered += BoirePotion;
        _spellsList[10].onEventTriggered += ChargerEau;
        _spellsList[11].onEventTriggered += ChargerFeu;
        _spellsList[12].onEventTriggered += ChargerPlante;
        _spellsList[15].onEventTriggered += DefendreEau;
        _spellsList[16].onEventTriggered += DefendreFeu;
        _spellsList[17].onEventTriggered += DefendrePlante;
        _spellsList[20].onEventTriggered += LancerEau;
        _spellsList[21].onEventTriggered += LancerFeu;
        _spellsList[22].onEventTriggered += LancerPlante;
        _spellsList[23].onEventTriggered += LancerPotion;

        onBadCommand += BadCommand;

        onHeadChange += HeadChange;
    }

    private void Start()
    {
        foreach(var head in _enemy.DragonHeads)
        {
            head.Init();
        }
    }

    private void Update()
    {
        if (_gameOver || GrimoireManager.Instance.PauseMenuOpened) return;
        
        _gameTimer -= Time.deltaTime;
        var timerValue = (int)_gameTimer + 1;
        _timerText.text = timerValue.ToString();
        if(_headClassSelected != null)
        {
            _dragonHealth.value = (float)_headClassSelected.HealthPerHead / _headClassSelected.MaxHealthPerHead;
        }
        if (_gameTimer <= 0 && !_gameOver)
        {
            _gameOver = true;
            _timerText.text = "0";
            _gameOverPanel.gameObject.SetActive(true);
        }

        _exclamation.enabled = _spellsCooldown.Count > 0;
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
        
        if(_headClassSelected == null)
        {
            Debug.Log("PAS DE TETE");
        }
        else if (_headClassSelected.HealthPerHead > 0)
        {
            if (_playerTurn)
            {
                Debug.Log("Tour joueur");
                switch (_currentTurnAction)
                {
                    case ENTITIES_ACTIONS.LANCER:
                        SPELLREACTION outcome = SPELLREACTION.NO_REACT;
                        if (_currentHydraSpellState != SPELLSTATE.None)
                        {
                            outcome = CalculateReaction(_currentPlayerSpellState, _currentHydraSpellState);
                        }

                        if (_currentPlayerSpellState == SPELLSTATE.POTION)
                        {
                            HeadDamage(-10);
                        }
                        
                        switch (outcome)
                        {
                            case SPELLREACTION.NO_REACT:
                                Debug.Log("FLOP WTF ?");
                                break;
                            case SPELLREACTION.WEAKNESS:
                                _console.ShowMessage(
                                    "Elle a pris cher ! Vu comme tu l’as sonnée, tu peux sûrement réaliser une nouvelle action avant qu’elle réagisse.");
                                _isDragonStun = true;
                                HeadDamage(10);
                                break;
                            case SPELLREACTION.RESIST:
                                _console.ShowMessage("Oups, ce n’est pas très efficace… J’espère que tu sais faire mieux.");
                                Debug.Log("No DMG");
                                break;
                            case SPELLREACTION.NEUTRAL:
                                _console.ShowMessage("Pas mal, tu peux mieux faire mais tu peux aussi faire pire.");
                                Debug.Log("Take DMG");
                                HeadDamage(5);
                                break;
                            default:
                                break;
                        }
                        Debug.Log(outcome);
                        break;
                    case ENTITIES_ACTIONS.DEFENDRE:
                        switch (_currentPlayerSpellState)
                        {
                            case SPELLSTATE.EAU:
                                Debug.Log("Defense eau");
                                break;
                            case SPELLSTATE.FEU:
                                Debug.Log("Defense feu");
                                break;
                            case SPELLSTATE.PLANTE:
                                Debug.Log("Defense plante");
                                break;
                            default : break;
                        }
                        break;
                    case ENTITIES_ACTIONS.BOIRE:
                        Debug.Log("Il a bu je suis chokbar");
                        break;
                    case ENTITIES_ACTIONS.ANALYSER:
                        switch (_currentPlayerSpellState)
                        {
                            case SPELLSTATE.PLANTE:
                                if (_currentPlayerSpellState != _currentHydraSpellState)
                                {
                                    Debug.Log("Nop marche pas l'analyse chef");
                                }
                                else
                                {
                                    Debug.Log("PLANTE ANALYSE");
                                }
                                break;
                            case SPELLSTATE.EAU:
                                if (_currentPlayerSpellState != _currentHydraSpellState)
                                {
                                    Debug.Log("Nop marche pas l'analyse chef");
                                }
                                else
                                {
                                    Debug.Log("EAU ANALYSE");
                                }
                                break;
                            case SPELLSTATE.FEU:
                                if (_currentPlayerSpellState != _currentHydraSpellState)
                                {
                                    Debug.Log("Nop marche pas l'analyse chef");
                                }
                                else
                                {
                                    Debug.Log("FEU ANALYSE");
                                }
                                break;
                            case SPELLSTATE.TETE:
                                switch (_headClassSelected.Element)
                                {
                                    case SPELLSTATE.None:
                                        break;
                                    case SPELLSTATE.EAU:
                                        _console.ShowMessage("Tête d’eau : Extrêmement humide. Astuce : Utilisez les plantes.");
                                        break;
                                    case SPELLSTATE.FEU:
                                        _console.ShowMessage("Tête de feu : Très chaude. Astuce : Utilisez l’eau.");
                                        break;
                                    case SPELLSTATE.PLANTE:
                                        _console.ShowMessage("Tête de plante : Dépendante du soleil. Astuce : utilisez le feu.");
                                        break;
                                    case SPELLSTATE.TETE:
                                        break;
                                    case SPELLSTATE.POTION:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                    break;
                            default: 
                                break;
                        }
                        break;
                    case ENTITIES_ACTIONS.CHARGER:
                        
                        if (_countChargedTurn > 0) 
                        {
                            Debug.Log("Deja en charge");
                            return; 
                        }
                        Debug.Log("Sort charg�");
                        _countChargedTurn = 2;
                        break;
                }
                //Animation
                StartCoroutine(WaitEndOfTurn());
            }
            else
            {
                //Debug.Log("Tour ennemi");
                //Charged turn

                var temp = new Dictionary<string, int>();
                
                foreach (var pair in _spellsCooldown)
                {
                    temp.Add(pair.Key, pair.Value);
                    Debug.Log(pair.Key + ", " + pair.Value);
                }
                
                foreach (var pair in temp)
                {
                    _spellsCooldown[pair.Key]--;

                    if (_spellsCooldown[pair.Key] <= 0)
                    {
                        _spellsCooldown.Remove(pair.Key);
                    }
                }
                
                if (_countChargedTurn == 2) _countChargedTurn -= 1;
                else if (_countChargedTurn == 1)
                {
                    SPELLREACTION outcome = SPELLREACTION.NO_REACT;
                    if (_currentHydraSpellState != SPELLSTATE.None)
                    {
                        outcome = CalculateReaction(_currentChargedSpell, _currentHydraSpellState);
                    }
                    switch (outcome)
                    {
                        case SPELLREACTION.NO_REACT:
                            Debug.Log("FLOP WTF ? * 2");
                            break;
                        case SPELLREACTION.WEAKNESS:
                            _isDragonStun = true;
                            HeadDamage(15);
                            break;
                        case SPELLREACTION.RESIST:
                            Debug.Log("No DMG * 2");
                            break;
                        case SPELLREACTION.NEUTRAL:
                            Debug.Log("Take DMG * 2");
                            HeadDamage(10);
                            break;
                        default:
                            break;
                    }
                    _currentChargedSpell = SPELLSTATE.None;
                    _countChargedTurn = 0;
                    Debug.Log(outcome);
                }
                //Normal Turn
                if (!_isDragonStun)
                {
                    //Defense
                    if (_currentTurnAction == ENTITIES_ACTIONS.DEFENDRE && _currentPlayerSpellState == _currentHydraSpellState)
                    {
                        Debug.Log("Potite def");
                    }
                    else
                    {
                        switch(_currentHydraSpellState)
                        {
                            case SPELLSTATE.None:
                                Debug.Log("State de l'hydra à none ???");
                                break;
                            case SPELLSTATE.EAU:
                                _console.ShowIndependantMessage("Vous êtes sous l'effet de la tête d'eau, inversez l'ordre des mots pour votre prochaine commande !");
                                _underWaterEffect = true;
                                break;
                            case SPELLSTATE.FEU:
                                _gameTimer -= _fireHeadTimerDamage;
                                break;
                            case SPELLSTATE.PLANTE:
                                SetCooldownOnARandomVerb();
                                break;
                            case SPELLSTATE.TETE:
                                Debug.Log("State de l'hydra sur tete ???");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        Debug.Log("Attaque de l'hydra");
                    }
                    Debug.Log("Roar");
                }
                else 
                {
                    Debug.Log("Stun !");
                    _isDragonStun = false;
                }
                //Animation
                StartCoroutine(WaitEndOfTurn());
            }
        }
        else
        {
            Debug.Log("Victoire");
        }
    }

    //SPELLS
    #region Spells
    private void LancerFeu()
    {
        Debug.Log("Throw fire");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
        _currentPlayerSpellState = SPELLSTATE.FEU;

    }
    
    private void LancerEau()
    {
        Debug.Log("Throw water");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
        _currentPlayerSpellState = SPELLSTATE.EAU;
    }
    
    private void LancerPlante()
    {
        Debug.Log("Throw plant");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
        _currentPlayerSpellState = SPELLSTATE.PLANTE;
    }
    
    private void LancerPotion()
    {
        Debug.Log("Throw potion");
        _currentTurnAction = ENTITIES_ACTIONS.LANCER;
        _currentPlayerSpellState = SPELLSTATE.POTION;
    }
    
    private void BoirePotion()
    {
        _gameTimer += 20;
        _currentTurnAction = ENTITIES_ACTIONS.BOIRE;
    }
    
    private void BoireFeu()
    {
        _gameTimer -= 20;
        _currentTurnAction = ENTITIES_ACTIONS.BOIRE;
    }
    
    private void ChargerFeu()
    {
        Debug.Log("Charge Fire");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;
        _currentPlayerSpellState = SPELLSTATE.FEU;
        _currentChargedSpell = SPELLSTATE.FEU;
    }
    private void ChargerEau()
    {
        Debug.Log("Charge Water");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;
        _currentPlayerSpellState = SPELLSTATE.EAU;
        _currentChargedSpell = SPELLSTATE.EAU;
    }

    private void ChargerPlante()
    {
        Debug.Log("Charge Plant");
        _currentTurnAction = ENTITIES_ACTIONS.CHARGER;
        _currentPlayerSpellState = SPELLSTATE.PLANTE;
        _currentChargedSpell = SPELLSTATE.PLANTE;
    }

    private void AnalyserFeu()
    {
        Debug.Log("Analyse fire");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;
        _currentPlayerSpellState = SPELLSTATE.FEU;

    }

    private void AnalyserEau()
    {
        Debug.Log("Analyse water");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;
        _currentPlayerSpellState = SPELLSTATE.EAU;


    }

    private void AnalyserPlante()
    {
        Debug.Log("Analyse plant");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;
        _currentPlayerSpellState = SPELLSTATE.PLANTE;


    }

    private void AnalyserTete()
    {
        Debug.Log("Analyse head");
        _currentTurnAction = ENTITIES_ACTIONS.ANALYSER;
        _currentPlayerSpellState = SPELLSTATE.TETE;

    }

    private void DefendreEau()
    {
        Debug.Log("Defend water");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;
        _currentPlayerSpellState = SPELLSTATE.EAU;

    }

    private void DefendreFeu()
    {
        Debug.Log("Defend feu");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;
        _currentPlayerSpellState = SPELLSTATE.FEU;

    }

    private void DefendrePlante()
    {
        Debug.Log("Defend plant");
        _currentTurnAction = ENTITIES_ACTIONS.DEFENDRE;

        _currentPlayerSpellState = SPELLSTATE.PLANTE;

    }
    #endregion

    //If a command who's not existing is called
    private void BadCommand()
    {
        Debug.Log("Error");
    }

    //When HeadEvent is called head change
    private void HeadChange(USBDeviceName headSelected)
    {
        _selectedDragonHead = headSelected;

        switch (_selectedDragonHead)
        {
            case USBDeviceName.MiddleHead:
                _headClassSelected = _enemy.DragonHeads[1];
                _currentHydraSpellState = _headClassSelected.Element;
                _listOfDragonHead[0].color = new Color(160, 160, 160, 255);
                _listOfDragonHead[1].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[2].color = new Color(160, 160, 160, 255);
                return;
            case USBDeviceName.RightHead:
                _headClassSelected = _enemy.DragonHeads[2];
                _currentHydraSpellState = _headClassSelected.Element;
                _listOfDragonHead[0].color = new Color(160, 160, 160, 255);
                _listOfDragonHead[1].color = new Color(160, 160, 160, 255);
                _listOfDragonHead[2].color = new Color(255, 255, 255, 255);
                return;
            case USBDeviceName.LeftHead:
                Debug.Log("yo les mecs");
                _headClassSelected = _enemy.DragonHeads[0];
                _currentHydraSpellState = _headClassSelected.Element;
                _listOfDragonHead[0].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[1].color = new Color(160, 160, 160, 255);
                _listOfDragonHead[2].color = new Color(160, 160, 160, 255);
                return;
            case USBDeviceName.Multiple:
                Debug.Log("Error");
                _headClassSelected = null;
                _currentHydraSpellState = SPELLSTATE.None;
                _listOfDragonHead[0].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[1].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[2].color = new Color(255, 255, 255, 255);
                return;
            case USBDeviceName.None:
                Debug.Log("Error");
                _headClassSelected = null;
                _currentHydraSpellState = SPELLSTATE.None;
                _listOfDragonHead[0].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[1].color = new Color(255, 255, 255, 255);
                _listOfDragonHead[2].color = new Color(255, 255, 255, 255);
                return;
            default:
                break;
        }
    }

    private void HeadDamage(int dmg)
    {
        _headClassSelected.HealthPerHead -= dmg;
    }
    private void SetCooldownOnARandomVerb()
    {
        string spellToBlock = null;

        while (spellToBlock == null || _spellsCooldown.ContainsKey(spellToBlock))
        {
            var random = Random.Range(0, 5);

            spellToBlock = random switch
            {
                0 => "BOIRE",
                1 => "LANCER",
                2 => "CHARGER",
                3 => "ANALYSER",
                4 => "DEFENDRE",
                _ => null
            };
        }
        
        Debug.Log(spellToBlock);

        _spellsCooldown.Add(spellToBlock, _plantHeadCooldownOnSpell);
    }

    private IEnumerator WaitEndOfTurn()
    {
        _currentTurnDuration = 1f;

        yield return new WaitForSeconds(_currentTurnDuration);

        //Hydra turn based
        _playerTurn = !_playerTurn;
        
        if(!_playerTurn) NextTurn();
    }
    #endregion

    #region Enum
    public enum SPELLSTATE
    {
        None,
        EAU,
        FEU,
        PLANTE,
        TETE,
        POTION
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
        ANALYSER,
        STUN
    }

    #endregion
}
