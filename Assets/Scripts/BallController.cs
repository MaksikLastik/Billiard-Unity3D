using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour 
{

    public static GameObject mainBall;              // Биток
    public static List<string> collisions;          // Лист столкновений
    public static Button butExit;                   // Кнопка выхода
    private static Camera cam;                      // Камера с видо сверху

    public static bool needPutMainBall = false;     // Нужно ли поставить биток
    public static bool playing = true;              // Играем ли дальше
    private static bool abilityHit = true;           // Можно ли нажать пробел для выполения удара
    private static bool turnEnded = true;           // Очередь игрока закончена
    private static bool pause = false;              // Пауза для регистрации движения шаров после удара бота

    public float addSpeed;                          // Дополннительный коэффициент к удару по битку
    public float maxForce;                          // Максимальная сила удара
    public Vector3 lineDirection;                   // Направление линии удара
    public float lineLength;                        // Длина линии удара
    public float rotationSpeed;                     // Скорость вращения линии
    public float lengthChangeSpeed;                 // Скорость изменения линии удара

    private float horizontalInput;                  // Горизональный ввод 
    private float verticalInput;                    // Вертикальный ввод
    private Vector3 endLine;                        // Конечная точка линии удара

    // Инициализация всех ранее объявленных переменных перед работой постоянной функции FixedUpdate
    void Start()
    {
        mainBall = gameObject;
        collisions = new List<string>();
        butExit = GameObject.Find("Button Exit").GetComponent<Button>();
        cam = (Camera)FindObjectOfType(typeof(Camera));

        addSpeed = 5f;
        maxForce = 30f;
        lineDirection = Vector3.right;
        lineLength = 2f;
        rotationSpeed = 2f;
        lengthChangeSpeed = 1f;
    }

    void FixedUpdate()
    {
        // Блокируем кнопку выхода, пока шары не остановятся
        butExit.interactable = turnEnded;

        // Постановка битка (Если потребуется)
        PutMainBall();

        // Если игра не закончена и не требуется поставить биток
        if (playing) {
            //  То если это игра с ботом и очередь бота
            if (GlobalParam._opponentBot && !PlayerUI.turn)
            {
                // Ход бота
                MakeHitByBot();
                // Расчета правил
                CalculationRules();
            }
            // Иначе если это с ботом и очередь игрока или игра без бота
            else if (PlayerUI.turn && GlobalParam._opponentBot || !GlobalParam._opponentBot)
            {
                // Отрисовка линии удара
                DrawLineHit();
                // Расчета правил
                CalculationRules();
                // Выполнить удар по шару 
                MakeHitByPlayer();
            }
        }
    }

    // Если произошло столкновение битка скаким-то объектом
    private void OnCollisionEnter(Collision collision)
    {
        // Если объект, котрый толкнули, является черным, полым или полосатым шаром
        if (collision.gameObject.tag == "half_full" || collision.gameObject.tag == "full" || collision.gameObject.tag == "lost")
        {
            // То добавляем в лист столкновений
            collisions.Add(collision.gameObject.tag);
        }
    }

    // Функция для определения, остановились ли все шары
    public bool CheckStops()
    {
        // Булевская переменная для определения движения шаров
        bool aremoving = false;

        // Инициализируем и заполняем спискок всех шаров по их тегам
        List<GameObject> Balls = new List<GameObject>();
        Balls.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        Balls.AddRange(GameObject.FindGameObjectsWithTag("full"));
        Balls.AddRange(GameObject.FindGameObjectsWithTag("lost"));
        Balls.AddRange(GameObject.FindGameObjectsWithTag("half_full"));

        // Цикл проверки на движение шаров
        foreach (GameObject Ball in Balls)
        {
            // Если шар двигается
            if (Ball.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
            {
                // То переменная, характеризующую наличие движение true
                aremoving = true;
                // Выходим из цикла, так как минимум один шар уже двигается
                break;
            }
            else
            {
                // Иначе переменная, характеризующая наличие движение false
                aremoving = false;
            }
        }
        // Очищаем лист шаров
        Balls.Clear();
        // Возвращаем переменную, характеризующую наличие движение
        return aremoving;
    }

    // Функция для постановки битка
    public void PutMainBall()
    {
        // Если требуется поставить биток
        if (needPutMainBall) {
            // Если биток требуется поставить игроку
            if (!GlobalParam._opponentBot || GlobalParam._opponentBot && PlayerUI.turn)
            {
                // Выводим сообщение о требовании постановки битка игроку
                PlayerUI._winnerInfo.text = "Выберите позицию для битка";

                // Инициализируем и заполняем лист запрещающих позиций постановки битка 
                List<GameObject> forbiddenPlaces = new List<GameObject>();
                GameObject[] half_full = GameObject.FindGameObjectsWithTag("half_full");
                GameObject[] full = GameObject.FindGameObjectsWithTag("full");
                GameObject[] lost = GameObject.FindGameObjectsWithTag("lost");
                GameObject[] hole = GameObject.FindGameObjectsWithTag("hole");
                forbiddenPlaces.AddRange(half_full);
                forbiddenPlaces.AddRange(full);
                forbiddenPlaces.AddRange(lost);
                forbiddenPlaces.AddRange(hole);

                // Инициализируем переменную, показывающая корректность выбранной позиции постановки битка 
                bool correctPlace = false;
                // Если нажали левой кнопки мыши на какую-то позицию
                if (Input.GetMouseButtonDown(0))
                {
                    // Инициализация луча от камеры в сторону позиции мыши
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    // Если этот луч пересекает первый встречный объект (возвращаемое значение hit - точка пересечения луча и этого объекта) 
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // То если эта точка попадает в раницы стола
                        if (hit.point.x < 30f && hit.point.z < 19f && hit.point.x > -60f && hit.point.z > -19f)
                        {
                            // Цикл по всем запрещающим областям на игровом столе
                            foreach (GameObject forbiddenPlace in forbiddenPlaces)
                            {
                                // Если эта область существует
                                if (forbiddenPlace != null)
                                {
                                    // То если дистанция от точки постановки битка до запрещающей области <= 4
                                    if (Vector3.Distance(forbiddenPlace.transform.position, hit.point) <= 4)
                                    {
                                        // Это не корректная область
                                        correctPlace = false;
                                        // Выходим из цикла, так как уже нельзя поставить в эту позции биток
                                        break;
                                    }
                                    else
                                    {
                                        // Это корректная область
                                        correctPlace = true;
                                    }
                                }
                            }

                            // Если позиция постановки битка корректна
                            if (correctPlace)
                            {
                                // Ставим биток в эту позицию и обнуляем его физические свойства
                                mainBall.transform.position = new Vector3(hit.point.x, 25.85f, hit.point.z); ;
                                mainBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                                mainBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            }
                            // Убираем необходимость поостановки битка
                            needPutMainBall = false;
                            // Если играем без бота или играем с ботом и ход игрока
                            if (!GlobalParam._opponentBot || GlobalParam._opponentBot && PlayerUI.turn)
                            {
                                // Отображаем линию удара
                                mainBall.GetComponent<LineRenderer>().enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    // Иначе игра не продолжается
                    playing = false;
                }
            }
            // Иначе если требуется поставить биток боту
            else if (GlobalParam._opponentBot && !PlayerUI.turn)
            {
                // То ставим его в центр и обнуляем его физические параметры
                mainBall.transform.position = new Vector3(-23f, 25.85f, 0f);
                mainBall.GetComponent<Rigidbody>().velocity = Vector3.zero;

                // Убираем необходимость поостановки битка
                needPutMainBall = false;
            }
        }
        else
        {
            // Иначе очищаем всплывающее сообщение и продолжаем игру
            PlayerUI._winnerInfo.text = "";
            playing = true;
        }
    }

    // Функция Выполнения удара по шару ботом
    public void MakeHitByBot()
    {
        // Боту можно совершить удар
        if (abilityHit)
        {
            // Ициализируем случайную силу удара для бота
            System.Random rnd = new System.Random();
            float power = rnd.Next(10, 30);

            // Инициализируем и заполняем лист шаров, кроме битка
            List<GameObject> balls = new List<GameObject>();
            GameObject[] half_full = GameObject.FindGameObjectsWithTag("half_full");
            GameObject[] full = GameObject.FindGameObjectsWithTag("full");
            GameObject[] lost = GameObject.FindGameObjectsWithTag("lost");
            balls.AddRange(half_full);
            balls.AddRange(full);
            balls.AddRange(lost);

            // Объявляем переменную ближайшего к битку шара нужного типа
            GameObject closestBall = null;

            // Инициализируем дистанцию от битка до шара нужного типа, равной бесконечность (потом будет уменьшаться)
            float closestDistanceToBall = Mathf.Infinity;

            // Цикл для нахождения ближайшего шара нужного типа
            foreach (GameObject ball in balls)
            {
                // Если данный шар в этой итерации нужного типа или типы шаров не определены и данный шар в этой итерации не черный шар
                if ((ball.tag == $"{PlayerUI.player2TypeBalls}" || PlayerUI.player2TypeBalls == "-") && ball.tag != "lost")
                {
                    // Инициализируем и расчитываем дистанцию до данного шара в этой итерации
                    float distance = Vector3.Distance(mainBall.transform.position, ball.transform.position);

                    // Если эта дистанция до данного шара в этой итерации меньше общей ближайшей дистанции
                    if (distance < closestDistanceToBall)
                    {
                        // Приравниваем ближайший шар к данному шару в этой итерации
                        closestBall = ball;
                        // Обновляем общую ближайшую дистанция
                        closestDistanceToBall = distance;
                    }
                }
                // Если бот забил все шары своего типа и данный шар в этой итерации - черный шар
                if (Score.score2 == 7 && ball.tag == "lost")
                {
                    // Инициализируем и расчитываем дистанцию до данного шара в этой итерации
                    float distance = Vector3.Distance(mainBall.transform.position, ball.transform.position);

                    // Приравниваем ближайший шар к данному шару в этой итерации
                    closestBall = ball;
                    // Выходим из цикла, так как у нас только один черный шар
                    break;
                }
            }

            // Если найден ближайший шар нужного типа
            if (closestBall != null)
            {
                // Определение направления от битка к ближайшему шару
                Vector3 directionToBall = (closestBall.transform.position - mainBall.transform.position).normalized;

                // Придаем силу для удараа битка ботом
                mainBall.GetComponent<Rigidbody>().AddForce(directionToBall * power * addSpeed, ForceMode.Impulse);

                // Не даем возможность снова бить по битку
                abilityHit = false;
                // Очередь бота еще не закончена
                turnEnded = false;
                // Ставим флаг паузы для регистрации движения битка
                pause = true;
            }           
        }
    }

    // Функция отрисовки линии удара
    public void DrawLineHit()
    {
        // Получение горизонатльного и вертикального вводов от игрока
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Поворот направления линии удара в зависимости от горизонтального ввода
        lineDirection = Quaternion.Euler(0, horizontalInput * rotationSpeed, 0) * lineDirection;
        // Изменение длины линии в зависимости от вертикального ввода и ограничение её значения
        lineLength = Mathf.Clamp(lineLength + verticalInput * lengthChangeSpeed, 1f, maxForce);

        // Рассчет конечной точки линии удара относительно позиции главного шара
        endLine = mainBall.GetComponent<Rigidbody>().transform.position + lineDirection.normalized * lineLength;

        // Установка позиции начала и конца линии на позицию главного шара
        mainBall.GetComponent<LineRenderer>().SetPosition(0, mainBall.GetComponent<Rigidbody>().transform.position);
        mainBall.GetComponent<LineRenderer>().SetPosition(1, endLine);
    }

    // Функция расчета правил
    public void CalculationRules()
    {
        // Если игра без бота или ходит игрок и игра с ботом
        if (!GlobalParam._opponentBot || GlobalParam._opponentBot && PlayerUI.turn)
        {
            // То если шары остановились и очередь игрока не закончена
            if (!CheckStops() && !turnEnded)
            {
                // Расчитываем правила
                Rules.RulesBalls();
                // Обновляем пользовательский интерфейс
                PlayerUI.UpdateUI();
                // Очищаем листы столкновений и забитых шаров
                Rules.ClearCollisionsAndColliders();
                // Очередь игрока закончена
                turnEnded = true;
                // Разрешаем бить по битку
                abilityHit = true;
            }
        }
        // Иначе если игра с отом и ход бота
        else if (GlobalParam._opponentBot && !PlayerUI.turn) 
        {
            // То если шары остановились и очередь игрока не закончена и нет паузы для регистрации движения битка после удара ботом
            if (!CheckStops() && !turnEnded && !pause)
            {
                // Расчитываем правила
                Rules.RulesBalls();
                // Обновляем пользовательский интерфейс
                PlayerUI.UpdateUI();
                // Очищаем листы столкновений и забитых шаров
                Rules.ClearCollisionsAndColliders();
                // Очередь игрока закончена
                turnEnded = true;
                // Разрешаем бить по битку
                abilityHit = true;
            }
            else
            {
                // Иначе отменить паузу для регистрации движения битка после удара ботом
                pause = false;
            }
        }
    }

    // Функция выполнения удара по шару игроком
    public void MakeHitByPlayer()
    {
        if (Input.GetKey(KeyCode.Space) && abilityHit)
        {
            // Ставим флаг чтобы заблокировать повторное нажатие на пробел до полной остановки всех шаров
            abilityHit = false;
            // Показываем, что ход еще не закончен
            turnEnded = false;
            // Отключаем отображение линии удара
            mainBall.GetComponent<LineRenderer>().enabled = false;
            // Придаем силу толчка шару
            mainBall.GetComponent<Rigidbody>().AddForce((endLine - mainBall.GetComponent<Rigidbody>().transform.position) * addSpeed, ForceMode.Impulse);
        }
        /*// Ставим биток в центре стола по нажатию rcntl
        if (Input.GetKey(KeyCode.RightControl))
        {
            transform.position = new Vector3(-23f, 25f, 0f);
            mainBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            mainBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }*/
    }

}
