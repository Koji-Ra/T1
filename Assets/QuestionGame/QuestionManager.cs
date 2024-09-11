using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public int scores = 0;
    public int questionCount = 1;
    public TextMeshProUGUI scoreText, endGameText;
    [SerializeField] Animator animator;
    [SerializeField] GameObject[] questions;
    // Start is called before the first frame update
    void Start()
    {
        //questions = GameObject.FindGameObjectsWithTag("Question");
        Debug.Log($"Question count {questions.Length}");
        ActiveQuest();
        endGameText.text = "";
        endGameText.gameObject.SetActive( false );
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + scores;
    }
    public void ActiveQuest()
    {
        foreach (var item in questions)
        {
            item.SetActive(false);
        }
        if(questionCount-1 < questions.Length)
            questions[questionCount-1].SetActive(true);
        else
        {
            //EndGame here
            endGameText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(false);
            endGameText.text = $"Best score: {scores}";
            Debug.Log($"End game you score: {scores}");
        }
    }
    public void Wrong()
    {
        if(animator !=  null)
            animator.SetTrigger("wrong");
    }
    public void Correct()
    {
        if(animator !=  null)
        animator.SetTrigger("correct");
    }
}
