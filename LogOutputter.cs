using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// シーン中に発生したエラーやワーニング、ログなどが、コールバックされる
// 自分で発生させたデバッグログなどもここに入る
// TODO : シーン上にこのスクリプトを追加
// TODO : アセットフォルダー上にlogフォルダを作成（ビルド後はDataフォルダ内にlogフォルダを作成）
public class LogOutputter : MonoBehaviour
{
    private string filename = "log.txt";
    private string foldername = "log";

    void Awake()
    {
        // ログが書き出された時のコールバック設定
        Application.logMessageReceived += LogCallback;
    }

    /// <summary>
    /// ログを取得するコールバック
    /// </summary>
    /// <param name="condition">メッセージ</param>
    /// <param name="stackTrace">コールスタック</param>
    /// <param name="type">ログの種類</param>
    public void LogCallback(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
            case LogType.Warning:
                // 何もしない
                break;
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                // ログを出力(ビルド後は、スラックへの通知やシーンの初期化を行う)
                OutputLog(condition, stackTrace);
#if !UNITY_EDITOR
                SendLogForSlack(condition, stackTrace);
                InitScene();
#endif
                break;
        }
    }

    /// <summary>
    /// ログを外部ファイルに保存する
    /// </summary>
    /// <param name="conditionStr">メッセージ</param>
    /// <param name="stackTraceStr">コールスタック</param>
    public void OutputLog(string conditionStr, string stackTraceStr)
    {
        // 日付を取得
        DateTime todayNow = DateTime.Now;
        string dateStr = todayNow.ToString("yyyy-MM-dd-");

        // ファイルパスを取得
        string path = Application.dataPath + "/" + foldername + "/" + dateStr + filename;

        // ファイルの存在チェック
        FileStream file = null;
        if (!File.Exists(path))
        {
            // 存在しなかったので、ファイルを作成
            try
            {
                file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (IOException e)
            {
                //Debug.Log(e.Message);
            }
            finally
            {
                if (file != null)
                {
                    try
                    {
                        file.Dispose();
                    }
                    catch (IOException e2)
                    {
                        //Debug.Log(e2.Message);
                    }
                }
            }
        }

        // ファイルへ書き込み
        string dateTimeStr = todayNow.ToString("yyyy/MM/dd HH:mm:ss");
        string outputStr = dateTimeStr + "\n" + conditionStr + "\n" + stackTraceStr + "\n";
        File.AppendAllText(path, outputStr);
    }


    /// <summary>
    /// スラックへの通知
    /// </summary>
    /// <param name="conditionStr">メッセージ</param>
    /// <param name="stackTraceStr">コールスタック</param>
    private void SendLogForSlack(string conditionStr, string stackTraceStr)
    {
        // スラック通知
        //sendSlackMessage.CreateErrorMessage(stackTraceStr, conditionStr);
    }

    /// <summary>
    /// シーンの初期化
    /// </summary>
    private void InitScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}