using System;
using UnityEngine;

public enum ActionWords
{
    BOIRE,
    LANCER,
    CHARGER,
    ANALYSER,
    DEFENDRE,
}

public enum ElementWords
{
    FEU,
    EAU,
    PLANTE,
    TETE,
    POTION
}

[CreateAssetMenu(fileName = "New Spell", menuName = "Custom/Spell")]
public class Spell : ScriptableObject
{
    #region Fields

    [SerializeField] private ActionWords _action;
    [SerializeField] private ElementWords _element;

    [SerializeField, TextArea] private string _message;

    public Action onEventTriggered;

    #endregion

    #region Properties

    public ActionWords Action => _action;
    public ElementWords Element => _element;
    public string Message => _message;

    #endregion
}