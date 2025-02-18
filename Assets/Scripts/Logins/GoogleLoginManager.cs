using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Google;
using PlayFab.ClientModels;
using PlayFab;

public class GoogleLoginManager : MonoBehaviour
{
    public string imageURL;
    public string userNameStr;
    public Sprite _profilePic;
    public string Name;
    public bool googleLoginbool = false;
    public Image defaultAvatar;
    private GoogleSignInConfiguration configuration;
    private string webClientId = "627207061788-0qt59can4vo3q0obmdjf31lk2ldrnbga.apps.googleusercontent.com";

    public static GoogleLoginManager instance;
    public TextMeshProUGUI gname;
   // public TextMeshProUGUI id;
    public Image Google_userDp;
    public GameObject panel;
   // public GameObject openpanel;

    private const string GoogleUserNameKey = "GoogleUserNameKey";
    private const string GoogleUserIdKey = "GoogleUserIdKey";
    private const string GoogleUserDpKey = "GoogleUserDpKey";

    void Awake()
    {
        Debug.Log("Google login manager Awake");

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true,
            RequestAuthCode = true
        };
    }

    private void Start()
    {
        Debug.Log("Google login manager Start");

        GlobalManager.Instance.InitializeGoogleLogin();
        googleLoginbool = PlayerPrefs.GetInt("googleLoginbool", 0) == 1;
        if (googleLoginbool)
        {
            Debug.Log("Google login manager Start 11111");
            LoadGoogleData();
            StartCoroutine(ShowPanels());
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            OnAuthenticationFinished, TaskScheduler.Default);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Google SignIn failed.");
            foreach (var exception in task.Exception.InnerExceptions)
            {
                var error = (GoogleSignIn.SignInException)exception;
                Debug.LogError("Error: " + error.Status + " " + error.Message);
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Google SignIn was canceled.");
        }
        else
        {
            userNameStr = task.Result.DisplayName;
            gname.text = userNameStr;
            SetDisplayNameGoogle(userNameStr);
            // id.text = task.Result.IdToken;

            googleLoginbool = true;
            PlayerPrefs.SetInt("googleLoginbool", googleLoginbool ? 1 : 0);
            PlayerPrefs.SetString(GoogleUserNameKey, task.Result.DisplayName);
            PlayerPrefs.SetString(GoogleUserIdKey, task.Result.IdToken);

            // string profilePicUrl = task.Result.ImageUrl != null ? task.Result.ImageUrl.ToString() : null;
            string profilePicUrl = task.Result.ImageUrl.ToString();
            Debug.Log("Google login manager OnAuthenticationFinished====>" + profilePicUrl);
            if (profilePicUrl != null)
            {
                PlayerPrefs.SetString(GoogleUserDpKey, profilePicUrl);
            }
            PlayerPrefs.Save();

            StartCoroutine(ShowPanels(profilePicUrl));
            LoadGoogleData();
        }
    }
    public void SetDisplayNameGoogle(string FBName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = FBName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdated, NameOnError);
    }

    private void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Display Name Updated Successfully: " + result.DisplayName);
    }

    private void NameOnError(PlayFabError error)
    {
        Debug.LogError("Error updating display name: " + error.GenerateErrorReport());
    }
    public void OnSignOut()
    {
        PlayerPrefs.DeleteKey(GoogleUserNameKey);
        PlayerPrefs.DeleteKey(GoogleUserIdKey);
        PlayerPrefs.DeleteKey(GoogleUserDpKey);
        PlayerPrefs.DeleteKey("googleLoginbool");
        PlayerPrefs.Save();

        GoogleSignIn.DefaultInstance.SignOut();
        googleLoginbool = false;
        ResetUserData();

        StartCoroutine(HidePanels());
    }

    private void ResetUserData()
    {
        if (gname != null) gname.text = "New Text";
    }

    private IEnumerator ShowPanels(string profilePicUrl = null)
    {
        Debug.Log("Google login manager ShowPanels====>" + profilePicUrl);
        yield return new WaitForEndOfFrame();  // Wait until the frame is fully rendered
        if (panel != null) panel.SetActive(false);
        //if (openpanel != null) openpanel.SetActive(true);
        defaultAvatar.enabled = true;
        defaultAvatar.gameObject.SetActive(true);

        if (defaultAvatar != null)
        {
            bool shouldShowDefaultAvatar = string.IsNullOrEmpty(profilePicUrl);
            Debug.Log("Google login manager ShowPanels DefaultAvatar====>" + shouldShowDefaultAvatar);
            defaultAvatar.enabled = shouldShowDefaultAvatar;
            Google_userDp.enabled = !shouldShowDefaultAvatar;

        }
    }

    private IEnumerator HidePanels()
    {
        yield return new WaitForEndOfFrame();  // Wait until the frame is fully rendered
        if (panel != null) panel.SetActive(true);
       // if (openpanel != null) openpanel.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetUserData();
    }

    public void LoadGoogleData()
    {
        Debug.Log("Google login manager LoadGoogleData 11111====>");
        string savedProfilePicUrl = null;

        if (PlayerPrefs.HasKey(GoogleUserNameKey))
        {
            string savedName = PlayerPrefs.GetString(GoogleUserNameKey);
            gname.text = savedName;
        }

        if (PlayerPrefs.HasKey(GoogleUserDpKey))
        {
            Debug.Log("Google login manager LoadGoogleData 2222====>");
            savedProfilePicUrl = PlayerPrefs.GetString(GoogleUserDpKey);
        }
        Debug.Log("Google login manager LoadGoogleData 3333====>" + savedProfilePicUrl);
        StartCoroutine(ShowPanels(savedProfilePicUrl));

        if (!string.IsNullOrEmpty(savedProfilePicUrl))
        {
            StartCoroutine(LoadProfilePic(savedProfilePicUrl));
        }
    }

    private IEnumerator LoadProfilePic(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            _profilePic = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            Google_userDp.sprite = _profilePic;

            PlayerPrefs.SetString(GoogleUserDpKey, imageUrl);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("Failed to load profile picture: " + www.error);
        }
    }
}
