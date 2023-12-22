using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour 
{

    // Лист забитых шаров
    public static List<string> colliders;

    // Очки забитых шаров игроков
    public static int score1;
    public static int score2;

    private void Start()
    {
        colliders = new List<string>();
    }

    // Если обнаружено попадание какого-либо шара в лузу
    void OnTriggerEnter(Collider other)
    {
        // Если забит биток
        if (other.gameObject.CompareTag("Player")) 
        {
            // Добавляем в лист забитых шаров
            colliders.Add(other.gameObject.tag);

            // Телепортируем биток в часть карты, не видную игрокам и обнуляем его скорость
            BallController.mainBall.transform.position = new Vector3(200f, 5f, 200f);
            BallController.mainBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BallController.mainBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            Debug.Log("Main ball in hole");
        }

        // Если забит черный шар
        if (other.gameObject.CompareTag("lost")) 
        {
            // Добавляем в лист забитых шаров 
            colliders.Add(other.gameObject.tag);

            // Удаляем черный шар со сцены
            Destroy(other.gameObject);

            Debug.Log("Black ball in hole");
        }

        // Если забит полый шар
        if (other.gameObject.CompareTag("full")) {
            // Добавляем в лист забитых шаров 
            colliders.Add(other.gameObject.tag);

            // Удаляем черный шар со сцены
            Destroy(other.gameObject);

            // Если ходит первый игрок
            if (PlayerUI.turn)
            {
                // То если типы шаров не определены и первый был забит полый шар (условия для того, чтобы определить,
                // кому уйдет очко еще до распределения типов шаров между игроками)
                if (PlayerUI.player1TypeBalls == "-" && colliders[0] == "full")
                {
                    // То очко первому игроку
                    score1++;
                }
                // Иначе если забит полый шар
                else if (PlayerUI.player1TypeBalls == "full")
                {
                    // То очко первому игроку
                    score1++;
                }
                else
                {
                    // Иначе очко второму игроку
                    score2++;
                }
            }
            // Иначе ходит второй игрок
            else
            {
                // То если типы шаров не определены и первый был забит полый шар (условия для того, чтобы определить,
                // кому уйдет очко еще до распределения типов шаров между игроками)
                if (PlayerUI.player2TypeBalls == "-" && colliders[0] == "full")
                {
                    // То очко второму игроку
                    score2++;
                }
                // Иначе если забит полый шар
                else if (PlayerUI.player2TypeBalls == "full")
                {
                    // То очко второму игроку
                    score2++;
                }
                else
                {
                    // Иначе очко первому игроку
                    score1++;
                }
            }

            Debug.Log("full in hole: " + score1.ToString());
        }

        // Если забит полосатый шар
        if (other.gameObject.CompareTag("half_full")) {
            // Добавляем в лист забитых шаров 
            colliders.Add(other.gameObject.tag);

            // Удаляем черный шар со сцены
            Destroy(other.gameObject);

            // Если ходит первый игрок
            if (PlayerUI.turn)
            {
                // То если типы шаров не определены и первый был забит полосатый шар (условия для того, чтобы определить,
                // кому уйдет очко еще до распределения типов шаров между игроками)
                if (PlayerUI.player1TypeBalls == "-" && colliders[0] == "half_full")
                {
                    // То очко первому игроку
                    score1++;
                }
                // Иначе если забит полосатый шар
                else if (PlayerUI.player1TypeBalls == "half_full")
                {
                    // То очко первому игроку
                    score1++;
                }
                else
                {
                    // Иначе очко второму игроку
                    score2++;
                }
            }
            // Иначе ходит второй игрок
            else
            {
                // То если типы шаров не определены и первый был забит полосатый шар (условия для того, чтобы определить,
                // кому уйдет очко еще до распределения типов шаров между игроками)
                if (PlayerUI.player2TypeBalls == "-" && colliders[0] == "half_full")
                {
                    // То очко второму игроку
                    score2++;
                }
                // Иначе если забит полосатый шар
                else if (PlayerUI.player2TypeBalls == "half_full")
                {
                    // То очко второму игроку
                    score2++;
                }
                else
                {
                    // Иначе очко второму игроку
                    score1++;
                }
            }

            Debug.Log("half_full in hole: " + score2.ToString());
        }
    }

}
