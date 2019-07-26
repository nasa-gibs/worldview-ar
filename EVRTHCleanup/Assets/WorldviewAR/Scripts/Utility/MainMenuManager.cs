using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    private bool start = false;

    [SerializeField]
    private Button freePlayButton, soloGameButton, multiplayerGameButton, aquaButton, auraButton, iceSat2Button, landsat8Button, startButton;
    [SerializeField]
    private GameObject modeCanvas, imageTargetCanvas, startCanvas, punCanvas;

    private void Awake () {
        freePlayButton.onClick.AddListener(delegate { SetGameMode(0); });
        soloGameButton.onClick.AddListener(delegate { SetGameMode(1); });
        multiplayerGameButton.onClick.AddListener(delegate { SetGameMode(2); });
        aquaButton.onClick.AddListener(delegate { SetTargetIndex(0); });
        auraButton.onClick.AddListener(delegate { SetTargetIndex(1); });
        iceSat2Button.onClick.AddListener(delegate { SetTargetIndex(2); });
        landsat8Button.onClick.AddListener(delegate { SetTargetIndex(3); });
        startButton.onClick.AddListener(StartGame);
        InitFirstState();
	}

    private void InitFirstState(){
        modeCanvas.SetActive(true);
        imageTargetCanvas.SetActive(false);
        startCanvas.SetActive(false);
        punCanvas.SetActive(false);
    }

    private void SetGameMode(int mode)
    {
        UserInformation.GameMode = mode;
        StartCoroutine(LoadScene());
        InitSecondState();
    }

    private void InitSecondState(){
        modeCanvas.SetActive(false);
        imageTargetCanvas.SetActive(true);
    }

    private void SetTargetIndex(int index)
    {
        UserInformation.TargetIndex = index;
        InitThirdState();
    }

    private void InitThirdState(){
        imageTargetCanvas.SetActive(false);
        if(UserInformation.GameMode == 2){
            punCanvas.SetActive(true);
        }
        else{
            startCanvas.SetActive(true);
        }
    }

    private void StartGame(){
        start = true;
    }

    IEnumerator LoadScene(){
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Mathf.Clamp(UserInformation.GameMode+1, 0, 2));
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            startButton.GetComponentInChildren<Text>().text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
            startButton.interactable = false;

            if (asyncOperation.progress >= 0.9f)
            {
                startButton.GetComponentInChildren<Text>().text = "Start";
                startButton.interactable = true;

                if (start)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
