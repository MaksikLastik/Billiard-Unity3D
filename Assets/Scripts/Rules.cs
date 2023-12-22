using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour
{

    private static bool firstCollisionIsMyBall;                 // ������ ������� ��� ����� �� ����
    private static bool isScoredMainBall;                       // ����� �� �����
    private static bool isScoredBlackBall;                      // ����� �� ������ ���
    private static bool areScoredAllMyBall;                     // �� ������ ����� ����� �� � ��� ���� ������ ����
    private static bool isScoredEvenOneMyBall;                  // ����� �� ���� �� ���� ��� ����� ����
    private static bool isScoredAnyBall;                        // ����� �� ����� ���, ����� �������
    private static bool isCollisionAnyBall;                     // ���� �� ������������ � ����� �����
    private static bool isScoredEvenOneBall;                    // �� ������ ������ ����� �� ���� �� ���� ��� ����� �� ������� (������������ �� ���� ����� ��� ������� ������)
    private static bool isScoredEvenOneBallLastTime = false;    // � ������� ��� ��� �� ����� ������ ���

    // ���������� ��������� ����������
    public static void CalculationBool()
    {
        // ������ �������� isScoredMainBall � isScoredBlackBall
        // ������� ������� �������� false, �.�. ����� � ������ ��� ����� ���� �� ������ �� ���
        isScoredMainBall = false;
        isScoredBlackBall = false;
        // ���� � ����� ������� ����� ���-�� ����
        if (Score.colliders.Count > 0) {
            // ���� ������ ������� ����� � ������� ����
            foreach (string collider in Score.colliders)
            {
                // ���� � ����� ������� ����� ������ �����
                if (collider == "Player")
                {
                    isScoredMainBall = true;
                }
                // ����� ���� � ����� ������� ����� ������ ������ ���
                else if (collider == "lost")
                {
                    isScoredBlackBall = true;
                }
            }
        }

        // ������ �������� areScoredAllMyBall
        //
        if (PlayerUI.turn && Score.score1 == 7 ||
           !PlayerUI.turn && Score.score2 == 7)
        {
            areScoredAllMyBall = true;
        }
        else
        {
            areScoredAllMyBall = false;
        }

        // ������ �������� isScoredAnyBallExceptBlackBall
        if (Score.colliders.Count > 0)
        {
            isScoredAnyBall = true;
        }
        else
        {
            isScoredAnyBall = false;
        }

        // ������ �������� isCollisionAnyBall
        if (BallController.collisions.Count > 0)
        {
            isCollisionAnyBall = true;
        }
        else 
        { 
            isCollisionAnyBall = false; 
        }

        // ������ �������� isScoredEvenOneBall
        if ((Score.score1 + Score.score2) > 0)
        {
            isScoredEvenOneBall = true;
        }
        else
        {
            isScoredEvenOneBall = false;
        }

        // ������ �������� firstCollisionMyBall
        if (isScoredEvenOneBall) {
            if (BallController.collisions.Count > 0)
            {
                if (BallController.collisions[0] == $"{PlayerUI.player1TypeBalls}" && PlayerUI.turn ||
                    BallController.collisions[0] == $"{PlayerUI.player2TypeBalls}" && !PlayerUI.turn ||
                    PlayerUI.turn && Score.score1 == 7 && BallController.collisions[0] == "lost" ||
                    !PlayerUI.turn && Score.score2 == 7 && BallController.collisions[0] == "lost")
                {
                    firstCollisionIsMyBall = true;
                }
                else
                {
                    firstCollisionIsMyBall = false;
                }
            }
            else
            {
                firstCollisionIsMyBall = false;
            }

            // ������ �������� isScoredEvenOneMyBall;
            if (Score.colliders.Count > 0)
            {
                foreach (string collider in Score.colliders)
                {
                    if (collider == $"{PlayerUI.player1TypeBalls}" && PlayerUI.turn ||
                        collider == $"{PlayerUI.player2TypeBalls}" && !PlayerUI.turn)
                    {
                        isScoredEvenOneMyBall = true;
                        break;
                    }
                }
            }
            else
            {
                isScoredEvenOneMyBall = false;
            }
        }
        else
        {
            firstCollisionIsMyBall = false;
            isScoredEvenOneMyBall = false;
        }
    } 

    // ����� ������� � ����������� �� ����������� �������
    public static void RulesBalls()
    {
        // �������� ������� ��� �������� ���������� ����������
        CalculationBool();
        // ���� ������ �� ������ ��� �� ������ ����, �� ������ �����
        // ���� ������ �� �����, ����� ��� �� ���������� ���� �����
        if (!isScoredEvenOneBall)
        {
            // ���� �� ���� �� ������ ������� �� ������� ���� ������ ��� ����� ������ � ����
            if (!isCollisionAnyBall || isScoredMainBall)
            {
                // �� ������� ���� � ����������� ������������� �����
                ChaingingTurnAndPutMainBall();
            }
            else
            {
                // ����� ������ ������� ����
                ChaingingTurn();
            }
        }
        // ����� ���� � ������� ���� ��� �� ���� ������ �� ������ ����, �� ������ �����
        // ���� ������, ��� �� ������ ������(-��) ���(-�), ����� �����
        else if (!isScoredEvenOneBallLastTime)
        {
            // ����������� ��� ���������, ��� ��� ������ ������� ��� ������������ ������ ������������
            // ���� ����� true, ��� ��� ���� �� ���� ��� ��� �����
            isScoredEvenOneBallLastTime = true;
            // ���� ����� ������ ���
            if (isScoredBlackBall)
            {
                // ������� ��������
                WinOpponent();
            }
            // ����� ���� ����� ����� ���, ����� �������
            else if (isScoredAnyBall)
            {
                // ���� ����� �����
                if (isScoredMainBall)
                {
                    // ������������ ���� ����� ��� ������� � ������� ���� � ����������� ������������� �����
                    DistributionBallTypes();
                    ChaingingTurnAndPutMainBall();
                }
                // ����� ������������ ���� ����� ��� �������, ����� ���������� ���
                else
                {
                    DistributionBallTypes();
                    DisplayLine();
                    return;
                }
            }
        }
        // ����� ���� �� ���� �� ������ ������� ����� � ����� ���
        // ���� ������, ��� ��� ���������� ���� �����
        else if (!isCollisionAnyBall)
        {
            // �� ������� ���� � ����������� ������������� �����
            ChaingingTurnAndPutMainBall();
        }
        // ����� ���� ����� ������ ���
        else if (isScoredBlackBall)
        {
            // ���� ������ ��� ���� ���� ������, ������� ������ �����
            if (areScoredAllMyBall)
            {
                // ���� ����� ����� ���
                if (isScoredMainBall)
                {
                    // �� ������� ��������
                    WinOpponent();
                }
                // ����� ������� �������� �����
                else
                {
                    MyWin();
                }
            }
            // ����� ������� ��������
            else
            {
                WinOpponent();
            }
        }
        // ����� ���� ����� �����
        else if (isScoredMainBall) {
            // �� ������� ���� � ����������� ������������� �����
            ChaingingTurnAndPutMainBall();
        }
        // ����� ���� ������ ������� ���� �� ����, ��� � � ��������� ������
        else if (firstCollisionIsMyBall)
        {
            // ���� �������� ����� ����� ���� �� ���� ���� ���
            if (isScoredEvenOneMyBall)
            {
                // �� ����� ���������� ���
                DisplayLine();
                return;
            }
            // ����� ������� ����
            else
            {
                ChaingingTurn();
            }
        }
        // ����� ������� ���� � ����������� ������������� �����
        else
        {
            ChaingingTurnAndPutMainBall();
        }
    }

    // ���������� ����� � ������������ �� ���� � ������ ����
    public static void DisplayLine() 
    {
        if (GlobalParam._opponentBot)
        {
            if (PlayerUI.turn)
            {
                BallController.mainBall.GetComponent<LineRenderer>().enabled = true;
            }
            else
            {
                BallController.mainBall.GetComponent<LineRenderer>().enabled = false;
            }
        }
        else
        {
            BallController.mainBall.GetComponent<LineRenderer>().enabled = true;
        }
    }

    // ������ ��� ����������� ��������

    // ����� ����
    public static void ChaingingTurn()
    {
        PlayerUI.turn = !PlayerUI.turn;
        DisplayLine();
    }

    // ����� ���� + ���������� �����
    public static void ChaingingTurnAndPutMainBall()
    {
        PlayerUI.turn = !PlayerUI.turn;
        BallController.needPutMainBall = true;
    }

    // ������ ��������� ������
    public static void MyWin()
    {
        // �� ���� ���������� ������
        BallController.playing = false;

        // ���� ������� ������ �����
        if (PlayerUI.turn)
        {
            // �� ��������� ���������� � ������ ������� ������
            PlayerUI._winnerInfo.text = PlayerUI.player1Name + " won";
        }
        else
        {
            // ����� ��������� ���������� � ������ ������� ������
            PlayerUI._winnerInfo.text = PlayerUI.player2Name + " won";
        }

        // ��������� ������ ���������� ������
        BallController.butExit.interactable = true;
        BallController.mainBall.GetComponent<BallController>().enabled = false;
    }

    // ������ ���������
    public static void WinOpponent()
    {
        // �� ���� ���������� ������
        BallController.playing = false;

        // ���� ������� ������ �����
        if (PlayerUI.turn)
        {
            // �� ��������� ���������� � ������ ������� ������
            PlayerUI._winnerInfo.text = PlayerUI.player2Name + " won";
        }
        else
        {
            // ����� ��������� ���������� � ������ ������� ������
            PlayerUI._winnerInfo.text = PlayerUI.player1Name + " won";
        }

        // ��������� ������ ���������� ������
        BallController.butExit.interactable = true;
        BallController.mainBall.GetComponent<BallController>().enabled = false;
    }

    // ��������� ���� ����� �� ��������
    public static void DistributionBallTypes()
    {
        // ���� ����� ������ �����, � ������ ������� ���� ������ ����
        if (PlayerUI.turn && Score.colliders[0] == "full")
        {
            // �� ��� ������� ������ - ����� ����, ��� ������� - ���������
            PlayerUI.player1TypeBalls = "full";
            PlayerUI.player2TypeBalls = "half_full";
        }
        // ���� ����� ������ �����, � ������ ������� ���� ������ ����
        else if (!PlayerUI.turn && Score.colliders[0] == "full")
        {
            // �� ��� ������� ������ - �������� ����, ��� ������� - �����
            PlayerUI.player1TypeBalls = "half_full";
            PlayerUI.player2TypeBalls = "full";
        }
        // ���� ����� ������ �����, � ������ ������� ���� ���������� ����
        else if (PlayerUI.turn && Score.colliders[0] == "half_full")
        {
            // �� ��� ������� ������ - �������� ����, ��� ������� - �����
            PlayerUI.player1TypeBalls = "half_full";
            PlayerUI.player2TypeBalls = "full";
        }
        // ���� ����� ������ �����, � ������ ������� ���� ���������� ����
        else if (!PlayerUI.turn && Score.colliders[0] == "half_full")
        {
            // �� ��� ������� ������ - ����� ����, ��� ������� - ���������
            PlayerUI.player1TypeBalls = "full";
            PlayerUI.player2TypeBalls = "half_full";
        }
    }

    // �������� ����� ������������ � ������� �����
    public static void ClearCollisionsAndColliders()
    {
        BallController.collisions.Clear();
        BallController.collisions = new List<string>();

        Score.colliders.Clear();
        Score.colliders = new List<string>();
    }
}
