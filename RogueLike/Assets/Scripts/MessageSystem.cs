using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageSystem : MonoBehaviour
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
        messagePool = new List<TextMeshPro>();

            for(int i =0;i < objectCount; i++)
        {
            fillList();
        }
    }

    [SerializeField] GameObject damagePopup;
    List<TMPro.TextMeshPro> messagePool;

    public void PostMessage (int damage, Vector3 position)
    {
        string message = damage.ToString();
        messagePool[count].gameObject.SetActive (true);
        messagePool[count].transform.position = position;
        messagePool[count].text = message;
        messagePool[count].alpha = 1.0f;
        messagePool[count].transform.localScale = new Vector3(0.55f,0.55f,0.55f);

        if (damage < 20)
        messagePool[count].color = new Color(1,1,1);
        else if(damage >= 20 && damage < 50)
        messagePool[count].color = new Color(1, 1, 0);
        else if (damage >= 50 && damage < 100)
        messagePool[count].color = new Color(1, 0, 0);
        else
        messagePool[count].color = new Color(0.043f, 0.85f, 0.85f);

        count += 1;

        if(count >= objectCount)
        {
            count = 0;
        }
    }

    public void fillList()
    {
        GameObject go = Instantiate(damagePopup, transform);
        messagePool.Add(go.GetComponent<TMPro.TextMeshPro>());
        go.SetActive(false);
    }
}
