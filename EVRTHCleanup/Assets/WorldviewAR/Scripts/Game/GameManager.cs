using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using EVRTH.Scripts.Utility;

public class GameManager : MonoBehaviour {
    private QuestionManager questionManager;
    private GameUIManager gameUIManager;
    private GameObject[] allPlayers;
    private PlayerManager localPlayerManager;
    private float timer = 60f;
    private bool readyState = true, timerActive = false;

    private const float questionTimer = 60f;
    private const int questionValue = 100;

    [SerializeField]
    private LayerPresetLoader layerPresetLoader;
    [SerializeField]
    private MapTileManager mapTileManager;
    [SerializeField]
    private ToolManager toolManager;
    [SerializeField]
    private GameObject playerPrefab;

    public static GameManager GameManagerInstance;

	private void Awake()
	{
        GameManagerInstance = this;
	}

	private void Start()
	{
        InitLocalPlayer();
        questionManager = GetComponent<QuestionManager>();
        gameUIManager = GetComponent<GameUIManager>();
        localPlayerManager = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
	}

	private void Update()
	{
        UpdateTimer();
        UpdateStateMachine();
	}

    private void InitLocalPlayer(){
        if (!PlayerManager.LocalPlayerInstance)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity, 0);
                PhotonNetwork.OnEventCall += OnSetQuestion;
                PhotonNetwork.OnEventCall += OnStartQuestion;
                PhotonNetwork.OnEventCall += OnAnswerQuestion;
            }
            else
            {
                Instantiate(playerPrefab);
            }
        }
    }

    private void UpdateStateMachine(){
        if (PhotonNetwork.isMasterClient || !PhotonNetwork.connected)
        {
            //Master client should always be synchronized
            localPlayerManager.synchronized = true;

            //Idle
            if (CheckAllPlayersState(0))
            {
                return;
            }
            //Ready to set question
            else if (CheckAllPlayersState(1))
            {
                if (PhotonNetwork.connected)
                {
                    PhotonNetwork.RaiseEvent((byte)0, questionManager.GetQuestionIndex() + 1, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });
                }
                else
                {
                    OnSetQuestion(0, questionManager.GetQuestionIndex() + 1, 0);
                }
            }
            //Ready to start question
            else if (CheckAllPlayersState(2))
            {
                if (PhotonNetwork.connected)
                {
                    PhotonNetwork.RaiseEvent((byte)1, 0, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });
                }
                else
                {
                    OnStartQuestion(1, 0, 0);
                }
            }
            //Ready to check answer
            else if (CheckAllPlayersState(3))
            {
                if (PhotonNetwork.connected)
                {
                    PhotonNetwork.RaiseEvent((byte)2, 0, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });
                }
                else
                {
                    OnAnswerQuestion(2, 0, 0);
                }
            }
        }
    }

    private void OnSetQuestion(byte eventcode, object content, int senderid)
    {
        if (eventcode != 0 || localPlayerManager.state != 1) { return; }
        localPlayerManager.synchronized = true;
        gameUIManager.HideSynchronizeScreen();
        localPlayerManager.state = 0;
        questionManager.SetQuestionIndex((int)content);
        SetQuestion();
        gameUIManager.StartCoroutine(gameUIManager.ShowQuestionScreen());
    }

    private void OnStartQuestion(byte eventcode, object content, int senderid)
    {
        if (eventcode != 1 || localPlayerManager.state != 2 || !localPlayerManager.synchronized) { return; }
        localPlayerManager.state = 0;
        SetTimer(60f);
        timerActive = true;
        gameUIManager.HideLoadingScreen();
    }

    private void OnAnswerQuestion(byte eventcode, object content, int senderid)
    {
        if (eventcode != 2 || localPlayerManager.state != 3 || !localPlayerManager.synchronized) { return; }
        localPlayerManager.state = 0;
        CheckAnswer();
    }

    private bool CheckAllPlayersState(int state){
        foreach(GameObject player in GetAllPlayers()){
            if(player.GetComponent<PlayerManager>().state != state && player.GetComponent<PlayerManager>().synchronized){
                return false;
            }
        }
        return true;
    }

    private GameObject[] GetAllPlayers(){
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        return allPlayers;
    }

    private void UpdateTimer(){
        if(timer <= 0 || !timerActive){
            timer = 0;
            return;
        }
        timer -= Time.deltaTime;
        gameUIManager.SetTimerText(GetTimer());
    }

    private void SetTimer(float timerTime){
        timer = timerTime;
    }

    private float GetTimer(){
        return Mathf.RoundToInt(timer);
    }

    private void CheckAnswer(){
        SetTimer(0f);
        timerActive = false;
        if(PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().answer == questionManager.GetCorrectAnswer(questionManager.GetQuestionIndex())){
            PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().score += questionValue;
        }
        gameUIManager.SetAnswerScreen(GetQuestionText(), GetAllPlayers(), questionManager.GetCorrectAnswer(questionManager.GetQuestionIndex()), questionValue);
        StartCoroutine(gameUIManager.ShowAnswerScreen());
    }

    private void SetPreset(int preset, Date date){
        layerPresetLoader.date = date;
        layerPresetLoader.ApplyPreset(preset);
    }

    private void SetTool(int tool){
        toolManager.ActivateTool(tool);
    }

    private void SetQuestion(){
        if(questionManager.GetToolIndex(questionManager.GetQuestionIndex()) == 2){
            StartCoroutine(SetABQuestionTimeout());
        }
        else{
            StartCoroutine(SetQuestionTimeout());
        }
        SetTool(questionManager.GetToolIndex(questionManager.GetQuestionIndex()));
        gameUIManager.SetAnswerText(GetAnswerTextArray());
        gameUIManager.SetQuestionText(GetQuestionText());
        gameUIManager.SetScoreText(GetAllPlayers());
    }

    private string GetQuestionText(){
        return questionManager.GetQuestionText(questionManager.GetQuestionIndex());
    }

    private string[] GetAnswerTextArray()
    {
        return questionManager.GetRandomAnswerArray(questionManager.GetQuestionIndex());
    }

    IEnumerator SetABQuestionTimeout(){
        SetPreset(questionManager.GetPresetIndexAB(questionManager.GetQuestionIndex()), questionManager.GetDateAB(questionManager.GetQuestionIndex()));
        gameUIManager.SetSecondaryDataSetText("Secondary Data Set: " + layerPresetLoader.presets[layerPresetLoader.currentPreset].presetName + ": " + string.Format("{0:MM/dd/yyyy}", layerPresetLoader.date.ToDateTime));
        yield return new WaitForSeconds(2f);
        float timeLoading = 0;
        float percentLoaded = 0;
        while(!mapTileManager.CheckTilesLoaded()){
            if ((int)percentLoaded == (int)mapTileManager.PercentTilesLoaded())
            {
                timeLoading += Time.deltaTime;
            }
            else
            {
                timeLoading = 0;
            }
            percentLoaded = mapTileManager.PercentTilesLoaded();

            if (timeLoading > 10f){
                Date newDate = questionManager.GetDateAB(questionManager.GetQuestionIndex());
                newDate.day += 1;
                SetPreset(questionManager.GetPresetIndexAB(questionManager.GetQuestionIndex()), newDate);
                timeLoading = 0f;
                yield return null;
            }
            yield return null;
        }
        mapTileManager.SaveTextureAllTiles();
        SetPreset(questionManager.GetPresetIndex(questionManager.GetQuestionIndex()), questionManager.GetDate(questionManager.GetQuestionIndex()));
        gameUIManager.SetPrimaryDataSetText("Primary Data Set: " + layerPresetLoader.presets[layerPresetLoader.currentPreset].presetName + ": " + string.Format("{0:MM/dd/yyyy}", layerPresetLoader.date.ToDateTime));
    }

    IEnumerator SetQuestionTimeout(){
        SetPreset(questionManager.GetPresetIndex(questionManager.GetQuestionIndex()), questionManager.GetDate(questionManager.GetQuestionIndex()));
        gameUIManager.SetPrimaryDataSetText("Primary Data Set: " + layerPresetLoader.presets[layerPresetLoader.currentPreset].presetName + ": " + string.Format("{0:MM/dd/yyyy}", layerPresetLoader.date.ToDateTime));
        gameUIManager.SetSecondaryDataSetText("");
        yield return new WaitForSeconds(2f);
        float timeLoading = 0;
        float percentLoaded = 0;
        while (!mapTileManager.CheckTilesLoaded())
        {
            if((int)percentLoaded == (int)mapTileManager.PercentTilesLoaded()){
                timeLoading += Time.deltaTime;
            }
            else{
                timeLoading = 0;
            }
            percentLoaded = mapTileManager.PercentTilesLoaded();

            if (timeLoading > 8f)
            {
                Date newDate = questionManager.GetDate(questionManager.GetQuestionIndex());
                newDate.day += 1;
                SetPreset(questionManager.GetPresetIndex(questionManager.GetQuestionIndex()), newDate);
                timeLoading = 0f;
                yield return null;
            }
            yield return null;
        }
    }
}
