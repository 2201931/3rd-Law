using UnityEngine;
using System.Collections;

public class DottedLine2D : MonoBehaviour
{
    public GameObject dotPrefab; // Prefab with the Sprite Renderer and circle sprite
    public Transform startPoint;
    public Transform endPoint;
    public int numberOfDots = 10;
    public float speed = 1f;
    private GameObject[] dots;

    void Start()
    {
        CreateDottedLine();
        StartCoroutine(AnimateDots());
    }

    void CreateDottedLine()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            Vector2 position = Vector2.Lerp(startPoint.position, endPoint.position, (float)i / (numberOfDots - 1));
            dots[i] = Instantiate(dotPrefab, position, Quaternion.identity, transform);
        }
    }

    IEnumerator AnimateDots()
    {
        while (true)
        {
            for (int i = 0; i < numberOfDots; i++)
            {
                float t = (Time.time * speed + (float)i / numberOfDots) % 1;
                dots[i].transform.position = Vector2.Lerp(startPoint.position, endPoint.position, t);
            }
            yield return null;
        }
    }
}
