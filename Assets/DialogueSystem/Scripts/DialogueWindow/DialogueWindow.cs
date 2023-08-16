using DG.Tweening;
using IL3DN;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueData;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _npcNameText;
    [SerializeField] private Transform _questionsRoot;
    [SerializeField] private DialogueQuestion _dialogueQuestionPrefab;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private IL3DN_SimpleFPSController _player;
    [SerializeField] private Button _exitButton;

    private List<DialogueQuestion> _dialogueQuestionsButtons;
    private InteractableActor _currentActor;

    public static DialogueWindow Instance => _dialogueWindowInstance;
    private static DialogueWindow _dialogueWindowInstance;

    // Start is called before the first frame update

    private void Awake()
    {
        _dialogueWindowInstance = this;
        _dialogueQuestionsButtons = new List<DialogueQuestion>();

        for (int i = 0; i < 3; i++)
        {
            _dialogueQuestionsButtons.Add(Instantiate(_dialogueQuestionPrefab, _questionsRoot));
        }

        _exitButton.onClick.AddListener(() => Hide(0.2f));
        Hide(0f);
    }

    public void Hide(float time)
    {
        _rectTransform.DOScale(0, 0.3f).OnComplete(() => 
        {
            foreach (var answer in _dialogueQuestionsButtons)
            {
                answer.gameObject.SetActive(false);
            }

            _player.ToggleMovement(true);
            if (_currentActor == null) return;

            _currentActor.ExitDialogue();
        });
    }

    public void Show(DialogueData npcDialogueData, InteractableActor actor)
    {
        _currentActor = actor;

        _npcNameText.text = npcDialogueData.name;
        AssignQuestions(npcDialogueData.Answers);

        _rectTransform.DOScale(1, 0.3f);

        _player.ToggleMovement(false);
    }

    private void AssignQuestions(QuestionAnswerPair[] questionAnswerPairs)
    {
        if (questionAnswerPairs.Length == 0) return;

        if (questionAnswerPairs.Length > _dialogueQuestionsButtons.Count)
        {
            for (int i = 0; i < questionAnswerPairs.Length - _dialogueQuestionsButtons.Count; i++)
            {
                _dialogueQuestionsButtons.Add(Instantiate(_dialogueQuestionPrefab, _questionsRoot));
            }
        }

        for(int i = 0; i < questionAnswerPairs.Length; i++)
        {
            _dialogueQuestionsButtons[i].gameObject.SetActive(true);
            _dialogueQuestionsButtons[i].AssignQuestionAnswerPair(questionAnswerPairs[i]);
        }
    }
}
