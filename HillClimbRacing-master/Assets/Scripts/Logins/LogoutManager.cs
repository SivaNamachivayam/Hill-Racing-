using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public Button logoutBtn;
    //public Button playBtn;
    public GameObject panel;
    public GameObject settingPanel;
    public GameObject Guestpanel;
    //public GameObject MainmenuPanel;


    // Start is called before the first frame update
    void Start()
    {
        /*if(GoogleLoginManager.instance.googleLoginbool == true || FaceBookLogin.instance.FBLoginbool == true || GuestLogin.instance.guestlogin == true)
        {
             panel.gameObject.SetActive(true);
        }*/
        Debug.Log("LogoutManager ===> " + GlobalManager.Instance.faceBookLogin);
        //Debug.Log("Elan LogoutManager11111 ===> " + FaceBookLogin.instance.FBLoginbool);

        if (GlobalManager.Instance.faceBookLogin != null)
        {
            Debug.Log("LogoutManager 1111===> " + GlobalManager.Instance.faceBookLogin);
            // FaceBookLogin.instance.LoadFacebookData();
        }

    }
    public void LogOutButtonClk()
    {
        Debug.Log("comes Logoutmanager");
        Debug.Log("Logoutmanager ===>" + GlobalManager.Instance.faceBookLogin);

        if (GlobalManager.Instance.googleLoginManager.googleLoginbool == true)
        {
            GlobalManager.Instance.googleLoginManager.OnSignOut();
            panel.gameObject.SetActive(true);
            settingPanel.SetActive(false);
        }
        else if (GlobalManager.Instance.faceBookLogin.FBLoginbool == true)
        {
            GlobalManager.Instance.faceBookLogin.LogOut();
            panel.gameObject.SetActive(true);
            settingPanel.SetActive(false);
        }
        else if (GuestLogin.instance.guestlogin == true)
        {
            GuestLogin.instance.OnLogoutButtonClick();
            //GlobalManager.Instance.guestLogin.OnLogoutButtonClick();
            //GlobalManager.Instance.guestLogin.guestlogin == true
            panel.gameObject.SetActive(true);
            settingPanel.SetActive(false);
        }
    }

    /* public void OnClickPlayBtn()
     {
         panel.SetActive(false);
         Guestpanel.SetActive(false);
         //SceneManager.LoadScene("game");
         MainmenuPanel.SetActive(true);
     }*/
}

