using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    // Очередь хода (true - первй игрок, false - второй игрок)
    public static bool turn = true;

    // Типы шаров игроков
    public static string player1TypeBalls;
    public static string player2TypeBalls;

    // Имена игроков
    public static string player1Name;
    public static string player2Name;

    // Общий текст информации о состоянии очков и типов шров каждого игрока (для работы с другими скриптами)
    public static Text _player1Text;
    public static Text _player2Text;
    // Общий текст информации о состоянии очков и типов шров каждого игрока (привязаны к объектам)
    public Text player1Text;
    public Text player2Text;

    // Всплывающее сообщение информации о победе или просьбе поставить биток (для работы с другими скриптами)
    public static Text _winnerInfo;
    // Всплывающее сообщение информации о победе или просьбе поставить биток (привязан к объекту)
    public Text winnerInfo;

    public void Start()
    {
        // Сбрасываем типы шаров
        player1TypeBalls = player2TypeBalls = "-";

        // Инициализируем буферные переменные
        _player1Text = player1Text;
        _player2Text = player2Text;
        _winnerInfo = winnerInfo;

        // Если при запуске игры имя первого игрока оказалось не пустым
        if (GlobalParam._player1Name.Length != 0)
        {
            // То присваиваем ему его имя из глобального поля
            player1Name = GlobalParam._player1Name;
        }
        // Иначе если играем против бота
        else if (GlobalParam._opponentBot)
        {
            // То присваиваем ему имя "Player"
            player1Name = "Player";
        }
        else
        {
            // Иначе присваиваем ему имя "Player 1"
            player1Name = "Player 1";
        }

        // Если при запуске игры имя первого игрока оказалось не пустым
        if (GlobalParam._player2Name.Length != 0)
        {
            // То присваиваем ему его имя из глобального поля
            player2Name = GlobalParam._player2Name;
        }
        // Иначе если играем против бота
        else if (GlobalParam._opponentBot)
        {
            // То присваиваем ему имя "Bot"
            player2Name = "Bot";
        }
        else
        {
            // Иначе присваиваем ему имя "Player 2"
            player2Name = "Player 2";
        }

        // Обновляем интерфейс
        UpdateUI();
    }

    public void Update()
    {
        // Постоянно обновляем интерфейс
        player1Text = _player1Text;
        player2Text = _player2Text;
        winnerInfo = _winnerInfo;
        player1Text.color = _player1Text.color;
        player2Text.color = _player2Text.color;
    }

    public static void UpdateUI()
    {
        // Если ходит первый игрок, то он подсвечивается красным, а второй - белым
        if (turn)
        {
            _player1Text.color = Color.red;
            _player2Text.color = Color.white;
        }
        // Иначе ходит второй игрок подсвечивается красным, а первый - белым
        else
        {
            _player1Text.color = Color.white;
            _player2Text.color = Color.red;
        }

        // Заполняем информацию для каждого игрока
        _player1Text.text = player1Name + " (" + player1TypeBalls + "): " + Score.score1.ToString();
        _player2Text.text = player2Name + " (" + player2TypeBalls + "): " + Score.score2.ToString();
    }
}