using UnityEngine;
using System.Collections;

public class MaterialChanger : MonoBehaviour
{
    public Material[] materials; // Ҫ�л��Ĳ�������
    public float changeInterval = 1f; // �л����ʱ�䣨�룩

    private Renderer rend;
    private int currentIndex = 0;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (materials.Length > 0)
        {
            rend.material = materials[0]; // ��ʼ��Ϊ��һ������
            StartCoroutine(ChangeMaterialOverTime());
        }
    }

    IEnumerator ChangeMaterialOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeInterval); // �ȴ�ָ��ʱ��

            currentIndex++;
            if (currentIndex >= materials.Length)
            {
                currentIndex = 0; // �����ĩβ��ѭ��
            }

            rend.material = materials[currentIndex];
        }
    }
}
