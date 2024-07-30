using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


[RequireComponent(typeof(GameController))]
public class BallSpawner : NetworkBehaviour
{
    [SerializeField] private BallBehaviour _ballPrefab; // 생성할 공의 프리팹
    private Camera mainCamera; // 화면의 터치 또는 클릭 위치를 변환할 카메라

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnBall(PlayerRef servePlayer)
    {
        // 현재 플레이어가 공을 생성할 권한이 있는지 확인
        if (Runner.LocalPlayer != servePlayer) return;
        Debug.Log("공 생성 권한 있음");

        StartCoroutine(WaitForTouchAndSpawnBall(servePlayer));
    }

    private IEnumerator WaitForTouchAndSpawnBall(PlayerRef servePlayer)
    {
        Debug.Log("공 생성 터치 입력 대기");
        // 터치 입력을 기다림
        while (!Input.GetMouseButtonDown(0))
        {
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("공 생성 터치 입력 받음 생성 대기");
        // 화면 좌표를 월드 좌표로 변환
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = 1f; // 카메라와의 거리 (z축 위치)
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // 공의 회전값 설정 (기본적으로 회전이 없도록 설정)
        Quaternion rotation = Quaternion.identity;

        // 공 생성
        BallBehaviour ball = Runner.Spawn(_ballPrefab, worldPosition, rotation, PlayerRef.None);
    }
}

