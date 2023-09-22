using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int xpGemAmount;
    [HideInInspector] public float xpBank;

    public Transform playerTransform;
    private float timer = 1;

    [HideInInspector] public int killCount = 0;
    [SerializeField] TMPro.TextMeshProUGUI killCounter;

    [SerializeField] GameObject xpBankGemPrefab;
    GameObject xpBankGem;

    [SerializeField] GameObject breakableObject;

    private void Awake()
    {
        Instance = this;
        xpBankGem = Instantiate(xpBankGemPrefab);
        xpBankGem.SetActive(false);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (UnityEngine.Random.value <= 0.1)
            {
                GameObject breakable = Instantiate(breakableObject);
                Vector3 position = GenerateRandomPosition();
                while(CheckForCollision(position))
                    position = GenerateRandomPosition();
                breakable.transform.position = position;
            }
            timer = 1;
        }


        if (xpBank > 100 && !xpBankGem.activeSelf)
        {
            xpBankGem.SetActive(true);
            Vector3 position = GenerateRandomPosition();
            while (CheckForCollision(position))
                position = GenerateRandomPosition();
            xpBankGem.transform.position = position;   
        }
    }

    public Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3();

        float f = UnityEngine.Random.value > 0.5f ? -1f : 1f;

        if (UnityEngine.Random.value > 0.5f)
        {
            position.x = UnityEngine.Random.Range(-17, 8);
            position.y = 8 * f;
        }
        else
        {
            position.y = UnityEngine.Random.Range(-17, 8);
            position.x = 17 * f;
        }
        position.x += playerTransform.position.x;
        position.y += playerTransform.position.y;
        position.z = 0;

        return position;
    }

    private bool CheckForCollision(Vector3 position)
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collision in collisions)
        {
            if (collision.transform.name == "buildings")
            {
                return true;
            }
        }
        return false;
    }

    public void IncrementKillCount()
    {
        killCount++;
        killCounter.text = ":" + killCount.ToString();
    }
}
