//CSVファイルを読み込んでクイズを作るよ！一行目に問題文　二行目に答え　3~6行目に選択肢を入れる形式のファイルにしてね！！//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class question : MonoBehaviour
{
    TextAsset csvFile; // CSVファイル
    public static List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;  
    public static List<int> qnumber = new List<int>(); // 出題番号リスト;     
    public int csvrow; // CSVファイルの行数
    public static string answer; // クイズの答え   
    private int k = 0; //配列の変数
    private int qransu = 0; // 出題する問題の行      

    void Start()
    {
        int qcount = gamestart.getqCount();
        if(qcount == 0) // １問目のみ以下処理を行う
        {
            csvFile = Resources.Load("questiondata") as TextAsset; // Resouces下のCSV読み込み
            StringReader reader = new StringReader(csvFile.text); 
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                string line = reader.ReadLine(); // 一行ずつ読み込み
                csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
                csvrow++; // CSVファイルの行数カウント
            }           
            for (int j = 1; j <= csvrow-1; j++) 
            {
                qnumber.Add(j); // CSVファイルの行をリストに追加（1行目は除外）
            }
            qnumber = qnumber.OrderBy ( a => System.Guid.NewGuid () ).ToList (); // 読み込んだリストをシャッフル 
            csvrow = 0; //変数初期化
        }        
        qransu = qnumber[qcount]; // シャッフルされたリストを問題数順に取得
        QuestionLabelSet();
        AnswerLabelSet();
    }

    private void QuestionLabelSet()
    {    
        csvDatas[k] = csvDatas[qransu]; // CSVの"qransu"行目の問題を取得      
        Text qLabel = GameObject.Find("Canvas/Question").GetComponent<Text> ();
        qLabel.text = csvDatas[k][0];  
    }

    private void AnswerLabelSet()
    {
        //回答文面の作成
        string[] array = new string[] { csvDatas[k][2], csvDatas[k][3], csvDatas[k][4], csvDatas[k][5] };
        array = array.OrderBy(x => System.Guid.NewGuid()).ToArray(); // 回答候補のリストをシャッフル
        //ボタンが4つあるのでそれぞれ代入
        for (int i=1; i<=4 ; i++)
        {
            Text aLabel = GameObject.Find("Canvas/Button" + i).GetComponentInChildren<Text> ();
            aLabel.text = array[i-1];
            answer = csvDatas[k][1];
        }       
    }
}
