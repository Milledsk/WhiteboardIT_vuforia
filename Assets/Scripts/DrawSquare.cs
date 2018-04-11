using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSquare : MonoBehaviour {

    Material mat;

    private Vector3 firstPosition;
    private Vector3 position;
    private Screenshot screenshot;

    int buttonLeft_x;
    int buttonLeft_y;
    int width;
    int height;

    bool draw = true; 

    public int lineThickness = 10; 

	// Use this for initialization
	void Start () {
        screenshot = GameObject.Find("Screenshot").GetComponent<Screenshot>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.y < 200)
            {
                return;
            }

            firstPosition = Input.mousePosition;
            //Debug.Log(firstPosition);
            draw = true; 
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.y < 200)
            {
                return;
            }

            position = Input.mousePosition;
            //Debug.Log(position);
        }

        FindButtonLeft();
    }

    private void OnRenderObject()
    {
        if (!draw)
        {
            return; 
        }

        if(GetComponent<CanvasGroup>().alpha == 0)
        {
            draw = false; 
            return;
        }

        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;

            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        //Draw lines
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();
        GL.Begin(GL.QUADS);

        GL.Color(Color.gray);

        GL.Vertex3(buttonLeft_x, buttonLeft_y, 0);
        GL.Vertex3(buttonLeft_x + lineThickness, buttonLeft_y, 0);
        GL.Vertex3(buttonLeft_x + lineThickness, buttonLeft_y + height, 0);
        GL.Vertex3(buttonLeft_x, buttonLeft_y + height, 0);

        GL.Vertex3(buttonLeft_x, buttonLeft_y + height, 0);
        GL.Vertex3(buttonLeft_x, buttonLeft_y + height - lineThickness, 0);
        GL.Vertex3(buttonLeft_x + width, buttonLeft_y + height - lineThickness, 0);
        GL.Vertex3(buttonLeft_x + width, buttonLeft_y + height, 0);

        GL.Vertex3(buttonLeft_x + width, buttonLeft_y + height, 0);
        GL.Vertex3(buttonLeft_x + width - lineThickness, buttonLeft_y + height, 0);
        GL.Vertex3(buttonLeft_x + width - lineThickness, buttonLeft_y, 0);
        GL.Vertex3(buttonLeft_x + width, buttonLeft_y, 0);

        GL.Vertex3(buttonLeft_x + width, buttonLeft_y, 0);
        GL.Vertex3(buttonLeft_x + width, buttonLeft_y + lineThickness, 0);
        GL.Vertex3(buttonLeft_x, buttonLeft_y + lineThickness, 0);
        GL.Vertex3(buttonLeft_x, buttonLeft_y, 0);

        GL.End();
        GL.PopMatrix();
    }

    public void FindButtonLeft()
    {
        buttonLeft_x = (int)Mathf.Min(firstPosition.x, position.x);
        buttonLeft_y = (int)Mathf.Min(firstPosition.y, position.y);
        width = (int)Mathf.Abs(position.x - firstPosition.x);
        height = (int)Mathf.Abs(position.y - firstPosition.y);
    }

    public int[] GetSquareProporties()
    {
        int[] proporties = new int[] { buttonLeft_x, buttonLeft_y, width, height };
        position = firstPosition;
        return proporties;
    }


}
