using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;

/// <summary>
/// Struct holding the player data.
/// </summary>
public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public ulong clientId;
    public int characterIndex;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public bool Equals(PlayerData other)
    {
        return
        this.clientId == other.clientId &&
        this.characterIndex == other.characterIndex &&
        this.playerName == other.playerName &&
        this.playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
       serializer.SerializeValue(ref clientId);
       serializer.SerializeValue(ref characterIndex);
       serializer.SerializeValue(ref playerName);
       serializer.SerializeValue(ref playerId);
    }
}