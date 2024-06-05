using Meta.WitAi.Dictation;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Activation : MonoBehaviour
{
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService _dictation;
    [SerializeField] private MultiRequestTranscription _transcription;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // _dictation.ActivateImmediately();
            // ActivateIMID();
        }
    }

    public void ToggleActivation()
    {
        if (_dictation.MicActive)
        {
            _dictation.Deactivate();
        }
        else
        {
            _dictation.Activate();
        }
    }

    public void ActivateIMID()
    {
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        _dictation.ActivateImmediately();
        yield return new WaitForSeconds(.8f);
        FindObjectOfType<API_Call>().OnEnterPress();
        yield return new WaitForSeconds(0.4f);
        _transcription.Clear();
    }
}
