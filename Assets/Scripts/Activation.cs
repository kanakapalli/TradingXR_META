using Meta.WitAi.Dictation;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Activation : MonoBehaviour
{
    // [SerializeField] InputField _searchDictation;
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService _dictation;
    [SerializeField] private MultiRequestTranscription _transcription;
    [SerializeField] private Animator _animator;
    [SerializeField] private Text _response_text;
    [SerializeField] private Image _background;
    [SerializeField] private float _padding = 10f;

    private bool _mic_activation = false;

    private void Update()
    {
        DynamicBackground();
        if (Input.GetKey(KeyCode.Space))
        {
             // _dictation.ActivateImmediately();
             ActivateIMID();
        }
    }

    private void DynamicBackground()
    {
        int lineCount = _response_text.cachedTextGenerator.lines.Count;
        float lineHeight = _response_text.fontSize * _response_text.lineSpacing + _padding;
        float totalHeight = lineHeight * lineCount + _padding;

        RectTransform bgRect = _background.rectTransform;
        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, totalHeight);
    }

    public void ToggleActivation()
    {
        if (_mic_activation)
        {
            _dictation.Deactivate();
            _mic_activation = false;
        }
        else
        {
            _dictation.Activate();
            _mic_activation = true;
        }
    }

    public void ActivateIMID()
    {
        _dictation.ActivateImmediately();
        // StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        _dictation.ActivateImmediately();
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<API_Call>().OnEnterPress();
        yield return new WaitForSeconds(0.8f);
        _dictation.Deactivate();
        yield return new WaitForSeconds(0.4f);
        _transcription.Clear();
    }

    public void EndListening()
    {
        _dictation.Deactivate();
        FindObjectOfType<API_Call>().OnEnterPress();
        _transcription.Clear();
    }

    public void ToggleAnimation(bool value)
    {
        _animator.SetBool("Listening", value);
    }
}
