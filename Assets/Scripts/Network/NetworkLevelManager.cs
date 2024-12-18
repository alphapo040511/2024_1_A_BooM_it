using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkLevelManager : NetworkBehaviour
{
    public List<NetworkBlock> DisabledBlocks = new List<NetworkBlock>();
    public NetworkLevelGenerator levelGenerator;
    public BattleManager battleManager;

    public override void FixedUpdateNetwork()
    {
        for(int i = 0; i < DisabledBlocks.Count; i++)
        {
            DisabledBlocks[i].Timer(Runner.DeltaTime);
        }
    }

    public void DestroyBlocks(Vector3Int hitPosition, Vector3Int[] positions)
    {
        if (Runner.IsServer)
        {
            RPC_RelayMessage_Destroy(hitPosition, positions);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Destroy(Vector3Int hitPosition, Vector3Int[] positions)
    {
        foreach(Vector3Int intPosition in positions)
        {
            for (int y = intPosition.y; y < levelGenerator.mapData.BlockArr.GetLength(1); y++)
            {
                DestroyBlock(hitPosition + intPosition + Vector3Int.up * y);
            }
        }
    }

    private void DestroyBlock(Vector3Int targetPos)
    {
        if (levelGenerator.blockDictionary.ContainsKey(targetPos))
        {
            levelGenerator.blockDictionary[targetPos].DestroyBlock(5);
            levelGenerator.UpdateSurfaceBlocksAround(targetPos);
        }
    }

    public void RegenerationBlocks(Vector3Int position)
    {
        if (Runner.IsServer)
        {
            RPC_RelayMessage_Regeneration(position);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Regeneration(Vector3Int position)
    {
        if (levelGenerator.blockDictionary.ContainsKey(position))
        {
            levelGenerator.blockDictionary[position].Respawn();
        }
    }
}
