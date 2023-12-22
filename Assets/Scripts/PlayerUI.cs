using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    // ������� ���� (true - ����� �����, false - ������ �����)
    public static bool turn = true;

    // ���� ����� �������
    public static string player1TypeBalls;
    public static string player2TypeBalls;

    // ����� �������
    public static string player1Name;
    public static string player2Name;

    // ����� ����� ���������� � ��������� ����� � ����� ���� ������� ������ (��� ������ � ������� ���������)
    public static Text _player1Text;
    public static Text _player2Text;
    // ����� ����� ���������� � ��������� ����� � ����� ���� ������� ������ (��������� � ��������)
    public Text player1Text;
    public Text player2Text;

    // ����������� ��������� ���������� � ������ ��� ������� ��������� ����� (��� ������ � ������� ���������)
    public static Text _winnerInfo;
    // ����������� ��������� ���������� � ������ ��� ������� ��������� ����� (�������� � �������)
    public Text winnerInfo;

    public void Start()
    {
        // ���������� ���� �����
        player1TypeBalls = player2TypeBalls = "-";

        // �������������� �������� ����������
        _player1Text = player1Text;
        _player2Text = player2Text;
        _winnerInfo = winnerInfo;

        // ���� ��� ������� ���� ��� ������� ������ ��������� �� ������
        if (GlobalParam._player1Name.Length != 0)
        {
            // �� ����������� ��� ��� ��� �� ����������� ����
            player1Name = GlobalParam._player1Name;
        }
        // ����� ���� ������ ������ ����
        else if (GlobalParam._opponentBot)
        {
            // �� ����������� ��� ��� "Player"
            player1Name = "Player";
        }
        else
        {
            // ����� ����������� ��� ��� "Player 1"
            player1Name = "Player 1";
        }

        // ���� ��� ������� ���� ��� ������� ������ ��������� �� ������
        if (GlobalParam._player2Name.Length != 0)
        {
            // �� ����������� ��� ��� ��� �� ����������� ����
            player2Name = GlobalParam._player2Name;
        }
        // ����� ���� ������ ������ ����
        else if (GlobalParam._opponentBot)
        {
            // �� ����������� ��� ��� "Bot"
            player2Name = "Bot";
        }
        else
        {
            // ����� ����������� ��� ��� "Player 2"
            player2Name = "Player 2";
        }

        // ��������� ���������
        UpdateUI();
    }

    public void Update()
    {
        // ��������� ��������� ���������
        player1Text = _player1Text;
        player2Text = _player2Text;
        winnerInfo = _winnerInfo;
        player1Text.color = _player1Text.color;
        player2Text.color = _player2Text.color;
    }

    public static void UpdateUI()
    {
        // ���� ����� ������ �����, �� �� �������������� �������, � ������ - �����
        if (turn)
        {
            _player1Text.color = Color.red;
            _player2Text.color = Color.white;
        }
        // ����� ����� ������ ����� �������������� �������, � ������ - �����
        else
        {
            _player1Text.color = Color.white;
            _player2Text.color = Color.red;
        }

        // ��������� ���������� ��� ������� ������
        _player1Text.text = player1Name + " (" + player1TypeBalls + "): " + Score.score1.ToString();
        _player2Text.text = player2Name + " (" + player2TypeBalls + "): " + Score.score2.ToString();
    }
}