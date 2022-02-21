using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelParser : MonoBehaviour
{
    public string filename;
    public GameObject Rock;
    public GameObject Brick;
    public GameObject QuestionBox;
    public GameObject Stone;
    public Transform levelRoot;

    // --------------------------------------------------------------------------
    void Start()
    {
        LoadLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
    }

    // --------------------------------------------------------------------------
    private void LoadLevel()
    {
        string fileToParse = $"{Application.dataPath}{"/Resources/"}{filename}.txt";
        Debug.Log($"Loading level file: {fileToParse}");
        
        Stack<string> levelRows = new Stack<string>();

        // Get each line of text representing blocks in our level
        using (StreamReader sr = new StreamReader(fileToParse))
        {
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                levelRows.Push(line);
            }

            sr.Close();
        }

        // Go through the rows from bottom to top
        int row = 0;
        while (levelRows.Count > 0)
        {
            string currentLine = levelRows.Pop();

            int column = 0;
            char[] letters = currentLine.ToCharArray();
            foreach (var letter in letters)
            {
                // Instantiate a new GameObject that matches the type specified by letter
                // Position the new GameObject at the appropriate location by using row and column
                // Parent the new GameObject under levelRoot
                switch (letter)
                {
                    case 'x':
                        var groundBlock = Instantiate(Rock);
                        groundBlock.transform.position = new Vector3(column, row, 0f);
                        // setting new object's parent with levelRoot
                        groundBlock.transform.SetParent(levelRoot);
                        break;
                    case 'b':
                        // setting new object's parent with levelRoot via upon creation
                        var brickBlock = Instantiate(Brick, levelRoot, true);
                        brickBlock.transform.position = new Vector3(column, row, 0f);
                        break;
                    case 's':
                        var stoneBlock = Instantiate(Stone, levelRoot, true);
                        stoneBlock.transform.position = new Vector3(column, row, 0f);
                        break;
                    case '?':
                        var questionBlock = Instantiate(QuestionBox, levelRoot, true);
                        questionBlock.transform.position = new Vector3(column, row, 0f);
                        break;
                }
                column++;
            }
            row++;
        }
    }

    // --------------------------------------------------------------------------
    private void ReloadLevel()
    {
        foreach (Transform child in levelRoot)
        {
           Destroy(child.gameObject);
        }
        LoadLevel();
    }
}
