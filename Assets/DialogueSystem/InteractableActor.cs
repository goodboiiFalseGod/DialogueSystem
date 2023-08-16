using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractableActor : MonoBehaviour
{
    [SerializeField] private bool _rotatesToPlayer;
    [SerializeField] private Animator _actorAnimator;
    [SerializeField] private Camera _dialogueCamera;
    private Camera _previousCamera;

    private static int _rotationHash = Animator.StringToHash("Rotation");
    private static int _dialogueIdleHash = Animator.StringToHash("DialogueIdle");

    private Quaternion _initialRotation;

    private void Awake()
    {
        _dialogueCamera.gameObject.SetActive(false);
        _initialRotation = transform.rotation;
    }

    public void Speech()
    {
        _actorAnimator.SetBool(_dialogueIdleHash, true);
    }

    private void RotateToPlayer(Transform playerTransform)
    {
        // Calculate the angle between the two forward directions
        Vector3 targetPos = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 toPlayerDirection = targetPos - current;
        Vector3 rotation = Quaternion.LookRotation(toPlayerDirection).eulerAngles;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, toPlayerDirection, Vector3.up);

        _actorAnimator.SetFloat(_rotationHash, Mathf.Sign(angleToPlayer));

        transform.DORotate(rotation, 0.5f, RotateMode.Fast).OnComplete(() =>
        {
            _previousCamera = Camera.main;
            Camera.main.gameObject.SetActive(false);
            _dialogueCamera.gameObject.SetActive(true);
            Speech();
            _actorAnimator.SetFloat(_rotationHash, -1);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        RotateToPlayer(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.rotation = _initialRotation;
        _actorAnimator.SetBool(_dialogueIdleHash, false);

        _dialogueCamera.gameObject.SetActive(false);
        _previousCamera.gameObject.SetActive(true);
    }
}
