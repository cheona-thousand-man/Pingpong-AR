using System;
using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // Game Session AGNOSTIC Settings
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private BallBehaviour _ball;

    // Local Runtime references
    private Rigidbody _rigidbody;
    private Camera _mainCamera;
    private NetworkButtons ButtonsPrevious {get; set;}
    private TickTimer _serveCooldown;
    private float _delayBetweenServes = 0.5f;

    public bool AcceptInput => Object.IsValid;

    private void Awake()
    {
        // We're controlling the ship using forces, so grab the rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    public override void FixedUpdateNetwork()
    {
        // Handle input if we have StateAuthority over the object (GetInput returns false otherwise)
        if (AcceptInput)
        {
            if (GetInput<PlayerInput>(out var input))
            {
                Move(input);
                SpawnBall(input);
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
    }

    private void SpawnBall(PlayerInput input)
    {
        if (input.Buttons.WasPressed(ButtonsPrevious, PlayerButtons.Fire))
        {
            if (_serveCooldown.ExpiredOrNotRunning(Runner) == false) return;

            Vector3 spawnPosition = _rigidbody.position + transform.up * 2;
            Quaternion rotation = Quaternion.identity;
            Runner.Spawn(_ball, spawnPosition, rotation, Runner.LocalPlayer);

            _serveCooldown = TickTimer.CreateFromSeconds(Runner, _delayBetweenServes);
        }
    }

    // 플레이어의 위치를 화면 경계 내로 제한하는 메서드
    private Vector3 ClampPositionToScreen(Vector3 position)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);

        // viewportPosition.x와 y 값을 0과 1 사이로 제한하여 화면 밖으로 나가지 않도록 합니다.
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.0f, 1.0f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.0f, 1.0f);

        // 제한된 viewportPosition을 다시 월드 좌표로 변환합니다.
        return _mainCamera.ViewportToWorldPoint(viewportPosition);
    }
}