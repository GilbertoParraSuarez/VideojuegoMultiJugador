using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;
    public AudioSource buttonClick;
    public AudioSource keyClick;

    private void Start()
    {
        usernameInput.characterLimit = 11;
        usernameInput.onValueChanged.AddListener(OnInputValueChanged);
    }

    public void OnClickConnect()
    {
        buttonClick.Play();
        if (usernameInput.text.Length >= 1 && usernameInput.text.Length <= 11)
        {
            StartCoroutine(SendUsernameToServer(usernameInput.text));
        }
    }

    IEnumerator SendUsernameToServer(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/bdd/SaveUsername.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                buttonText.text = "Error: " + www.error;
            }
            else
            {
                string response = www.downloadHandler.text;
                if (response == "success")
                {
                    PhotonNetwork.NickName = username;
                    buttonText.text = "CARGANDO...";
                    PhotonNetwork.AutomaticallySyncScene = true;
                    PhotonNetwork.ConnectUsingSettings();
                }
                else if (response == "username_exists")
                {
                    buttonText.text = "Username already exists.";
                }
                else
                {
                    buttonText.text = "Error: " + response;
                }
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby 1");
    }

    public void OnClickExit()
    {
        buttonClick.Play();
        Application.Quit();
    }

    private void OnInputValueChanged(string arg0)
    {
        keyClick.Play();
    }
}
