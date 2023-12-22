using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SaveGame : MonoBehaviour
{
    /*public static List<Tuple<string, float, float, float>> saveBalls = new();*/

    public static void SaveGames()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + $"/{PlayerUI.player1Name}_{PlayerUI.player2Name}.dat");

        List<GameObject> balls = new List<GameObject>();
        GameObject[] half_full = GameObject.FindGameObjectsWithTag("half_full");
        GameObject[] full = GameObject.FindGameObjectsWithTag("full");
        GameObject[] lost = GameObject.FindGameObjectsWithTag("lost");
        balls.AddRange(half_full);
        balls.AddRange(full);
        balls.AddRange(lost);

        SaveData data = new SaveData();

        foreach (GameObject ball in balls)
        {
            if (ball != null)
            {
                Vector3 pos = ball.transform.position;
                data.saveBalls.Add(Tuple.Create(ball.name, pos.x, pos.y, pos.z));
            }
        }

        data.savePlayer1Name = PlayerUI.player1Name;
        data.savePlayer2Name = PlayerUI.player2Name;
        data.savePlayer1TypeBall = PlayerUI.player1TypeBalls;
        data.savePlayer2TypeBall = PlayerUI.player2TypeBalls;
        data.saveScore1 = Score.score1;
        data.saveScore2 = Score.score2;
        data.saveOpponentBot = GlobalParam._opponentBot;
        data.saveTurn = PlayerUI.turn;
        data.saveNeedPutMainBall = BallController.needPutMainBall;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public static void LoadGame(FileStream fileLoad)
    {
        if (File.Exists(fileLoad.Name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            /*fileLoad = File.Open(fileLoad.Name, FileMode.Open);*/

            SaveData data = (SaveData)bf.Deserialize(fileLoad);
            fileLoad.Close();

            List<GameObject> balls = new List<GameObject>();
            GameObject[] half_full = GameObject.FindGameObjectsWithTag("half_full");
            GameObject[] full = GameObject.FindGameObjectsWithTag("full");
            GameObject[] lost = GameObject.FindGameObjectsWithTag("lost");
            balls.AddRange(half_full);
            balls.AddRange(full);
            balls.AddRange(lost);

            foreach (GameObject ball in balls)
            {
                bool flag = false;
                foreach (Tuple<string, float, float, float> saveBall in data.saveBalls) {
                    if (saveBall.Item1 == ball.name)
                    {
                        flag = true;
                        Vector3 pos = BallController.mainBall.transform.position;
                        pos.x = saveBall.Item2;
                        pos.y = saveBall.Item3;
                        pos.z = saveBall.Item4;
                    }
                }
                if (!flag)
                {
                    Destroy(ball);
                }
            }

            PlayerUI.player1Name = data.savePlayer1Name;
            PlayerUI.player2Name = data.savePlayer2Name;
            PlayerUI.player1TypeBalls = data.savePlayer1TypeBall;
            PlayerUI.player2TypeBalls = data.savePlayer2TypeBall;
            Score.score1 = data.saveScore1;
            Score.score2 = data.saveScore2;
            GlobalParam._opponentBot = data.saveOpponentBot;
            PlayerUI.turn = data.saveTurn;
            BallController.needPutMainBall = data.saveNeedPutMainBall;

            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }
}

class SaveData
{
    public List<Tuple<string, float, float, float>> saveBalls;
    public string savePlayer1Name;
    public string savePlayer2Name;
    public string savePlayer1TypeBall;
    public string savePlayer2TypeBall;
    public int saveScore1;
    public int saveScore2;
    public bool saveOpponentBot;
    public bool saveTurn;
    public bool saveNeedPutMainBall;
}
