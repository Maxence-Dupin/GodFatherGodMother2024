using UnityEngine;
using UnityEngine.UI;

public class GrimoireWord : MonoBehaviour
{
    #region Fields

    [SerializeField] private string _word;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _secondSprite;

    #endregion

    #region Properties

    public string Word => _word;

    #endregion

    #region Public Methods

    public void UpdateImage()
    {
        _image.sprite = _secondSprite;
    }

    #endregion
}
