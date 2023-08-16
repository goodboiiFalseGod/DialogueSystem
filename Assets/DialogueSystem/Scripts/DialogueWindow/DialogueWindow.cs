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
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private IL3DN_SimpleFPSController _player;

    [SerializeField] private TextMeshProUGUI _npcNameText;

    [SerializeField] private TextMeshProUGUI _answerText;
    [SerializeField] private GameObject _questionsSection;

    [SerializeField] private Transform _questionsRoot;

    [SerializeField] private Button _exitButton;

    [SerializeField] private DialogueQuestionButton _dialogueQuestionPrefab;

    private List<DialogueQuestionButton> _dialogueQuestionsButtons;
    private InteractableActor _currentActor;

    public static DialogueWindow Instance => _dialogueWindowInstance;
    private static DialogueWindow _dialogueWindowInstance;

    // Start is called before the first frame update

    private void Awake()
    {
        Hide(0f);
        _dialogueWindowInstance = this;
        _dialogueQuestionsButtons = new List<DialogueQuestionButton>();

        for (int i = 0; i < 3; i++)
        {
            InstantiateQuestionButton();
        }

        _exitButton.onClick.AddListener(() => Hide(0.2f));
    }

    public void Hide(float time)
    {
        _rectTransform.DOScale(0, time).OnComplete(() => 
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
                InstantiateQuestionButton();
            }
        }

        for(int i = 0; i < questionAnswerPairs.Length; i++)
        {
            _dialogueQuestionsButtons[i].gameObject.SetActive(true);
            _dialogueQuestionsButtons[i].AssignQuestionAnswerPair(questionAnswerPairs[i]);
        }
    }

    public void QuestionClicked(QuestionAnswerPair questionAnswerPair)
    {
        StartCoroutine(ShowAnswer(questionAnswerPair.Answer));
    }

    public IEnumerator ShowAnswer(string[] answers)
    {
        _questionsSection.SetActive(false);
        _answerText.gameObject.SetActive(true);

        int i = 0;
        _answerText.text = answers[i];

        do
        {
            i++;
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space));

            if (i != answers.Length)
            {
                _answerText.text = answers[i];
            }

            yield return null;

        } while (i < answers.Length);

        _questionsSection.SetActive(true);
        _answerText.gameObject.SetActive(false);
    }

    private void InstantiateQuestionButton()
    {
        DialogueQuestionButton question = Instantiate(_dialogueQuestionPrefab, _questionsRoot);
        _dialogueQuestionsButtons.Add(question);
        question.Init(this);
    }
}
