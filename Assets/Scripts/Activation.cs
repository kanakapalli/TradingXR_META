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

    private bool _mic_activation = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
             // _dictation.ActivateImmediately();
             ActivateIMID();
        }
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
