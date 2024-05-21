using Meta.WitAi.Dictation;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Activation : MonoBehaviour
{
    [FormerlySerializedAs("dictation")]
    [SerializeField] private DictationService _dictation;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _dictation.ActivateImmediately();
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
        yield return new WaitForSeconds(2f);
        FindObjectOfType<API_Call>().OnEnterPress();
    }
}
