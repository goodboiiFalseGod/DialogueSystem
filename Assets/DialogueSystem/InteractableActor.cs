using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableActor : MonoBehaviour
{
    [SerializeField] private bool _rotatesToPlayer;
    [SerializeField] private Animator _actorAnimator;

    private static int _rotationHash = Animator.StringToHash("Rotation");
    private static int _dialogueIdleHash = Animator.StringToHash("DialogueIdle");

    private Quaternion _initialRotation;

    private void Start()
    {
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
        float angleToPlayer = Vector3.SignedAngle(transform.forward, toPlayerDirection, Vector3.up);

        Debug.Log(angleToPlayer.ToString());

        StartCoroutine(AnimateRotation(Mathf.Sign(angleToPlayer), angleToPlayer));
    }

    private IEnumerator AnimateRotation(float direction, float angle)
    {
        _actorAnimator.SetFloat(_rotationHash, direction);

        while(angle != 0)
        {
            transform.Rotate(Vector3.up, direction * Mathf.Min(150 * Time.deltaTime, Mathf.Abs(angle)));
            angle -= direction * Mathf.Min(150 * Time.deltaTime, Mathf.Abs(angle));

            yield return null;
        }

        _actorAnimator.SetFloat(_rotationHash, -1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        Speech();
        RotateToPlayer(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.rotation = _initialRotation;
        _actorAnimator.SetBool(_dialogueIdleHash, false);
    }
}
