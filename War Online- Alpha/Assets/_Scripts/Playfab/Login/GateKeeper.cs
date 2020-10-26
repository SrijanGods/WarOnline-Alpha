using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GateKeeper : MonoBehaviour
{
    [Header("UI GOs")]
    public GameObject LoginPanel;

    [Header("Player Profile")]
    public Image userImage;

    [Header("FaceBook Login")]
    public Button FBLogin;
    private string FBUserName;
    private string FBid;
    private string FBurl;

    [Header("Play Game Login")]
    public Button GoogleLogin;

    [Space]
    public bool inDev;
    [HideInInspector]
    public bool PlayfabConnected;

    private void Awake()
    {
        FB.Init(() => FB.ActivateApp());

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    private void Start()
    {
        if (inDev)
        {
            PlayerPrefs.DeleteAll();
        }

        FBLogin.onClick.AddListener(OnFacebookInitialized);
        GoogleLogin.onClick.AddListener(GoogleSignIn);

        if (PlayerPrefs.HasKey("LoggedInWithFB"))
        {
            StartCoroutine("FBCall");
        }
        else
        {
            print(PlayerPrefs.GetString("LoggedInWithFB"));
        }
    }

    #region Facebook

    IEnumerator FBCall()
    {
        yield return new WaitUntil(() => FB.IsInitialized);
        OnFacebookInitialized();
    }
    public void OnFacebookInitialized()
    {
        if (FB.IsLoggedIn)
            FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, OnFacebookLoggedIn);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            FBid = result.ResultDictionary["user_id"].ToString();

            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest 
            { 
                CreateAccount = true, 
                AccessToken = AccessToken.CurrentAccessToken.TokenString,
                TitleId = "CAAAA"
            },
            res =>
            {
                PlayerPrefs.SetString("LoggedInWithFB", "Yes");
                if (res.NewlyCreated)
                {
                    FB.API("me?fields=name", HttpMethod.GET, resultCallback =>
                    {
                        FBUserName = resultCallback.ResultDictionary["name"].ToString();

                        if (FBUserName.Length > 20)
                        {
                            string hold = FBUserName.Substring(0, 24);
                            FBUserName = hold;
                        }
                        if (FBUserName.Length < 3)
                        {
                            FBUserName = "000" + FBUserName;
                        }
                        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
                        {
                            DisplayName = FBUserName.ToString()
                        },
                        r =>
                        {
                            print("Name Updated Successfully");
                        },
                        e =>
                        {
                            print(e.Error);
                        });

                    });

                    GetFacebookUserPictureFromUrl("me", 150, 150, resI =>
                    {
                        StartCoroutine(GetTextureFromGraphResult(resI));
                    });

                }
                else
                {
                    PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
                    {
                        PlayFabId = res.PlayFabId,
                        ProfileConstraints = new PlayerProfileViewConstraints
                        {
                            ShowAvatarUrl = true,
                            ShowDisplayName = true
                        }
                    },
                    resP =>
                    {
                        if(resP.PlayerProfile.AvatarUrl != null && resP.PlayerProfile.DisplayName != null)
                        {
                            FBUserName = resP.PlayerProfile.DisplayName;
                            StartCoroutine(DownloadImage(resP.PlayerProfile.AvatarUrl));
                        }
                    },
                    errP =>
                    {
                        print(errP.Error);
                    });
                }
                print("Hmm");
                RequestPhotonToken(res);

            },
            err =>
            {
                print(err.Error);
            });
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
        }
    }

    #region ImageMech

    public void GetFacebookUserPictureFromUrl(string id, int width, int height, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}&redirect=false", id, height, width);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);

            }));

    }

    public IEnumerator GetTextureFromGraphResult(IGraphResult result)
    {
        var data = result.ResultDictionary["data"] as IDictionary<string, object>;
        var url = data["url"].ToString();

        FBurl = url;
        WWW request = new WWW(url);

        yield return new WaitUntil(() => request.isDone);

        UpdateUserAvatarURL(FBurl);
        StartCoroutine("DownloadImage", FBurl);
    }
    #endregion 

    private bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        Debug.LogError(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})",

            result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;

    }

    #endregion

    #region Google

    private void GoogleSignIn()
    {
        Social.localUser.Authenticate((bool success) => {

            if (success)
            {
                print("Google Signing In");
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                Debug.Log("Server Auth Code: " + serverAuthCode);

                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    ServerAuthCode = serverAuthCode,
                    CreateAccount = true
                }, (result) =>
                {
                }, OnPlayFabError);
            }
            else
            {
                print("Google Failed to Authorize your login");
            }

        });

    }

    #endregion

    #region PhotonAuth

    private string _playFabPlayerIdCache;

    private void RequestPhotonToken(PlayFab.ClientModels.LoginResult obj)
    {
        LogMessage("PlayFab authenticated. Requesting photon token...");

        _playFabPlayerIdCache = obj.PlayFabId;


        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        },
        AuthenticateWithPhoton, OnPlayFabError);
    }

    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
    {

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new Photon.Realtime.AuthenticationValues { AuthType = Photon.Realtime.CustomAuthenticationType.Custom };

        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        PhotonNetwork.AuthValues = customAuth;

        // PhotonNetwork will be connected to later in MainScript
        // PhotonNetwork.ConnectUsingSettings();

        PlayfabConnected = true;
        print("PlayFab authenticated with Photon");

        GlobalValues.Instance.loggedIn = true;
    }

    private void OnPlayFabError(PlayFabError obj)
    {
        LogMessage(obj.GenerateErrorReport());
    }

    public void LogMessage(string message)
    {
        print(message);
    }


    #endregion PhotonAuth

    #region PlayfabUniversalFuncs

    IEnumerator DownloadImage(string urlid)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlid);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            userImage.sprite = Sprite.Create(((DownloadHandlerTexture)request.downloadHandler).texture, new Rect(0, 0, ((DownloadHandlerTexture)request.downloadHandler).texture.width, ((DownloadHandlerTexture)request.downloadHandler).texture.height), new Vector2(0, 0));
    }

    private void UpdateUserAvatarURL(string link)
    {
        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
        {
            ImageUrl = link
        },
        res =>
        {
            print("Image Successfully Updated");
        },
        err =>
        {
            print(err.Error);
        });
    }

    #endregion

    //https://answers.unity.com/questions/779452/display-player-image-and-name-using-google.html
}
