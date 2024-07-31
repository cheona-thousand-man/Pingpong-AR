using System;
using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    // Game Session AGNOSTIC Settings
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private BallBehaviour _ball;
    [SerializeField] private float _rotationSpeed = 10.0f;

    // Local Runtime references
    private Rigidbody _rigidbody;
    private Camera _mainCamera;
    private GameController _gameController;
    private NetworkButtons ButtonsPrevious {get; set;}
    private TickTimer _serveCooldown;
    private float _delayBetweenServes = 0.5f;

    public bool AcceptInput => Object.IsValid;

    private void Awake()
    {
        // We're controlling the ship using forces, so grab the rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _gameController = FindObjectOfType<GameController>();
    }

    public override void FixedUpdateNetwork()
    {
        // Handle input if we have StateAuthority over the object (GetInput returns false otherwise)
        if (AcceptInput)
        {
            if (GetInput<PlayerInput>(out var input))
            {
                // 이동 로직
                Move(input); // 3D 게임 구현 시, 키보드 이동
                // MoveByCamera(); // AR게임 구현 시, 카메라가 탑재된 디바이스 이동에 따른 이동 구현

                // gameController에서 서브 가능 & 서브인 플레이어 차례인 플레이어만 공 생성 가능
                if (_gameController.CanServiceBall && (_gameController.ServePlayerId.Equals(Runner.LocalPlayer)))
                {
                    SpawnBall(input);
                }
            }
        }
        else
        {
            Debug.Log("AcceptInput is false");
        }
    }

    private void Move(PlayerInput input)
    {
        // 현재 오브젝트의 Transform을 가져옵니다.
        Transform xform = transform;

        // 플레이어의 좌우 입력(HorizontalInput)에 따라 이동 속도를 계산합니다.
        // Mathf.Clamp를 사용하여 입력 값을 -1과 1 사이로 제한합니다.
        float horizontalInput = Mathf.Clamp(input.HorizontalInput, -1, 1);

        // 이동 속도를 계산합니다.
        Vector3 movement = xform.right * horizontalInput * _moveSpeed * Runner.DeltaTime;

        // Rigidbody의 위치를 직접 변경하여 좌우로 이동합니다.
        Vector3 newPosition = _rigidbody.position + movement;

        // 화면 경계를 계산하고 플레이어 위치 제한
        newPosition = ClampPositionToScreen(newPosition);

        _rigidbody.MovePosition(newPosition);

        // 회전 각도 계산 및 적용
        float rotationAngle = horizontalInput * -45f; // 최대 45도 회전
        Quaternion targetRotation = Quaternion.Euler(0, rotationAngle, 0);

        // 현재 회전
        Quaternion currentRotation = _rigidbody.rotation;

        // 부드러운 회전을 위해 Slerp 사용
        Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * _rotationSpeed);

        _rigidbody.MoveRotation(newRotation);
    }

    private void MoveByCamera()
    {
        throw new NotImplementedException();
    }

    private void SpawnBall(PlayerInput input)
    {
        if (input.Buttons.WasPressed(ButtonsPrevious, PlayerButtons.Fire))
        {
            if (_serveCooldown.ExpiredOrNotRunning(Runner) == false) return;

            Vector3 spawnPosition = _rigidbody.position + transform.up * 2;
            Quaternion rotation = Quaternion.identity;
            BallBehaviour ball = Runner.Spawn(_ball, spawnPosition, rotation, Runner.LocalPlayer);
            ball.ServeBall(transform.forward * 20);

            // 공을 던진 후: 서브 횟수 처리 & 공이 사라지기 전까지는 서브 불가능하게 처리
            _gameController.CanServiceBall = false;

            _serveCooldown = TickTimer.CreateFromSeconds(Runner, _delayBetweenServes);
        }
    }

    private Vector3 ClampPositionToScreen(Vector3 position)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);

        // viewportPosition을 약간 더 확장하여 화면 경계를 벗어나지 않도록 clamp합니다.
        float margin = -0.4f;
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, -margin, 1.0f + margin);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, -margin, 1.0f + margin);

        // 제한된 viewportPosition을 다시 월드 좌표로 변환합니다.
        return _mainCamera.ViewportToWorldPoint(viewportPosition);
    }
}