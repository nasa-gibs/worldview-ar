using System;
using UnityEngine;
using EVRTH.Scripts.Utility;

[Serializable]
public class Question {
    public string questionIdentifier, questionText, correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3;

    [Header("Date")]
    public Date date;

    [Header("Date AB (Optional)")]
    public Date dateAB;

    [Space]
    public int presetIndex, presetIndexAB, toolIndex;

}
