using System.Collections.Generic;
using UnityEngine;

public class Dragon : Entity
{
    #region Fields
    [Space(10)] [SerializeField] private List<DragonHead> _dragonHeads;
    #endregion

    #region Properties
    public List<DragonHead> DragonHeads => _dragonHeads;
    #endregion

    #region Class

    [System.Serializable]
    public class DragonHead
    {
        [SerializeField] private int _maxHealthPerHead;
        [SerializeField] private int _healthPerHead;
        [SerializeField] private GameManager.SPELLSTATE _element;
        [SerializeField] private GameObject _spriteGameObject;
        [SerializeField] private int _associatedKeyNumber;


        public GameManager.SPELLSTATE Element => _element;

        public int MaxHealthPerHead { get => _maxHealthPerHead; set => _maxHealthPerHead = value; }
        public int HealthPerHead { get => _healthPerHead; set => _healthPerHead = value; }
        public int AssociatedKeyNumber { get => _associatedKeyNumber; set => _associatedKeyNumber = value; }

        public void Init()
        {
            HealthPerHead = MaxHealthPerHead;
        }
    }
    #endregion
}
