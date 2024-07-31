using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class NetworkUtilities
{

    // PlayerRef로 NetworkObject를 찾는 메서드
    public static NetworkObject GetNetworkObjectFromPlayerRef(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner is null.");
            return null;
        }

        return runner.GetPlayerObject(playerRef);
    }

    // NetworkBehaviourId에서 소유자인 PlayerRef를 얻는 메서드
    public static PlayerRef GetPlayerRefFromNetworkBehaviourId(NetworkRunner runner, NetworkBehaviourId networkBehaviourId)
    {
        // NetworkBehaviourId로 NetworkObject를 찾음
        if (runner.TryFindBehaviour(networkBehaviourId, out NetworkBehaviour networkBehaviour))
        {
            // NetworkObject를 얻음
            NetworkObject networkObject = networkBehaviour.Object;

            // NetworkObject에서 PlayerRef를 얻음
            if (networkObject != null)
            {
                return networkObject.InputAuthority;
            }
            else
            {
                Debug.LogError("NetworkObject를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("NetworkBehaviour를 찾을 수 없습니다.");
        }

        // 기본값 반환
        return PlayerRef.None;
    }

    // PlayerRef에서 PlayerObject에 삽입된 NetworkBehaviour 컴포넌트를 타입으로 찾는 메서드
    // 호출 시 원하는 클래스명으로 타입 지정 필요 (예: MyNetworkBehaviour / System.Type targetType = typeof(MyNetworkBehaviour)
    public static bool GetComponentFromPlayerRef(NetworkRunner runner, PlayerRef playerRef, System.Type targetType, out NetworkBehaviour targetComponent)
    {
        targetComponent = null; // 기본값 설정
        
        // PlayerObject를 가져옵니다
        var networkObject = runner.GetPlayerObject(playerRef);
        if (networkObject == null)
        {
            Debug.LogError("PlayerObject not found for PlayerRef.");
            return false;
        }

        // NetworkObject에서 해당 타입의 NetworkBehaviour를 찾습니다
        targetComponent = networkObject.GetComponent(targetType) as NetworkBehaviour;
        if (targetComponent == null)
        {
            Debug.LogError($"Component of type {targetType} not found on PlayerObject.");
            return false;
        }

        return true; // 컴포넌트를 성공적으로 찾았을 경우
    }

    // NetworkBehaviourId를 사용하여 NetworkBehaviour를 찾는 메서드
    // Runner.TryFindBehaviour< T > (NetworkBehaviourId id, out T behaviour)
}
