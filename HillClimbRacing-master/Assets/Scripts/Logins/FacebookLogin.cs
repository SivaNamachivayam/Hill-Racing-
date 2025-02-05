using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;
using System;
using UnityEngine.Networking; // For fetching the profile picture from URL
using UnityEngine.SceneManagement;

public class FaceBookLogin : MonoBehaviour
{
    public TextMeshProUGUI FB_userName;
    public Image defaultAvatar;
    //public TextMeshProUGUI FB_userId;
    public Image FB_userDp;
    public GameObject panel;
    //public GameObject openpanel;
    public bool FBLoginbool = false;
    public static FaceBookLogin instance;
    public GameObject GuestBtn;

    private const string FBUserNameKey = "FBUserName";
    private const string FBUserIdKey = "FBUserId";
    private const string FBUserDpKey = "FBUserDp"; // Save the profile picture URL

    private void Awake()
    {
        Debug.Log("FB manager Awake ==>" + GlobalManager.Instance);
        /*if (instance == null)
        {
            instance = this;
            GlobalManager.Instance.faceBookLogin = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }*/

    }

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
        else
        {
            FB.ActivateApp();
        }
        GlobalManager.Instance.InitializeFacebookLogin();

        FBLoginbool = PlayerPrefs.GetInt("FBLoginbool", 0) == 1;
        Debug.Log("FB manager Start 11111==>" + FBLoginbool);
        if (FBLoginbool)
        {
            Debug.Log("FB manager Start 22222==>" + FBLoginbool);
            LoadFacebookData();
            panel.gameObject.SetActive(false);
            defaultAvatar.gameObject.SetActive(false);
            // openpanel.gameObject.SetActive(true);
            FB_userDp.gameObject.SetActive(true);
            GuestBtn.gameObject.SetActive(false);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to initialize the Facebook SDK");
        }
    }

    public void Login()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, LoginCallback);
        }
        else
        {
            Debug.Log("Already logged in to Facebook");
        }
    }
    public void LogOut()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }
        else
        {
            Debug.Log("Not logged in to Facebook");
        }

        //FB.LogOut();

        // Clear Facebook-specific PlayerPrefs
        PlayerPrefs.DeleteKey(FBUserNameKey);
        PlayerPrefs.DeleteKey(FBUserIdKey);
        PlayerPrefs.DeleteKey(FBUserDpKey);
        PlayerPrefs.DeleteKey("FBLoginbool");
        PlayerPrefs.Save();

        FBLoginbool = false;
        ResetUserData();
        StartCoroutine(ShowPanels(false));
        //GuestBtn.SetActive(true);
    }
    private void ResetUserData()
    {
        if (FB_userName != null) FB_userName.text = "";
        if (FB_userDp != null) FB_userDp.sprite = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        // Ensure user data is cleared when transitioning between scenes
        if (!FB.IsLoggedIn)
        {
            ResetUserData();  // Clear UI components if logged out
        }
    }

    private void LoginCallback(ILoginResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Facebook login cancelled");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Facebook login error: " + result.Error);
        }
        else if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook login successful");

            // Retrieve user data
            FB.API("/me?fields=id,first_name,last_name,email", HttpMethod.GET, UserDataCallback);
            FBLoginbool = true;
            PlayerPrefs.SetInt("FBLoginbool", FBLoginbool ? 1 : 0);
            PlayerPrefs.Save();
            StartCoroutine(ShowPanels(true));
            GuestBtn.SetActive(false);
        }
    }

    private void UserDataCallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Error retrieving user data: " + result.Error);
        }
        else
        {
            var userData = result.ResultDictionary;
            string userId = userData["id"].ToString();
            string firstName = userData["first_name"].ToString();
            FB_userName.text = firstName;
            //FB_userId.text = userId;


            // Save user data in PlayerPrefs
            PlayerPrefs.SetString(FBUserNameKey, firstName);
            PlayerPrefs.SetString(FBUserIdKey, userId);
            PlayerPrefs.Save();



            FB.API("/me/picture?redirect=false&type=large", HttpMethod.GET, ProfilePictureCallback);
        }
    }
    private void ProfilePictureCallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Error retrieving profile picture: " + result.Error);
        }
        else
        {
            var pictureData = result.ResultDictionary["data"] as Dictionary<string, object>;
            string pictureURL = pictureData["url"].ToString();

            Debug.Log("Profile Picture URL: " + pictureURL);

            PlayerPrefs.SetString(FBUserDpKey, pictureURL);
            PlayerPrefs.Save();

            StartCoroutine(FetchProfilePicture(pictureURL));
        }
    }

    private IEnumerator FetchProfilePicture(string pictureURL)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(pictureURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            FB_userDp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Debug.Log("Error fetching profile picture: " + www.error);
        }
    }

    public void LoadFacebookData()
    {
        Debug.Log("FB manager LoadFacebookData ==>");
        // Load Facebook data from PlayerPrefs
        if (PlayerPrefs.HasKey(FBUserNameKey) && PlayerPrefs.HasKey(FBUserIdKey) && PlayerPrefs.HasKey(FBUserDpKey))
        {
            Debug.Log("FB manager LoadFacebookData 1111 ==>");
            string savedName = PlayerPrefs.GetString(FBUserNameKey);
            string savedUserId = PlayerPrefs.GetString(FBUserIdKey);
            string savedProfilePicUrl = PlayerPrefs.GetString(FBUserDpKey);

            FB_userName.text = savedName;
            // FB_userId.text = savedUserId;

            // Fetch and display the profile picture
            StartCoroutine(FetchProfilePicture(savedProfilePicUrl));
        }
        else
        {
            Debug.Log("No Facebook data found in PlayerPrefs.");
        }
    }

    private IEnumerator ShowPanels(bool show)
    {
        if (show)
        {
            yield return new WaitForSeconds(0.5f); // Optional delay for smooth transitions
            panel.SetActive(false);
            //openpanel.SetActive(false);
            //Google_userDp.enabled = false;
            defaultAvatar.enabled = false;
        }
        else
        {
            yield return new WaitForSeconds(0.5f); // Optional delay for smooth transitions
            panel.SetActive(true);
            //openpanel.SetActive(true);
            //Google_userDp.enabled = false;
            defaultAvatar.enabled = false;
        }
    }
}

