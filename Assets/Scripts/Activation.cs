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
    [SerializeField] private float _padding = 30f;
    [SerializeField] private Canvas _canvas; // Add reference to the canvas

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
        // Get the width and height required by the text
        Vector2 textSize = GetTextSize(_response_text);
        RectTransform bgRect = _background.rectTransform;

        // Get the width and height of the canvas
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Calculate the new width and height with padding
        float newWidth = textSize.x + (_padding / 2);
        float newHeight = textSize.y + (_padding / 2);

        // Ensure the new width does not exceed the canvas width
        if (newWidth > canvasWidth)
        {
            newWidth = canvasWidth - _padding; // Adjust to fit within the canvas width
        }

        // Ensure the new height does not exceed the canvas height
        if (newHeight > canvasHeight)
        {
            newHeight = canvasHeight - _padding; // Adjust to fit within the canvas height
        }

        // Set the background size
        bgRect.sizeDelta = new Vector2(newWidth, newHeight);
    }

    private Vector2 GetTextSize(Text text)
    {
        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
        float width = textGen.GetPreferredWidth(text.text, generationSettings) / text.pixelsPerUnit;
        float height = textGen.GetPreferredHeight(text.text, generationSettings) / text.pixelsPerUnit;
        return new Vector2(width, height);
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
