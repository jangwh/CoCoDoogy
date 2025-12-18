using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public Transform mainPlane;
    public int gridWidth => (int)mainPlane.lossyScale.x * 5;
    public int gridHeight => (int)mainPlane.lossyScale.z * 5;
    public float cellSize = 1f;
    public Material lineMaterial;

    private Vector3 offset = new Vector3(.5f, 0, .5f);
    void Start()
    {
        // 세로선 (Z축 방향)
        for (int x = -gridWidth; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, -.5f, -gridHeight * cellSize);
            Vector3 end = new Vector3(x * cellSize, -.5f, gridHeight * cellSize);
            start += offset;
            end += offset;
            CreateLine(start, end);
        }

        // 가로선 (X축 방향)
        for (int z = -gridHeight; z <= gridHeight; z++)
        {
            Vector3 start = new Vector3(-gridWidth * cellSize, -.5f, z * cellSize);
            Vector3 end = new Vector3(gridWidth * cellSize, -.5f, z * cellSize);
            start += offset;
            end += offset;
            CreateLine(start, end);
        }

        CreateOrigin();
    }

    void CreateOrigin()
    {
        GameObject lineObj = new GameObject("OriginLine");
        lineObj.transform.parent = transform;

        var lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.SetPosition(0, new(.5f,-.5f,.5f));
        lr.SetPosition(1, new(-1.5f, -.5f, -1.5f));
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        lr.useWorldSpace = true;

        GameObject lineObj2 = new GameObject("OriginLine2");
        lineObj2.transform.parent = transform;

        var lr2 = lineObj2.AddComponent<LineRenderer>();
        lr2.material = lineMaterial;
        lr2.positionCount = 2;
        lr2.SetPosition(0, new(-1.5f, -.5f, .5f));
        lr2.SetPosition(1, new(.5f, -.5f, -1.5f));
        lr2.startWidth = .1f;
        lr2.endWidth = .1f;
        lr2.useWorldSpace = true;
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.parent = transform;

        var lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        if ((start + offset).x % 5 == 0 || (start + offset).z % 5 == 0)
        {
            lr.startWidth = .2f;
            lr.endWidth = .2f;
        }
        else {
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
        }
        lr.useWorldSpace = true;
    }
}


