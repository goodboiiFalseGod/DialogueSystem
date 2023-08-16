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
    private static DialogueWindow _dialogueWindowInstance;

    public static DialogueWindow Instance => _dialogueWindowInstance;

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

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        Hide(0.2f);
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
            _questionsSection.SetActive(true);
            _answerText.gameObject.SetActive(false);
        });
    }

    public void Show(DialogueData npcDialogueData, InteractableActor actor)
    {
        _currentActor = actor;

        _npcNameText.text = npcDialogueData.name;
        AssignQuestions(npcDialogueData.Answers);

        _rectTransform.DOScale(1, 0.3f);

        _player.ToggleMovement(false);

        if (actor.FirstSpeechDone) return;

        actor.FirstSpeechDone = true;
        StartCoroutine(ShowAnswer(npcDialogueData.FirstPhrase));
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
            _dialogueQuestionsButtons[i].AssignQuestionAnswerPair(questionAnswerPairs[i], i + 1);
        }
    }

    public void QuestionClicked(QuestionAnswerPair questionAnswerPair)
    {
        StartCoroutine(ShowAnswer(questionAnswerPair.Answer));
    }

    public IEnumerator ShowAnswer(TextAudioPair[] answers)
    {
        _questionsSection.SetActive(false);
        _answerText.gameObject.SetActive(true);

        int i = 0;
        _answerText.text = answers[i].TextLine;
        _currentActor.AudioSource.clip = answers[i].VoiceLine;
        _currentActor.AudioSource.Play();

        do
        {
            i++;
            yield return new WaitUntil(() => (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)));

            if (i != answers.Length)
            {
                _currentActor.AudioSource.clip = answers[i].VoiceLine;
                _currentActor.AudioSource.Play();
                _answerText.text = answers[i].TextLine;
            }

            yield return null;

        } while (i < answers.Length);

        _questionsSection.SetActive(true);
        _answerText.gameObject.SetActive(false);
    }

    public IEnumerator ShowAnswer(TextAudioPair answer)
    {
        _questionsSection.SetActive(false);
        _answerText.gameObject.SetActive(true);

        _answerText.text = answer.TextLine;
        _currentActor.AudioSource.clip = answer.VoiceLine;
        _currentActor.AudioSource.Play();

        while(true)
        {
            yield return new WaitUntil(() => (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)));
            break;
        }

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
