using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialWaifu : MonoBehaviour
{
    [SerializeField]
    private bool _hasContinueButton;
    [SerializeField]
    private TapObject _nextButton;
    [SerializeField]
    private ActionAfterTutorial _action;
    [SerializeField]
    private SpriteRenderer _waifuSprite;
    [SerializeField]
    private Transform _textBox;
    [SerializeField]
    private string _text;
    [SerializeField]
    private TMP_Text _textField;

    private int _textCount = 0;
    private Coroutine _textCoroutine;
    private const float TEXT_DELAY = 0.05f;
    public event Action OnContinueTap;
    public event Action<ActionAfterTutorial> OnHidden;

    private void Start()
    {
        _nextButton.OnClick += () => OnContinueTap?.Invoke();
        _nextButton.gameObject.SetActive(false);
        _textBox.localScale = Vector3.zero;
        _waifuSprite.color = new Color(1, 1, 1, 0);
    }

    public void Initialize()
    {
        _waifuSprite.DOFade(1, 0.5f).OnComplete(() =>
        {
            _textBox.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                _textCoroutine = StartCoroutine(TextCoroutine());
            });
        });
    }

    public void Hide()
    {
        if (_textCoroutine != null)
        {
            StopCoroutine(_textCoroutine);
        }
        _textBox.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad);
        _waifuSprite.DOFade(0, 0.5f).OnComplete(() =>
        {
            OnHidden?.Invoke(_action);
        });
    }

    private IEnumerator TextCoroutine()
    {
        yield return new WaitForSecondsRealtime(TEXT_DELAY);
        if (_textCount >= _text.Length)
        {
            if (_hasContinueButton)
                _nextButton.gameObject.SetActive(true);
        }
        else
        {
            string character = _text[_textCount].ToString();
            if (character.Equals("\\"))
            {
                character += _text[_textCount + 1].ToString();
                _textCount++;
            }
            if (character.Equals("<"))
            {
                var count = _textCount + 1;
                var st = _text[count].ToString();
                while (!st.Equals(">"))
                {
                    character += st;
                    count++;
                    _textCount++;
                    st = _text[count].ToString();
                }
                character += st;
                _textCount++;
            }
            _textField.text += character;
            _textCount++;
            _textCoroutine = StartCoroutine(TextCoroutine());
        }
    }
}
