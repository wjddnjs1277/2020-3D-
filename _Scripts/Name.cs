using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Name : MonoBehaviour
{
    public InputField nickNameInput;

    public void MainScene()
    {
        PlayerMovement.nickName = nickNameInput.text;
        NetworkManager.nickName = nickNameInput.text;
        SceneManager.LoadScene("Main");
    }

}
