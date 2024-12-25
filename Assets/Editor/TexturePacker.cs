using UnityEditor;
using UnityEngine;

// Editor window for listing all float curves in an animation clip
public class TexturePacker : EditorWindow
{
    private Texture2D red;
    private Texture2D green;
    private Texture2D blue;
    private Texture2D alpha;
    private Texture2D result;
    private string nameFileResult;


    private bool redOneMinus;
    private bool greenOneMinus;
    private bool blueOneMinus;
    private bool alphaOneMinus;

    [MenuItem("Window/Texture Packer")]
    static void Init()
    {
        GetWindow(typeof(TexturePacker));
    }

    public void OnGUI()
    {
        red = EditorGUILayout.ObjectField("Red Channel", red, typeof(Texture2D), false) as Texture2D;
        redOneMinus = EditorGUILayout.Toggle("OneMinus", redOneMinus);
        green = EditorGUILayout.ObjectField("Green Channel", green, typeof(Texture2D), false) as Texture2D;
        greenOneMinus = EditorGUILayout.Toggle("OneMinus", greenOneMinus);
        blue = EditorGUILayout.ObjectField("Blue Channel", blue, typeof(Texture2D), false) as Texture2D;
        blueOneMinus = EditorGUILayout.Toggle("OneMinus", blueOneMinus);
        alpha = EditorGUILayout.ObjectField("Alpha Channel", alpha, typeof(Texture2D), false) as Texture2D;
        alphaOneMinus = EditorGUILayout.Toggle("OneMinus", alphaOneMinus);
        nameFileResult = EditorGUILayout.TextField("Name:", nameFileResult);

        if (GUILayout.Button("Pack"))
        {
            if (!red) return;
            int width = red.width;
            int height = red.height;
            //ideally all input textures should be same size but I don check that bexuse its hard
            //also red must be filled


            //make new copies that are read/writable...
            Texture2D redRW = Blit(red);
            Texture2D greenRW = Blit(green);
            Texture2D blueRW = Blit(blue);
            Texture2D alphaRW = Blit(alpha);


            result = new Texture2D(width, height);
            

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float r = GetValue(x, y, redRW, redOneMinus, 0);
                    float g = GetValue(x, y, greenRW, greenOneMinus, 0);
                    float b = GetValue(x, y, blueRW, blueOneMinus, 0);
                    float a = GetValue(x, y, alphaRW, alphaOneMinus, 0);
                    result.SetPixel(x,y,new Color(r, g, b, a));
                }
            }
            result.Apply();
        }

        result = EditorGUILayout.ObjectField("Result", result, typeof(Texture2D), false) as Texture2D;

        if (GUILayout.Button("Save Result"))
        {
            SaveTexture(result);
            
        }
    }

    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = System.IO.Path.Combine(Application.dataPath , "Textures");
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        string fileName;
        if (string.IsNullOrEmpty(nameFileResult))
        {
            fileName = "PackedTexture.png";
        }
        else
        {
            fileName = nameFileResult + ".png";
        }
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(dirPath, fileName), bytes);
        //Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private float GetValue(int x, int y, Texture2D tex, bool oneMinus = false, float defaulfValue = 0)
    {
        float p = tex? tex.GetPixel(x, y).r : defaulfValue;
        return oneMinus ? 1 - p : p;
    }

    private Texture2D Blit(Texture2D source)
    {
        if(!source) return null;
        
        RenderTexture tmp = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, tmp);

        RenderTexture previous = RenderTexture.active;

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

        // Create a new readable Texture2D to copy the pixels to it
        Texture2D result = new Texture2D(source.width, source.height);

        // Copy the pixels from the RenderTexture to the new Texture
        result.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        result.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = previous;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        return result;
    }
}
