using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue Data/Create new dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField] public string NPCName;
    [SerializeField] public string AlternativeNPCName;

    [SerializeField] public QuestionAnswerPair[] Answers;

    [Serializable]
    public class QuestionAnswerPair
    {
        [SerializeField] public bool OneTimeQuestion = false;
        [SerializeField] public string Question;
        [SerializeField] public TextAudioPair[] Answer;
    }

    [Serializable]
    public class TextAudioPair
    {
        [SerializeField] public string TextLine;
        [SerializeField] public AudioClip VoiceLine;
    }
}
