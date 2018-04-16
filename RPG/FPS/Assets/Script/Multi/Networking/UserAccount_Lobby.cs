using UnityEngine;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour
{

    public Text usernameText;

    private void Start()
    {
        if (UserAccountManager.isLoggedIn) //Displays username
            usernameText.text = "Logged in as: " + UserAccountManager.playerUsername;
    }

    public void LogOut()
    {
        if (UserAccountManager.isLoggedIn)//logs user out
            UserAccountManager.instance.LogOut();
    }
}
