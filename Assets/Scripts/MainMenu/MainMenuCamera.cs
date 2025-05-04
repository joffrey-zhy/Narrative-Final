using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("��������")]
    public float minX = 0f;      // ��С X ֵ
    public float maxX = 16f;     // ��� X ֵ

    [Header("�ƶ��ٶ�")]
    public float speed = 1f;     // ���������Ŀ���

    private Vector3 _startPosition;

    void Start()
    {
        // ��¼�������ʼ�� Y �� Z ����
        _startPosition = transform.position;
    }

    void Update()
    {
        // ���㵱ǰӦ������ [0, maxX - minX] �ڵ�λ��
        float offset = Mathf.PingPong(Time.time * speed, maxX - minX);
        float x = minX + offset;

        // Ӧ���µ�λ�ã�����ԭ���� y, z ���䣩
        transform.position = new Vector3(x, _startPosition.y, _startPosition.z);
    }
}
