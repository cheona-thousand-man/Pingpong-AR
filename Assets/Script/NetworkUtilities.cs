using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class NetworkUtilities
{
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

}
