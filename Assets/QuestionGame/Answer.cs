using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    QuestionManager questionManager;
    bool isSelected = false;
    Button answer;
    // Start is called before the first frame update
    void Start()
    {
        questionManager = FindAnyObjectByType<QuestionManager>();
        answer = gameObject.GetComponent<Button>();
        answer.onClick.AddListener(CheckAnser);
    }

    private void CheckAnser()
    {
        if(isSelected) { return; }
        isSelected = true;
        if (gameObject.CompareTag("answer"))
        {
            questionManager.scores++;
            
            questionManager.Correct();
        }
        else
            questionManager.Wrong();

        questionManager.questionCount++;

        questionManager.ActiveQuest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
