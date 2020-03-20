using UnityEngine;
using UnityEngine.UI;

public class HandlePollution : MonoBehaviour
{
    public GameObject Terrain;
    public Transform subbasins;

    public void SetSizes(float[] sizes)
    {
        for (int i = 0; i < subbasins.childCount; i++)
        {
            MeshRenderer renderer = subbasins.GetChild(i).GetComponent<MeshRenderer>();
            if (sizes[i] <= 25)
            {
                renderer.material.SetColor("_Color", Color.green);
            }
            else if (sizes[i] > 25 && sizes[i] <= 50)
            {
                renderer.material.SetColor("_Color", Color.blue);
            }
            else if (sizes[i] > 50 && sizes[i] <= 75)
            {
                renderer.material.SetColor("_Color", Color.yellow);
            }
            else
            {
                renderer.material.SetColor("_Color", Color.red);
            }
        }
    }
}
