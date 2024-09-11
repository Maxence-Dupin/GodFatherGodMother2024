using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _name;

    #endregion

    #region Properties

    public string Name => _name;
    
    public bool IsStunned { get; set; }

    #endregion
}