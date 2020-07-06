﻿using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;

using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;

public class GateKeeper : MonoBehaviour {

    public bool usingFBGoogle;
    private string FacebookUserName = null;

    #region LoginWithEmail
    [Header("Login Gate")]
    [Tooltip("Contains the flashing \"Press Any Key\" graphic.")] [SerializeField] GameObject loginGateHandler;
    [Tooltip("Contains the login/signup selection buttons.")] [SerializeField] GameObject loginSelectHandler;
    [Tooltip("Time in seconds before the login prompt is reset. Defaults to five minutes.")] [SerializeField] float waitTimer = 300.0f;

    [Header("Login Elements")]
    [Tooltip("Contains E-mail and Password input fields, as well as the signup and login buttons.")][SerializeField] GameObject loginElementsHandler;
    [Tooltip("E-mail input field.")] [SerializeField] InputField loginUserEmail;
    [Tooltip("Password input field.")] [SerializeField] InputField loginUserPassword;

    [Header("Register Elements")]
    [Tooltip("Contains Username input field as well as the actual register button.")] [SerializeField] GameObject registerElementsHandler;
    [Tooltip("E-mail input field.")] [SerializeField] InputField registerUserEmail;
    [Tooltip("Password input field.")] [SerializeField] InputField registerUserPassword;
    [Tooltip("Username input field.")] [SerializeField] InputField registerUserName;

    [Header("Loading Elements")]
    [Tooltip("Contains loading icon and text.")] [SerializeField] GameObject loadingElementHandler;
    [Tooltip("Contains loading text.")] [SerializeField] Text loadingText;
    [Tooltip("Name of scene to load upon successful authentication")] [SerializeField] string nextScene = "StartScene";

    [Header("Miscellanous Messaging")]
    [Tooltip("Contains error text.")] [SerializeField] Text errorText;
    [Tooltip("Contains application version details")] [SerializeField] Text versionText;
    

    // Private variables
    private float currentTime;
    private string PlayerIDCache;
    private bool multiBool = true;

    public void RegisterPlayer()
    {
        hideErrorMessage();
        showLoading();
        registerElementsHandler.SetActive(false);
        updateLoadingText("Registering New User");
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest() {
            Email = registerUserEmail.text,
            Password = registerUserPassword.text,
            Username = registerUserName.text,
            TitleId = PlayFab.PlayFabSettings.TitleId
        }, PhotonRequestToken, RegisterError);
    }

    public void AddContactEmail()
    {
        var request = new AddOrUpdateContactEmailRequest
        {
            EmailAddress = registerUserEmail.text,
        };
        
        PlayFabClientAPI.AddOrUpdateContactEmail(request, result =>
        {
            Debug.Log("The player's account has been updated with a contact email");

        }, FailureCallback);
    }
    public void LoginPlayer()
    {
        hideErrorMessage();
        showLoading();
        loginElementsHandler.SetActive(false);
        updateLoadingText("Attempting to Log In");
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest() {
            Email = loginUserEmail.text,
            Password = loginUserPassword.text,
            TitleId = PlayFab.PlayFabSettings.TitleId,

    }, PhotonRequestToken, AuthError);
    }
    void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void PhotonRequestToken(PlayFab.ClientModels.LoginResult res)
    {
        updateLoadingText("Requesting Authentication Token");
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, PhotonAuth, AuthError);
    }

    private void PhotonRequestToken(RegisterPlayFabUserResult res)
    {
        updateLoadingText("Requesting Authentication Token");
        PlayerIDCache = res.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest() {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime
        }, PhotonAuth, RegisterError);
        AddContactEmail();
    }

    private void PhotonAuth(GetPhotonAuthenticationTokenResult res)
    {
        emptyLoginFields();
        emptyRegisterFields();
        updateLoadingText("Loading Game");
        // TODO: Change CustomAuthenticationType accordingly (maybe Steam?) in the future
        var authDetails = new Photon.Realtime.AuthenticationValues { AuthType = Photon.Realtime.CustomAuthenticationType.Custom };
        authDetails.AddAuthParameter("username", PlayerIDCache);
        authDetails.AddAuthParameter("token", res.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = authDetails;
        StartCoroutine(LoadNextScene());
    }

    private void AuthError(PlayFabError err)
    {
        hideLoading();
        loginElementsHandler.SetActive(true);
        Debug.LogError("Authentication Error: " + err.GenerateErrorReport());
        showErrorMessage("AUTHENTICATION ERROR\n" + err.ErrorMessage);
    }

    private void RegisterError(PlayFabError err)
    {
        hideLoading();
        registerElementsHandler.SetActive(true);
        Debug.LogError("Registration Error: " + err.GenerateErrorReport());
        showErrorMessage("REGISTRATION ERROR\n" + err.ErrorMessage);
    }

    private void updateLoadingText(string loadMsg)
    {
        loadingText.text = loadMsg;
    }

    private void showLoading()
    {
        loadingElementHandler.SetActive(true);
    }

    private void hideLoading()
    {
        loadingElementHandler.SetActive(false);
        loadingText.text = null;
    }

    private void showErrorMessage(string errorMsg)
    {
        errorText.text = errorMsg;
        errorText.gameObject.SetActive(true);
    }

    public void hideErrorMessage()
    {
        errorText.text = null;
        errorText.gameObject.SetActive(false);
    }

    public void emptyLoginFields()
    {
        loginUserEmail.text = null;
        loginUserPassword.text = null;
    }

    public void emptyRegisterFields()
    {
        registerUserEmail.text = null;
        registerUserPassword.text = null;
        registerUserName.text = null;
    }

    private void returnToGate()
    {
        emptyLoginFields();
        emptyRegisterFields();
        loginGateHandler.SetActive(true);
        loginSelectHandler.SetActive(false);
        loginElementsHandler.SetActive(false);
        registerElementsHandler.SetActive(false);
        hideErrorMessage();
        hideLoading();
    }

    IEnumerator loginReset()
    {
        currentTime = waitTimer;
        while (currentTime > 0.0f)
        {
            currentTime -= Time.deltaTime;
            yield return null;
        }
        returnToGate();
    }

    #endregion LoginWithEmail

    #region LoginWithFB

    private string _message;

    public void OnClick_FacebookSignIn()
    {
        SetMessage("Initializing Facebook..."); // logs the given message and displays it on the screen using OnGUI method

        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
        FB.Init(OnFacebookInitialized);
    }

    private void OnFacebookInitialized()
    {
        SetMessage("Logging into Facebook...");

        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
        if (FB.IsLoggedIn)
            FB.LogOut();

        // We invoke basic login procedure and pass in the callback to process the result
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }

    private void OnFacebookLoggedIn(ILoginResult iresult)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (iresult == null || string.IsNullOrEmpty(iresult.Error))
        {
            SetMessage("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest 
            { 
                CreateAccount = true, 
                AccessToken = AccessToken.CurrentAccessToken.TokenString 
            },
                result => 
                {
                    Debug.Log("Logged In FB");

                    PhotonRequestToken(result);
                    GetFacebookUserName("me", res =>
                    {
                        FacebookUserName = res.ResultDictionary["name"].ToString();
                    });
                }
                , OnPlayfabFacebookAuthFailed);
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            SetMessage("Facebook Auth Failed: " + iresult.Error + "\n" + iresult.RawResult, true);
        }
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }

    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }

    /// <param name="id">Unique identifier of a Facebook profile.</param>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public void GetFacebookUserName(string id, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        FB.API("/" + id, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                Debug.Log(string.Format("GetFacebookUserName => Success! (name: {0})",
                    res.ResultDictionary["name"]));

                if (successCallback != null)
                    successCallback(res);
            }));

        SetPlayerName();
    }

    private bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        Debug.LogError(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})",

            result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;
    }

    public void SetPlayerName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = FacebookUserName
        },
        resultN =>
        {
            print("Done");
        },
        errorN =>
        {
            print(errorN.ErrorMessage);
            print(FacebookUserName);
        });
    }

    #endregion LoginWithFB

    IEnumerator LoadNextScene()
    {
        AsyncOperation AO = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        AO.allowSceneActivation = false;
        while (AO.progress < 0.9f)
        {
            yield return null;
        }

        AO.allowSceneActivation = true;
    }

    void Start()
    {
        currentTime = 0.0f;
        returnToGate();
        versionText.text += ("\nVersion " + Application.version);
    }

    void Update()
    {
        if (Input.anyKeyDown && !usingFBGoogle)
        {
            if (loginGateHandler.activeInHierarchy)
            {
                loginGateHandler.SetActive(false);
                loginSelectHandler.SetActive(true);
                StartCoroutine("loginReset");
            }
            else currentTime = waitTimer;
        }
    }
}
