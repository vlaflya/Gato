using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private List<TutorialWaifu> _hints;

    private TutorialWaifu _currentHint;
    private int _hintCount;
    private bool _active;

    public event Action ReadyLootBox;

    public void StartTutorial()
    {
        _active = true;
        _hintCount = -1;
        ContinueTutorial();
    }

    public void ContinueTutorial()
    {
        if (!_active)
            return;
        _hintCount++;
        if (_currentHint != null)
        {
            _currentHint.OnHidden += (action) =>
            {
                switch (action)
                {
                    case ActionAfterTutorial.ReadyLootBox:
                        {
                            ReadyLootBox?.Invoke();
                            break;
                        }
                }
            };
            _currentHint.Hide();
        }
        if (_hintCount == _hints.Count)
        {
            _active = false;
            return;
        }
        _currentHint = _hints[_hintCount];
        _currentHint.Initialize();
        _currentHint.OnContinueTap += ContinueTutorial;
    }
}

public enum ActionAfterTutorial
{
    None,
    ReadyLootBox
}
