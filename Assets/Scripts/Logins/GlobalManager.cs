using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager
{
    private static GlobalManager instance;
    public FaceBookLogin faceBookLogin { get; private set; }
    public GoogleLoginManager googleLoginManager;
    public GuestLogin guestLogin;

    static GlobalManager()
    {
        instance = new GlobalManager();
        Debug.Log("GlobalManager instance statically initialized.");
    }

    private GlobalManager()
    {
        InitializeFacebookLogin();
        InitializeGoogleLogin();
        //InitializeGuestLogin();
    }

    public static GlobalManager Instance => instance;

    public void InitializeFacebookLogin()
    {
        if (faceBookLogin == null)
        {
            GameObject fbLoginObject = GameObject.Find("FacebookLogin");

            if (fbLoginObject == null)
            {
                fbLoginObject = new GameObject("FacebookLogin");
                faceBookLogin = fbLoginObject.AddComponent<FaceBookLogin>();
                Object.DontDestroyOnLoad(fbLoginObject);
                Debug.Log("FaceBookLogin instance created and added to DontDestroyOnLoad.");
            }
            else
            {
                faceBookLogin = fbLoginObject.GetComponent<FaceBookLogin>();
                Debug.Log("FaceBookLogin instance found in the scene.");
            }
        }
    }

    public void InitializeGoogleLogin()
    {
        if (googleLoginManager == null)
        {
            GameObject googleLoginObject = GameObject.Find("GoogleLoginManager");

            if (googleLoginObject == null)
            {
                googleLoginObject = new GameObject("GoogleLoginManager");
                googleLoginManager = googleLoginObject.AddComponent<GoogleLoginManager>();
                Object.DontDestroyOnLoad(googleLoginObject);
                Debug.Log("FaceBookLogin instance created and added to DontDestroyOnLoad.");
            }
            else
            {
                googleLoginManager = googleLoginObject.GetComponent<GoogleLoginManager>();
                Debug.Log("FaceBookLogin instance found in the scene.");
            }
        }
    }
    /*//public void InitializeGuestLogin()
    //{
    //    if (guestLogin == null)
    //    {
    //        GameObject guestLoginObject = GameObject.Find("GuestLoginManager");

    //        //if (guestLoginObject == null)
    //        //{
    //        //    guestLoginObject = new GameObject("GuestLoginManager");
    //        //    //guestLogin = guestLoginObject.AddComponent<GuestLogin>();
    //        //    //Object.DontDestroyOnLoad(guestLogin);
    //        //    Debug.Log("FaceBookLogin instance created and added to DontDestroyOnLoad.");
    //        //}
    //        //else
    //        //{
    //        //    guestLogin = guestLoginObject.GetComponent<GuestLogin>();
    //        //    Debug.Log("FaceBookLogin instance found in the scene.");
    //        //}
    //    }
    //}*/
}
