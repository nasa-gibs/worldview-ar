using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    private float screenTimer = 5f;
    private bool countdown = false;

    [SerializeField]
    private Button confirmationYes, confirmationNo, startButton;
    [SerializeField]
    private Button[] answerButtons;
    [SerializeField]
    private Text questionText, questionScreenTitle, questionScreenAnswers, answerScreenTitle, 
        answerScreenAnswerCorrect,confirmationText, timerText, loadingText,
        primaryDataSetText, secondaryDataSetText;
    [SerializeField]
    private Slider progressBar;
    [SerializeField]
    private GameObject confirmationScreen, welcomeScreen, questionScreen, answerScreen, synchronizeScreen, loadingScreen, playerAnswerText, playerAnswerContent,
                        playerScoreText, playerScoreContent;
    [SerializeField]
    private MapTileManager mapTileManager;

	private void Awake()
	{
        InitListeners();
	}

	private void Update()
	{
        if (countdown) { ScreenProgress(); }
	}

	private void InitListeners(){
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int copy = i;
            answerButtons[i].onClick.AddListener(delegate { AnswerButton(copy); });
        }
        confirmationYes.onClick.AddListener(ConfirmAnswer);
        confirmationNo.onClick.AddListener(RejectAnswer);
        startButton.onClick.AddListener(StartGameButton);
    }

    private void StartProgressBar(float timer){
        progressBar.gameObject.SetActive(true);
        progressBar.maxValue = timer;
        screenTimer = timer;
        countdown = true;
    }

    private void StopProgressBar(){
        progressBar.gameObject.SetActive(false);
        countdown = false;
    }

    private void ScreenProgress(){
        screenTimer -= Time.deltaTime;
        progressBar.value = screenTimer;
    }

    private void StartGameButton()
    {
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().state = 1;
        welcomeScreen.SetActive(false);
        ShowSynchronizeScreen();
    }

    private void AnswerButton(int buttonNumber)
    {
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().answer = answerButtons[buttonNumber].GetComponentInChildren<Text>().text;
        confirmationText.text = "Confirm to answer: " + answerButtons[buttonNumber].GetComponentInChildren<Text>().text;
        confirmationScreen.SetActive(true);
    }

    private void ConfirmAnswer()
    {
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().state = 3;
        confirmationScreen.SetActive(false);
    }

    private void RejectAnswer()
    {
        confirmationScreen.SetActive(false);
    }

	public void SetQuestionText(string text){
        questionText.text = text;
        questionScreenTitle.text = text;
    }

    public void SetAnswerText(string[] answerArray){
        string[] answers = answerArray;
        string answersCombined = "";
        for (int i = 0; i < answerButtons.Length; i++){
            answerButtons[i].GetComponentInChildren<Text>().text = answers[i];
            answersCombined = answersCombined + "\n" + "Answer " + (i + 1).ToString() + ": " + answers[i];
        }
        questionScreenAnswers.text = answersCombined;
    }

    public void SetScoreText(GameObject[] allPlayers){
        foreach (Transform child in playerScoreContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(GameObject player in allPlayers){
            GameObject playerText = Instantiate(playerScoreText, playerScoreContent.transform);
            string playerName;
            if (PhotonNetwork.connected) { playerName = player.GetComponent<PhotonView>().owner.NickName; }
            else { playerName = "Player"; }
            string playerScore = player.GetComponent<PlayerManager>().score.ToString();
            playerText.GetComponent<Text>().text = playerName + ": " + playerScore;
        }
    }

    public void SetTimerText(float timer){
        timerText.text = "Time: " + timer.ToString();
        if (timer <= 0){
            ConfirmAnswer();
        }
    }

    public void SetPrimaryDataSetText(string text){
        primaryDataSetText.text = text;
    }

    public void SetSecondaryDataSetText(string text){
        secondaryDataSetText.text = text;
    }

    public void SetAnswerScreen(string titleQuestion, GameObject[] allPlayers, string answerCorrect, int questionValue){
        foreach(Transform child in playerAnswerContent.transform){
            Destroy(child.gameObject);
        }
        foreach(GameObject player in allPlayers){
            GameObject playerInfoText = Instantiate(playerAnswerText, playerAnswerContent.transform);
            string playerName;
            if (PhotonNetwork.connected) { playerName = player.GetComponent<PhotonView>().owner.NickName; }
            else { playerName = "Player"; }
            string answer = player.GetComponent<PlayerManager>().answer;
            string score = questionValue.ToString();
            if (answer == answerCorrect) { playerInfoText.GetComponent<Text>().color = Color.green; }
            else { playerInfoText.GetComponent<Text>().color = Color.red; score = 0.ToString(); }
            playerInfoText.GetComponent<Text>().text = playerName + " Answered:\t\t" + answer + "\t\t+" + score + " Points";
        }
        answerScreenTitle.text = titleQuestion;
        answerScreenAnswerCorrect.text = "Correct Answer: " + answerCorrect;
        answerScreenAnswerCorrect.color = Color.green;
    }

    public IEnumerator ShowQuestionScreen(){
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().state = 0;
        questionScreen.SetActive(true);
        StartProgressBar(5f);
        yield return new WaitForSeconds(5f);
        StopProgressBar();
        questionScreen.SetActive(false);
        StartCoroutine(ShowLoadingScreen());
    }

    public IEnumerator ShowAnswerScreen(){
        answerScreen.SetActive(true);
        StartProgressBar(5f);
        yield return new WaitForSeconds(5f);
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().state = 1;
        yield return new WaitForSeconds(1f);
        answerScreen.SetActive(false);
    }

    public IEnumerator ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
        while(!mapTileManager.CheckTilesLoaded()){
            loadingText.text = mapTileManager.PercentTilesLoaded().ToString() + "%";
            yield return null;
        }
        loadingText.text = "Loading Complete\nWaiting for other players...";
        PlayerManager.LocalPlayerInstance.gameObject.GetComponent<PlayerManager>().state = 2;
    }

    public void HideLoadingScreen(){
        loadingScreen.SetActive(false);
    }

    public void ShowSynchronizeScreen(){
        synchronizeScreen.SetActive(true);
    }

    public void HideSynchronizeScreen(){
        synchronizeScreen.SetActive(false);
    }
}
