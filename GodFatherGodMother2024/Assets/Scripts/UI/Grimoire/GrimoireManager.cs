using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image _darkPanel;

    private bool _grimoireOpened;

    #endregion

    #region Public Methods

    private void ToggleGrimoire()
    {
        _grimoireOpened = !_grimoireOpened;
        
        if (_grimoireOpened)
        {
            _darkPanel.DOFade(0.5f, 1f);
        }
        else
        {
            _darkPanel.DOFade(0f, 1f);
        }
    }

    #endregion
}
