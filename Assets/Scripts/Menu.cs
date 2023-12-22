using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{

    public InputField p1nwf;        // Имя первого игрока - GameWithFriend
    public InputField p2nwf;        // Имя второго игрока - GameWithFriend
    public InputField p1nwb;        // Имя игрока - GameWithBot
    public InputField p2nwb;        // Имя бота - GameWithBot

    public void StartGameWithFriend()
    {
        // Переносим имена в глобальное поле
        GlobalParam._player1Name = p1nwf.text;
        GlobalParam._player2Name = p2nwf.text;

        // Устанавливаем игру с человеком, переносим в глобальное поле
        GlobalParam._opponentBot = false;

        // Устанавливаем очередь хода с первого игрока
        PlayerUI.turn = true;

        // Загружаем сцену игры
        SceneManager.LoadScene("Billiard");
    }

    public void StartGameWithBot()
    {
        // Переносим имена в глобальное поле
        GlobalParam._player1Name = p1nwb.text;
        GlobalParam._player2Name = p2nwb.text;

        // Устанавливаем игру с ботом, переносим в глобальное поле
        GlobalParam._opponentBot = true;

        // Устанавливаем очередь хода с первого игрока
        PlayerUI.turn = true;

        // Загружаем сцену игры
        SceneManager.LoadScene("Billiard");
    }

    public void LoadGame()
    {
        FileStream fileS = File.Open(Application.persistentDataPath.ToString() + "/Player 1_Player 2.dat", FileMode.Open);
        SceneManager.LoadScene("Billiard");
        SaveGame.LoadGame(fileS);
    }

    public void ExitGame() 
    { 
        // Выход из игры
        Application.Quit(); 
    }

    public void ExitToMenu()
    {
        SaveGame.SaveGames();
        // Загружаем главное меню
        SceneManager.LoadScene("Menu");

        // Сбрасываем имена в глобальном поле
        GlobalParam._player1Name = "-";
        GlobalParam._player2Name = "-";

        // Обнуляем очки забитых шаров
        Score.score1 = Score.score2 = 0;

        // Очищаем всплывающее сообщение
        PlayerUI._winnerInfo.text = "";
    }

}
