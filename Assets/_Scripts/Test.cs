using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject cube;
    public bool isCubeShow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleCube()
    {
        cube.SetActive(!isCubeShow);
        isCubeShow = !isCubeShow;
    }
}
