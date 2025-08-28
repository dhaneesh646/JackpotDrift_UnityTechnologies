using System.Collections;
using UnityEngine;

public class Row : MonoBehaviour
{
    public Sprite[] symbolSprites;
    public bool isRotateStopped;
    public float spinSpeed = 20.0f;
    private float symbolHeight;
    public string stoppedSlot;
    public float symbolSpacing = 2.0f;
    public int totalSymbols = 11;
    public GameObject symbolPrefab;

    public RectTransform slotMaskArea;
    public int visibleSymbols = 3;
    [SerializeField] Transform[] symbolObjects;
    public string[] visibleSymbolsNames = new string[3];

    void Start()
    {
        CalculateSymbolSpacing();
        CreateSymbols();
        isRotateStopped = true;
    }

    public void StartRotate()
    {
        isRotateStopped = false;
        StartCoroutine(Rotate());
    }

    void CalculateSymbolSpacing()
    {
        SpriteRenderer prefabRenderer = symbolPrefab.GetComponent<SpriteRenderer>();
        if (prefabRenderer != null)
        {
            symbolHeight = prefabRenderer.bounds.size.y;
        }
        else
        {
            Debug.LogError("Symbol Prefab does not have a SpriteRenderer component.");
            return;
        }

        float visibleHeight = slotMaskArea.rect.height;
        symbolSpacing = visibleHeight / visibleSymbols;

        float scaleFactor = symbolSpacing / symbolHeight;

        symbolHeight = symbolSpacing;
    }

    void CreateSymbols()
    {
        symbolObjects = new Transform[totalSymbols];

        AutoFitSymbols();

        for (int i = 0; i < totalSymbols; i++)
        {
            GameObject symbol = Instantiate(symbolPrefab, transform);

            float startY = (symbolSpacing / 2.0f) + (visibleSymbols / 2.0f) * symbolSpacing;
            symbol.transform.localPosition = new Vector2(0, startY - i * symbolSpacing);
            int randomSpriteIndex = Random.Range(0, symbolSprites.Length);
            symbol.GetComponent<SpriteRenderer>().sprite = symbolSprites[randomSpriteIndex];

            symbolObjects[i] = symbol.transform;
        }
    }


    void AutoFitSymbols()
    {
        Vector3[] corners = new Vector3[4];
        slotMaskArea.GetWorldCorners(corners);
        float visibleHeight = corners[1].y - corners[0].y;
        symbolSpacing = visibleHeight / visibleSymbols;
    }

    IEnumerator Rotate()
    {
        float rotationDuration = Random.Range(2.0f, 4.0f);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            MoveSymbols(spinSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        float slowDownDuration = 1.5f; // Time to slow down and stop
        float slowDownTime = 0f;

        while (slowDownTime < slowDownDuration)
        {
            float currentSpeed = Mathf.Lerp(spinSpeed, 0, slowDownTime / slowDownDuration);
            MoveSymbols(currentSpeed);
            slowDownTime += Time.deltaTime;
            yield return null;
        }

        SnapToStopPosition();
        isRotateStopped = true;
    }

    void MoveSymbols(float speed)
    {
        float moveDistance = speed * Time.deltaTime;

        for (int i = 0; i < totalSymbols; i++)
        {
            Transform symbolTransform = symbolObjects[i];
            symbolTransform.localPosition = new Vector2(symbolTransform.localPosition.x, symbolTransform.localPosition.y + moveDistance);

            if (symbolTransform.localPosition.y > (visibleSymbols + 1) * symbolSpacing)
            {
                symbolTransform.localPosition = new Vector2(symbolTransform.localPosition.x, symbolTransform.localPosition.y - totalSymbols * symbolSpacing);

                int randomSpriteIndex = Random.Range(0, symbolSprites.Length);
                symbolTransform.GetComponent<SpriteRenderer>().sprite = symbolSprites[randomSpriteIndex];
            }
        }
    }

    void SnapToStopPosition()
    {
        float centerY = 0f; // Center of the mask in local space
        int centerIndex = 0;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < totalSymbols; i++)
        {
            float distance = Mathf.Abs(symbolObjects[i].localPosition.y - centerY);
            if (distance < minDistance)
            {
                minDistance = distance;
                centerIndex = i;
            }
        }

        // Now adjust all symbols so the center symbol is exactly at centerY
        float offset = symbolObjects[centerIndex].localPosition.y - centerY;
        for (int i = 0; i < totalSymbols; i++)
        {
            symbolObjects[i].localPosition = new Vector2(
                symbolObjects[i].localPosition.x,
                symbolObjects[i].localPosition.y - offset
            );
        }

        // Now get the visible symbols (center, one above, one below)
        int topIndex = (centerIndex - 1 + totalSymbols) % totalSymbols;
        int bottomIndex = (centerIndex + 1) % totalSymbols;

        // Assign the visible symbols
        visibleSymbolsNames[0] = symbolObjects[topIndex].GetComponent<SpriteRenderer>().sprite.name;
        visibleSymbolsNames[1] = symbolObjects[centerIndex].GetComponent<SpriteRenderer>().sprite.name;
        visibleSymbolsNames[2] = symbolObjects[bottomIndex].GetComponent<SpriteRenderer>().sprite.name;
    }
}