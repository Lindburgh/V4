using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // ファイルを扱うためのライブラリ

public class QuizManager : MonoBehaviour
{
    public List<Question> questions = new List<Question>(); // 問題リスト
    private Question currentQuestion;

    void Start()
    {
        LoadQuestions();
        SetNextQuestion();
    }

    void LoadQuestions()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("questions"); // CSVを読み込む
        StringReader reader = new StringReader(csvFile.text);
        bool isFirstLine = true;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            if (isFirstLine) 
            { 
                isFirstLine = false; 
                continue; 
            } // ヘッダーをスキップ

            string[] elements = line.Split(',');
            Question question = new Question
            {
                questionText = elements[0],
                choices = new string[] { elements[1], elements[2], elements[3], elements[4] },
                correctAnswer = elements[5]
            };

            questions.Add(question);
        }
    }

    void SetNextQuestion()
    {
        if (questions.Count > 0)
        {
            currentQuestion = questions[Random.Range(0, questions.Count)];
            Debug.Log($"問題: {currentQuestion.questionText}");
            // ここで UI に表示する処理を追加
        }
    }
}

// 質問のデータ構造
[System.Serializable]
public class Question
{
    public string questionText;
    public string[] choices;
    public string correctAnswer;
}

