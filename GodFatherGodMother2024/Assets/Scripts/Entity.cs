using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _name;
    [SerializeField] private int _health;

    #endregion

    #region Properties

    public string Name => _name;
    
    public int Health => _health;
    
    public bool IsStunned { get; set; }

    #endregion
}