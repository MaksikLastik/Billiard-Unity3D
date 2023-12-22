using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour
{

    private static bool firstCollisionIsMyBall;                 // Первый задетый шар моего ли типа
    private static bool isScoredMainBall;                       // Забит ли биток
    private static bool isScoredBlackBall;                      // Забит ли черный шар
    private static bool areScoredAllMyBall;                     // На данном этапе забил ли я все шары своего типа
    private static bool isScoredEvenOneMyBall;                  // забит ли хотя бы один шар моего типа
    private static bool isScoredAnyBall;                        // Забит ли любой шар, кроме черного
    private static bool isCollisionAnyBall;                     // Было ли столкновение с любым шаром
    private static bool isScoredEvenOneBall;                    // На данный момент забит ли хотя бы один шар одним из игроком (распределены ли типы шаров для каждого игрока)
    private static bool isScoredEvenOneBallLastTime = false;    // В прошлый раз был ли забит первый мяч

    // Вычисление булевских переменных
    public static void CalculationBool()
    {
        // Узнаем значение isScoredMainBall и isScoredBlackBall
        // Заранее ставишь значения false, т.к. биток и черный шар могут быть не забиты за ход
        isScoredMainBall = false;
        isScoredBlackBall = false;
        // Если в листе забитых шаров что-то есть
        if (Score.colliders.Count > 0) {
            // Цикл поиска забитых битка и черного шара
            foreach (string collider in Score.colliders)
            {
                // Если в листе забитых шаров найден биток
                if (collider == "Player")
                {
                    isScoredMainBall = true;
                }
                // Иначе если в листе забитых шаров найден черный шар
                else if (collider == "lost")
                {
                    isScoredBlackBall = true;
                }
            }
        }

        // Узнаем значение areScoredAllMyBall
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

        // Узнаем значение isScoredAnyBallExceptBlackBall
        if (Score.colliders.Count > 0)
        {
            isScoredAnyBall = true;
        }
        else
        {
            isScoredAnyBall = false;
        }

        // Узнаем значение isCollisionAnyBall
        if (BallController.collisions.Count > 0)
        {
            isCollisionAnyBall = true;
        }
        else 
        { 
            isCollisionAnyBall = false; 
        }

        // Узнаем значение isScoredEvenOneBall
        if ((Score.score1 + Score.score2) > 0)
        {
            isScoredEvenOneBall = true;
        }
        else
        {
            isScoredEvenOneBall = false;
        }

        // Узнаем значение firstCollisionMyBall
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

            // Узнаем значение isScoredEvenOneMyBall;
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

    // Выход исходов в зависимости от определённых условий
    public static void RulesBalls()
    {
        // Вызываем функцию для подсчета логических переменных
        CalculationBool();
        // Если игроки не забили еще ни одного шара, не считая битка
        // Идет расчет во время, когда еще не определены типы шаров
        if (!isScoredEvenOneBall)
        {
            // Если не было ни одного касания по другому шару битком или биток забили в лузу
            if (!isCollisionAnyBall || isScoredMainBall)
            {
                // То переход хода с последующим передвижением битка
                ChaingingTurnAndPutMainBall();
            }
            else
            {
                // Иначе только переход хода
                ChaingingTurn();
            }
        }
        // Иначе если в прошлом ходе еще не было забито ни одного шара, не считая битка
        // Идет расчет, что мы забили первый(-ые) шар(-ы), кроме битка
        else if (!isScoredEvenOneBallLastTime)
        {
            // Инвертируем эту пременную, так как каждый прошлый ход относительно любого последующего
            // хода будет true, так как хотя бы один шар был забит
            isScoredEvenOneBallLastTime = true;
            // Если забит черный шар
            if (isScoredBlackBall)
            {
                // Победил соперник
                WinOpponent();
            }
            // Иначе если забит любой шар, кроме черного
            else if (isScoredAnyBall)
            {
                // Если забит биток
                if (isScoredMainBall)
                {
                    // Распределяем типы шаров для игроков и переход хода с последующим передвижением битка
                    DistributionBallTypes();
                    ChaingingTurnAndPutMainBall();
                }
                // Иначе распределяем типы шаров для игроков, игрок продолжает ход
                else
                {
                    DistributionBallTypes();
                    DisplayLine();
                    return;
                }
            }
        }
        // Иначе если не было ни одного касания битка о любой шар
        // Идет расчет, что уже определены типы шаров
        else if (!isCollisionAnyBall)
        {
            // То переход хода с последующим передвижением битка
            ChaingingTurnAndPutMainBall();
        }
        // Иначе если забит черный шар
        else if (isScoredBlackBall)
        {
            // Если забиты все шары типа игрока, который сейчас ходит
            if (areScoredAllMyBall)
            {
                // Если забит белый шар
                if (isScoredMainBall)
                {
                    // То победит соперник
                    WinOpponent();
                }
                // Иначе победил ходивший игрок
                else
                {
                    MyWin();
                }
            }
            // Иначе победил соперник
            else
            {
                WinOpponent();
            }
        }
        // Иначе если забит биток
        else if (isScoredMainBall) {
            // То переход хода с последующим передвижением битка
            ChaingingTurnAndPutMainBall();
        }
        // Иначе если первое касание того же типа, что и у ходившего игрока
        else if (firstCollisionIsMyBall)
        {
            // Если ходивший игрок забил хотя бы один свой шар
            if (isScoredEvenOneMyBall)
            {
                // То игрок продолжает ход
                DisplayLine();
                return;
            }
            // Иначе переход хода
            else
            {
                ChaingingTurn();
            }
        }
        // Иначе переход хода с последующим передвижением битка
        else
        {
            ChaingingTurnAndPutMainBall();
        }
    }

    // Отобразить линии в зависимостси от хода и режима игры
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

    // Исходы при определённых условиях

    // Смена хода
    public static void ChaingingTurn()
    {
        PlayerUI.turn = !PlayerUI.turn;
        DisplayLine();
    }

    // Смена хода + постановка битка
    public static void ChaingingTurnAndPutMainBall()
    {
        PlayerUI.turn = !PlayerUI.turn;
        BallController.needPutMainBall = true;
    }

    // Победа ходившего игрока
    public static void MyWin()
    {
        // Не даем продолжать играть
        BallController.playing = false;

        // Если победил первый игрок
        if (PlayerUI.turn)
        {
            // То заполняем информацию о победе первого игрока
            PlayerUI._winnerInfo.text = PlayerUI.player1Name + " won";
        }
        else
        {
            // Иначе заполняем информацию о победе второго игрока
            PlayerUI._winnerInfo.text = PlayerUI.player2Name + " won";
        }

        // Отключаем скрипт управления битком
        BallController.butExit.interactable = true;
        BallController.mainBall.GetComponent<BallController>().enabled = false;
    }

    // Победа соперника
    public static void WinOpponent()
    {
        // Не даем продолжать играть
        BallController.playing = false;

        // Если победил первый игрок
        if (PlayerUI.turn)
        {
            // То заполняем информацию о победе первого игрока
            PlayerUI._winnerInfo.text = PlayerUI.player2Name + " won";
        }
        else
        {
            // Иначе заполняем информацию о победе второго игрока
            PlayerUI._winnerInfo.text = PlayerUI.player1Name + " won";
        }

        // Отключаем скрипт управления битком
        BallController.butExit.interactable = true;
        BallController.mainBall.GetComponent<BallController>().enabled = false;
    }

    // Закрепить типы шаров за игроками
    public static void DistributionBallTypes()
    {
        // Если ходит первый игрок, а первое касание было полого шара
        if (PlayerUI.turn && Score.colliders[0] == "full")
        {
            // То для первого игрока - полые шары, для второго - полосатые
            PlayerUI.player1TypeBalls = "full";
            PlayerUI.player2TypeBalls = "half_full";
        }
        // Если ходит второй игрок, а первое касание было полого шара
        else if (!PlayerUI.turn && Score.colliders[0] == "full")
        {
            // То для первого игрока - полсатые шары, для второго - полые
            PlayerUI.player1TypeBalls = "half_full";
            PlayerUI.player2TypeBalls = "full";
        }
        // Если ходит первый игрок, а первое касание было полосатого шара
        else if (PlayerUI.turn && Score.colliders[0] == "half_full")
        {
            // То для первого игрока - полсатые шары, для второго - полые
            PlayerUI.player1TypeBalls = "half_full";
            PlayerUI.player2TypeBalls = "full";
        }
        // Если ходит второй игрок, а первое касание было полосатого шара
        else if (!PlayerUI.turn && Score.colliders[0] == "half_full")
        {
            // То для первого игрока - полые шары, для второго - полосатые
            PlayerUI.player1TypeBalls = "full";
            PlayerUI.player2TypeBalls = "half_full";
        }
    }

    // Очистить листы столкновений и забитых шаров
    public static void ClearCollisionsAndColliders()
    {
        BallController.collisions.Clear();
        BallController.collisions = new List<string>();

        Score.colliders.Clear();
        Score.colliders = new List<string>();
    }
}
