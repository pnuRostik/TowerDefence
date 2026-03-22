using UnityEngine;

public class Path : MonoBehaviour
{
    public GameObject[] TrapOne;
    public GameObject[] TrapTwo;
    public GameObject[] TrapThree;


    private GameObject[][] allTraps;

    private void OnValidate()
    {
        allTraps = new GameObject[][]
        {
            TrapOne,
            TrapTwo,
            TrapThree
        };
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawLines(TrapOne);
        DrawLines(TrapTwo);
        DrawLines(TrapThree);
    }

    private void DrawLines(GameObject[] wayPoints)
    {
        if (wayPoints == null || wayPoints.Length < 2)
            return;

        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            if (wayPoints[i] != null && wayPoints[i + 1] != null)
            {
                Gizmos.DrawLine(
                    wayPoints[i].transform.position,
                    wayPoints[i + 1].transform.position
                );
            }
        }
    }


    public GameObject[] GetPath(int index)
    {
        Debug.Log(allTraps.Length);

        if (allTraps == null || index < 0 || index >= allTraps.Length)
        {
            return null;
        }
        Debug.Log("end");

        return allTraps[index];
    }
}