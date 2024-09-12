using System.Collections.Generic;
using UnityEngine;

public enum Element
{
    Fire,
    Plant,
    Water
}

public class Dragon : Entity
{
    #region Fields

    [SerializeField] private int _health;
    [Space(10)] [SerializeField] private List<DragonHead> _dragonHeads;

    #endregion

    #region Properties

    public int Health => _health;

    #endregion

    #region Class

    [System.Serializable]
    public class DragonHead
    {
        [SerializeField] private Element _element;
        [SerializeField] private GameObject _spriteGameObject;
        [SerializeField] private int _associatedKeyNumber;
    }

    #endregion
}
