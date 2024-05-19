using Oculus.Voice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AppVoiceExperience appVoiceExperience;
    [SerializeField] Text text;

    


    private bool appVoiceActive;

    void Awake()
    {
        appVoiceExperience.TranscriptionEvents.OnPartialTranscription.AddListener((transcrpt) => {
            text.text = transcrpt;
        });


        appVoiceExperience.TranscriptionEvents.OnFullTranscription.AddListener((transcrpt) => {
            text.text = transcrpt;
        });

        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(() => {
            appVoiceActive = false;
        });

        appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener((transcrpt) =>
        {
            appVoiceActive = true;
            text.text = "Listening";
        });

    }

    void Update()
    {
     


    }
}
