using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;

/// <summary>
/// Struct holding the player data.
/// </summary>
public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    //Public fields
    public ulong clientId;
    public int characterIndex;
    public int selectedLevel;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    /// <summary>
    /// Implementation of Equals method.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(PlayerData other)
    {
        return
        this.clientId == other.clientId &&
        this.characterIndex == other.characterIndex &&
        this.selectedLevel == other.selectedLevel &&
        this.playerName == other.playerName &&
        this.playerId == other.playerId;
    }

    /// <summary>
    /// Implementation of the INetworkSerializable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
       serializer.SerializeValue(ref clientId);
       serializer.SerializeValue(ref characterIndex);
       serializer.SerializeValue(ref playerName);
       serializer.SerializeValue(ref playerId);
       serializer.SerializeValue(ref selectedLevel);
    }
}