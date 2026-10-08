using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private float holdTime = 0.15f;
    [SerializeField] private float fadeTime = 0.35f;

    public void SetBlack() { body.color = Color.black; }
    public void SetWhite() { body.color = Color.white; }
    public void SetRed()
    {
        StopAllCoroutines();
        StartCoroutine(RedRoutine());
    }

    IEnumerator RedRoutine()
    {
        Color red = Color.red;
        body.color = red;
        yield return new WaitForSeconds(holdTime);

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            body.color = Color.Lerp(Color.red, Color.white, k);
            yield return null;
        }
        body.color = Color.white;
    }

    private void Awake()
    {
        body.color = Color.white;
    }
}
