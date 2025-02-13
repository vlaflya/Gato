using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _backSprite;

    [SerializeField]
    private List<TMP_Text> _hints;

    private TMP_Text _currentHint;
    private int _hintCount;
    private bool _active;

    public void StartTutorial()
    {
        _active = true;
        _hintCount = -1;
        _backSprite.DOFade(0.3f, 0.5f);
        ContinueTutorial();
    }

    public void ContinueTutorial()
    {
        if (!_active)
            return;
        _hintCount++;
        if (_currentHint != null)
        {
            _currentHint.DOFade(0, 0.5f);
        }
        if (_hintCount == _hints.Count)
        {
            _active = false;
            _backSprite.DOFade(0, 0.5f);
            return;
        }
        _currentHint = _hints[_hintCount];
        _currentHint.DOFade(1, 0.5f);
        if (_hintCount == _hints.Count - 1)
        {
            DOVirtual.DelayedCall(4f, () =>
            {
                ContinueTutorial();
            });
        }
    }
}
