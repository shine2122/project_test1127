using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

//사인업
public struct SignUpForm
{  
public string username;
public string password;
public string nickname;
public int score;
}

// 응답
public struct LogInResult
{
    public int result;
}

public enum ResType
{
    INVALID_USERNAME = 0,
    INVALID_PASSWORD,
    SUCCESS,
}

//로그인
public struct LogInForm
{
    public string username;
    public string password;
}

public class SignUpManager : MonoBehaviour {

    //사인업
    public Image signupPanel;
    public InputField usernameInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public InputField nicknameInputField;

    //로그인
    public InputField loginNameInputField;
    public InputField loginPassInputField;
    public Button loginButton;
    bool isPushed = false;

    private void Start()
    {
        loginButton.interactable = false;
    }

    // 회원가입 버튼 이벤트
    public void OnClickSignUpButton()
    {
        signupPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    // 로그인 버튼 이벤트
    public void OnClickLogInButton()
    {
        string username = loginNameInputField.text;
        string password = loginPassInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        LogInForm logInForm = new LogInForm();
        logInForm.username = username;
        logInForm.password = password;

        loginButton.interactable = false;
        StartCoroutine(LogIn(logInForm));
    }
    // 확인 버튼 이벤트
    public void OnClickConfirmButton()
    {
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        string username = usernameInputField.text;
        string nickname = nicknameInputField.text;

        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)
            || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname))
        {
            return;
        }

        if (password.Equals(confirmPassword))
        {
            // TODO: 서버에 회원가입 정보 전송
            SignUpForm signupForm = new SignUpForm();
            signupForm.username = username;
            signupForm.password = password;
            signupForm.nickname = nickname;

            StartCoroutine(SignUp(signupForm));
        }

    }
    // 취소 버튼 이벤트
    public void OnClickCancelButton()
    {
        signupPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(900, 0);
    }

    IEnumerator SignUp(SignUpForm form)
    {
        string postData = JsonUtility.ToJson(form);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/user/add", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator LogIn(LogInForm form)
    {
        string postData = JsonUtility.ToJson(form);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/users/signin", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.Send();
           loginButton.interactable = true;

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string resultStr = www.downloadHandler.text;

                var result = JsonUtility.FromJson<LogInResult>(resultStr);

                if (result.result == 2)
                {
                    SceneManager.LoadScene("Game");
                }

                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public void UpdateLogInInputfield()
    {
        if (!string.IsNullOrEmpty(loginNameInputField.text) && !string.IsNullOrEmpty(loginPassInputField.text))
        {
           loginButton.interactable= true;
           
        }
        else
        {
            loginButton.interactable = false;

        }
    }
}
