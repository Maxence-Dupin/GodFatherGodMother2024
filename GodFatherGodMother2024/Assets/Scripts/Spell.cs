using System;
using UnityEngine;

public enum ActionWords
{
    Throw,
    Defend,
    Heal
}

public enum ElementWords
{
    Fire,
    Water,
    Earth,
    Air,
    Potion
}

[CreateAssetMenu(fileName = "New Spell", menuName = "Custom/Spell")]
public class Spell : ScriptableObject
{
    public ActionWords action;
    public ElementWords element;

    public Action onEventTriggered;
}