using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class MessageSystem : NetworkBehaviour
{
    public static MessageSystem instance;

    private void Awake()
    {
        instance = this; 
    }

    int objectCount = 100;
    int count = 0;

    private void Start()
    {
        if (!IsOwner) return;
        messagePool = new List<NetworkObject>();

        for(int i = 0; i < objectCount; i++)
        {
            fillListServerRpc();
        }
    }

    [SerializeField] GameObject damagePopup;
    List<NetworkObject> messagePool;

    public void PostMessage (int damage, Vector3 position)
    {
        if (!IsOwner) return;
        PostMessageClientRpc(messagePool[count], damage, position);
        count += 1;

        if (count >= objectCount)
        {
            count = 0;
        }
    }
    [ClientRpc]
    private void PostMessageClientRpc(NetworkObjectReference messageReference, int damage, Vector3 position)
    {
        messageReference.TryGet(out NetworkObject messageObject);
        TextMeshPro message = messageObject.GetComponentInChildren<TextMeshPro>();

        string text = damage.ToString();
        message.transform.parent.gameObject.SetActive(true);
        message.transform.parent.transform.position = position;
        message.text = text;
        message.alpha = 1.0f;
        message.transform.parent.localScale = new Vector3(0.55f, 0.55f, 0.55f);

        if (damage < 20)
            message.color = new Color(1, 1, 1);
        else if (damage >= 20 && damage < 50)
            message.color = new Color(1, 1, 0);
        else if (damage >= 50 && damage < 100)
            message.color = new Color(1, 0, 0);
        else
            message.color = new Color(0.043f, 0.85f, 0.85f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void fillListServerRpc()
    {
        GameObject go = Instantiate(damagePopup);
        messagePool.Add(go.GetComponent<NetworkObject>());
        go.SetActive(false);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
