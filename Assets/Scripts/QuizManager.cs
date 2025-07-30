using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Quiz
    {
        public GameObject optionA;
        public GameObject optionB;
        public string correctTag; // "A" or "B"
    }

    [Header("クイズリスト（3問）")]
    public List<Quiz> quizzes;

    [Header("被験者ID（例：P001）")]
    public string participantID = "P001"; // Inspector から設定

    private int currentQuizIndex = 0;
    private List<int> results = new List<int>();

    void Start()
    {
        ShowQuiz(currentQuizIndex);
    }

    void ShowQuiz(int index)
    {
        if (index >= quizzes.Count)
        {
            FinishQuiz();
            return;
        }

        quizzes[index].optionA.SetActive(true);
        quizzes[index].optionB.SetActive(true);
    }

    public void SelectOption(string selectedTag)
    {
        Quiz quiz = quizzes[currentQuizIndex];

        bool isCorrect = selectedTag == quiz.correctTag;
        results.Add(isCorrect ? 1 : 0);

        quiz.optionA.SetActive(false);
        quiz.optionB.SetActive(false);

        currentQuizIndex++;
        ShowQuiz(currentQuizIndex);
    }

    void FinishQuiz()
    {
        Debug.Log("クイズ終了");

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"quiz_results_{timestamp}_{participantID}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        List<string> lines = new List<string>();

        // ヘッダー
        List<string> headers = new List<string>();
        for (int i = 0; i < results.Count; i++)
            headers.Add($"Q{i + 1}_result");
        lines.Add(string.Join(",", headers));

        // 結果
        lines.Add(string.Join(",", results));

        File.WriteAllLines(filePath, lines);
        Debug.Log("クイズ結果を保存: " + filePath);
    }
}
