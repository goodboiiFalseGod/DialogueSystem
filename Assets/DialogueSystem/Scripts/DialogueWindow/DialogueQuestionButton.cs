using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueQuestionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private Button _button;

    private DialogueWindow _dialogueWindow;
    private QuestionAnswerPair _currentPair;

    public void Init(DialogueWindow dialogueWindow)
    {
        _dialogueWindow = dialogueWindow;
        _button.onClick.AddListener(OnClick);
    }

    public void AssignQuestionAnswerPair(QuestionAnswerPair questionAnswer)
    {
        _currentPair = questionAnswer;
        _questionText.text = _currentPair.Question;
    }

    private void OnClick()
    {
        _dialogueWindow.QuestionClicked(_currentPair);
    }
}
