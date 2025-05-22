using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamestart : MonoBehaviour 
{
    private bool firstPush = false;

    public void PressStart() 
    { 
        // １度もゲームスタートボタンが押されていない場合、画面遷移
        if (!firstPush)
        {
            SceneManager.LoadScene("quiz"); 
            firstPush = true;

