using UnityEngine;
using DG.Tweening;
using IL3DN;

public class InteractableActor : MonoBehaviour
{
    [SerializeField] private Animator _actorAnimator;
    [SerializeField] private Camera _dialogueCamera;
    [SerializeField] private DialogueData _dialogueData;
    [SerializeField] private AudioSource _audioSource;
    private Camera _previousCamera;

    private static int _rotationHash = Animator.StringToHash("Rotation");
    private static int _dialogueIdleHash = Animator.StringToHash("DialogueIdle");
    private static int _mirrorRotation = Animator.StringToHash("TurnLeft");


    private Quaternion _initialRotation;
    private bool _speechCooldown = false;

    public AudioSource AudioSource => _audioSource;

    private void Awake()
    {
        _dialogueCamera.gameObject.SetActive(false);
        _initialRotation = transform.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player" || _speechCooldown) return;

        RotateToPlayer(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        ExitDialogue();
        _speechCooldown = false;
    }

    public void Speech()
    {
        _actorAnimator.SetBool(_dialogueIdleHash, true);
        DialogueWindow.Instance.Show(_dialogueData, this);
    }

    public void ExitDialogue()
    {
        if (_speechCooldown)
        {
            return;
        }

        _speechCooldown = true;

        _actorAnimator.SetBool(_dialogueIdleHash, false);

        _actorAnimator.SetBool(_rotationHash, !_actorAnimator.GetBool(_mirrorRotation));
        transform.DORotate(_initialRotation.eulerAngles, 0.5f, RotateMode.Fast).OnComplete(() => _actorAnimator.SetBool(_rotationHash, false));

        _dialogueCamera.gameObject.SetActive(false);
        _previousCamera.gameObject.SetActive(true);
    }

    private void RotateToPlayer(Transform playerTransform)
    {
        // Calculate the angle between the two forward directions
        Vector3 targetPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 toPlayerDirection = targetPos - current;
        Vector3 rotation = Quaternion.LookRotation(toPlayerDirection).eulerAngles;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, toPlayerDirection, Vector3.up);

        if(angleToPlayer > 0)
        {
            _actorAnimator.SetBool(_mirrorRotation, true);
        }
        else
        {
            _actorAnimator.SetBool(_mirrorRotation, false);
        }

        _actorAnimator.SetBool(_rotationHash, true);

        transform.DORotate(rotation, 0.5f, RotateMode.Fast).OnComplete(() =>
        {
            _previousCamera = Camera.main;
            Camera.main.gameObject.SetActive(false);
            _dialogueCamera.gameObject.SetActive(true);
            Speech();
            _actorAnimator.SetBool(_rotationHash, false);
        });

    }
}
