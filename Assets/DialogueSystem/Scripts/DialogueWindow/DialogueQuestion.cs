using TMPro;
using UnityEngine;
using static DialogueData;

public class DialogueQuestion : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questionText;
    private QuestionAnswerPair _currentPair;

    public void AssignQuestionAnswerPair(QuestionAnswerPair questionAnswer)
    {
        _currentPair = questionAnswer;
        _questionText.text = _currentPair.Question;
    }
}
