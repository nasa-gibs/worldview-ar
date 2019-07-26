using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EVRTH.Scripts.Utility;

public class QuestionManager : MonoBehaviour {
    private int questionIndex = 0;

    [SerializeField]
    private Question[] questions;

    public void SetQuestionIndex(int index)
    {
        if(index > questions.Length - 1){
            questionIndex = 0;
        }
        else if(index < 0){
            questionIndex = questions.Length;
        }
        else{
            questionIndex = index;
        }
    }

    public int GetQuestionIndex()
    {
        return questionIndex;
    }

    public string GetQuestionText(int index)
    {
        return questions[index].questionText;
    }

    public string GetCorrectAnswer(int index)
    {
        return questions[index].correctAnswer;
    }

    public string GetWrongAnswer1(int index)
    {
        return questions[index].wrongAnswer1;
    }

    public string GetWrongAnswer2(int index)
    {
        return questions[index].wrongAnswer2;
    }

    public string GetWrongAnswer3(int index)
    {
        return questions[index].wrongAnswer3;
    }

    public string[] GetRandomAnswerArray(int index){
        string[] arr = { questions[index].correctAnswer, questions[index].wrongAnswer1, questions[index].wrongAnswer2, questions[index].wrongAnswer3};
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            string tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
        return arr;
    }

    public int GetPresetIndex(int index)
    {
        return questions[index].presetIndex;
    }

    public int GetPresetIndexAB(int index){
        return questions[index].presetIndexAB;
    }

    public Date GetDate(int index){
        return questions[index].date;
    }

    public Date GetDateAB(int index){
        return questions[index].dateAB;
    }

    public int GetToolIndex(int index)
    {
        return questions[index].toolIndex;
    }
}
