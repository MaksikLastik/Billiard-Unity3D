using UnityEngine;

public class BallPhysic : MonoBehaviour
{
    void Update()
    {
        // ������ ����� ����������� ���������� ����� � ������������� ��, ����� ���������� �� ���� ������
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < 0.11f)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
