using System.Collections.Generic;
using UnityEngine;

public class Dragon : Entity
{
    #region Fields

    [SerializeField] private int _health;
    [Space(10)] [SerializeField] private List<DragonHead> _dragonHeads;

    #endregion

    #region Properties

    public int Health => _health;

    public List<DragonHead> DragonHeads => _dragonHeads;

    #endregion

    #region Class

    [System.Serializable]
    public class DragonHead
    {
        [SerializeField] private GameManager.SPELLSTATE _element;
        [SerializeField] private GameObject _spriteGameObject;
        [SerializeField] private int _associatedKeyNumber;
    }

    #endregion
}
