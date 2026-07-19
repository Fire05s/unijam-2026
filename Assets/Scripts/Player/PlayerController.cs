using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;

    [Header("Component")]
    [SerializeField] private Transform _orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Rigidbody _rb;

    [Header("Walk Sound")]
    [SerializeField] AudioClip[] _walkSounds;
    [SerializeField] float _walkSoundDelay = 0.6f;
    [SerializeField] Vector2 _walkSoundPitchRange;
    private float _walkSoundTimer;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
        if (_rb.linearVelocity != Vector3.zero)
        {
            PlayWalkSound();
        }
    }

    private void GetInput()
    {
        _horizontalInput = InputManager.Instance.Move.x;
        _verticalInput = InputManager.Instance.Move.y;
    }

    private void MovePlayer()
    {
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void PlayWalkSound()
    {
        _walkSoundTimer += Time.deltaTime;
        AudioSource soundSource = GetComponent<AudioSource>();
        if (_walkSoundTimer > _walkSoundDelay)
        {
            soundSource.resource = _walkSounds[Random.Range(0, _walkSounds.Length)];
            soundSource.pitch = Random.Range(_walkSoundPitchRange.x, _walkSoundPitchRange.y);
            soundSource.Play();
            _walkSoundTimer = 0f;
        }
    }

    public void ChangeSpeed(int newSpeed)
    {
        _moveSpeed = newSpeed;
    }
}
