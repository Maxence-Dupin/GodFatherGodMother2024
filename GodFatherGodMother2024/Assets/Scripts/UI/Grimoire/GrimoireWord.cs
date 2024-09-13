using System;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireWord : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _word;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _secondSprite;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _cooldownColor;

    #endregion

    #region Properties

    public string Word => _word;

    #endregion

    private void Update()
    {
        _image.color = GameManager.Instance.SpellsCooldown.ContainsKey(_word) ? _cooldownColor : _normalColor;
    }

    #region Public Methods

    public void UpdateImage()
    {
        _image.sprite = _secondSprite;
    }

    #endregion
}
