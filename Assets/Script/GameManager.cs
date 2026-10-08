using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    void Start()
    {
        cam_ = GetComponent<Camera>();
        float aspect = (float)Screen.width / Screen.height;
        float targetSize = Mathf.Max(BaseSize, TargetW * 0.5f / aspect);

        cam_.orthographicSize = targetSize;

        states_.Clear();
        tiles_.Clear();
        for (int i = 0; i < 7; i++)
        {
            var cache = new bool[] { false, false, false, false };
            int randomIndex = UnityEngine.Random.Range(0, 4);
            cache[randomIndex] = true;
            states_.Add(cache);

            var cacheObject = new Tile[] {
                Instantiate(tilePrefab, new Vector3(-1.5f, i, 0f), Quaternion.identity),
                Instantiate(tilePrefab, new Vector3(-0.5f, i, 0f), Quaternion.identity),
                Instantiate(tilePrefab, new Vector3(0.5f, i, 0f), Quaternion.identity),
                Instantiate(tilePrefab, new Vector3(1.5f, i, 0f), Quaternion.identity)
            };
            cacheObject[randomIndex].SetBlack();
            tiles_.Add(cacheObject);
        }

        cam_.transform.position = new Vector3(0f, tiles_[1][0].transform.position.y + cam_.orthographicSize - 0.5f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        int lane = -1;

        {
            if (Input.GetKeyDown(KeyCode.D)) lane = 0;
            else if (Input.GetKeyDown(KeyCode.F)) lane = 1;
            else if (Input.GetKeyDown(KeyCode.J)) lane = 2;
            else if (Input.GetKeyDown(KeyCode.K)) lane = 3;

            if (lane < 0 && Input.GetMouseButtonDown(0))
            {
                Vector3 world = cam_.ScreenToWorldPoint(Input.mousePosition);
                lane = Mathf.Clamp(Mathf.FloorToInt(world.x + TargetW * 0.5f), 0, 3);
            }

            if (lane < 0) goto MOVE_CAMERA; // no input
        }

        {
            if (states_[1][lane] == false)
            {
                tiles_[1][lane].SetRed();
                goto MOVE_CAMERA;
            }

            for (int i = 0; i < 4; i++)
            {
                Destroy(tiles_[0][i].gameObject);
            }
            tiles_.RemoveAt(0);
            states_.RemoveAt(0);

            var cache = new bool[] { false, false, false, false };
            int randomIndex = UnityEngine.Random.Range(0, 4);
            cache[randomIndex] = true;
            states_.Add(cache);

            float y = tiles_[tiles_.Count - 1][0].transform.position.y + 1f;
            var cacheObject = new Tile[] {
            Instantiate(tilePrefab, new Vector3(-1.5f, y, 0f), Quaternion.identity),
            Instantiate(tilePrefab, new Vector3(-0.5f, y, 0f), Quaternion.identity),
            Instantiate(tilePrefab, new Vector3(0.5f, y, 0f), Quaternion.identity),
            Instantiate(tilePrefab, new Vector3(1.5f, y, 0f), Quaternion.identity)
        };
            cacheObject[randomIndex].SetBlack();
            tiles_.Add(cacheObject);
        }

    MOVE_CAMERA:
        {
            float camY = cam_.transform.position.y;
            float targetY = tiles_[1][0].transform.position.y + cam_.orthographicSize - 0.5f;
            float dist = Mathf.Abs(targetY - camY);

            cam_.transform.position = new Vector3(
                0f,
                Mathf.MoveTowards(camY, targetY, dist / 0.01f * Time.deltaTime),
                -10f
            );
        }
    }
    private Camera cam_;
    const float TargetW = 4f;
    const float TargetH = 5f;
    const float BaseSize = TargetH * 0.5f;
    private List<bool[]> states_ = new List<bool[]>();
    private List<Tile[]> tiles_ = new List<Tile[]>();
}
