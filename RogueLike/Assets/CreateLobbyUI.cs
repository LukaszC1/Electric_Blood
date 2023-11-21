using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _createPublicBtn;
    [SerializeField] private Button _createPrivateBtn;
    [SerializeField] private TMP_InputField _lobbyNameInputField;



    private void Awake()
    {
        _createPublicBtn.onClick.AddListener(() => {
            ElectricBloodLobby.Instance.CreateLobby(_lobbyNameInputField.text, false);
        });
        _createPrivateBtn.onClick.AddListener(() => {
            ElectricBloodLobby.Instance.CreateLobby(_lobbyNameInputField.text, true);
        });
        _closeBtn.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _createPublicBtn.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
