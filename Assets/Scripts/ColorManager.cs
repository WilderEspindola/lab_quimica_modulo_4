using UnityEngine;
using UnityEngine.PlayerLoop;

public class ColorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Renderer colorCube;
    //public Color startColor= Color.green;
    public Color[] colors = { Color.green, Color.red, Color.blue, Color.black };
    public int initialColorInput = 0;
    /*public float startTime = 2.0f;
    public float timer = 0f;*/
    void Start()
    {
        //colorCube.material.color = startColor;

    }

    // Update is called once per frame
    public void ChangeColor()
    {
        /*timer += Time.deltaTime;
        if (timer > startTime)
        {
            initialColorInput = (initialColorInput + 1) % colors.Length;
            colorCube.material.color = colors[initialColorInput];
            timer = 0f;
        }*/
        initialColorInput = (initialColorInput + 1) % colors.Length;
        colorCube.material.color = colors[initialColorInput];
    }
}
