using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool RunPythonFile = true;


    public string InputFile="input";
    public string SpriteFile = "sprite";
    public string PythonBaseFile = "run";

    //Linking
    public VisualManager Visuals;
    public InputManager Input;
    public WritingManager Writing;
    public TextManager Text;





    //const string spriteFolder = "Testing";


    //const string filePath = "Testing";

    //Minimum value which can be assigned to positions
    public const int MinPosition = -1000;

    const string inputFile = "input.txt";
    const string spriteFile = "sprite.txt";
    const string runFile = "run.py";
    const string dataFolder = "Data";


    public static string DataFolder;

    public static string InputFilePath = "";
    public static string SpriteFilePath = "";

    public static string RunFilePath = "";
    public static string PythonPath = "";

    PythonJob pythonJob;


    const string exchange = @"Exchange/";
#if UNITY_STANDALONE_WIN
    const string pythonEXE = @"/.q/python.exe";

#elif UNITY_STANDALONE_OSX
        const string pythonEXE = @"/.q/bin/python";

#else
        //const string pythonEXE = @"/.q/python.exe";
        //notimplemented yet

#endif


    void Awake()
    {
        if (InputFile.Length<3) {
            InputFile = inputFile;
        } else {
            InputFile += ".txt";
        }

        if (SpriteFile.Length < 3) {
            SpriteFile = spriteFile;
        } else {
            SpriteFile += ".txt";
        }

        if (PythonBaseFile.Length < 3) {
            PythonBaseFile = runFile;
        } else {
            PythonBaseFile += ".py";
        }

        InputFilePath = Path.Combine(Application.streamingAssetsPath, exchange, InputFile);
        SpriteFilePath = Path.Combine(Application.streamingAssetsPath, exchange, SpriteFile);
        RunFilePath = Path.Combine(Application.streamingAssetsPath, exchange, PythonBaseFile);
        DataFolder = Path.Combine(Application.streamingAssetsPath, exchange, dataFolder);

        PythonPath = Application.streamingAssetsPath + pythonEXE;


        //inputFilePath = Path.Combine(Application.dataPath, filePath, inputFile);
        //spriteFilePath = Path.Combine(Application.dataPath, filePath, spriteFile);
        //SpriteFolder = Path.Combine(Application.dataPath, spriteFolder);
        Application.targetFrameRate = 60;

        File.WriteAllText(InputFilePath, string.Empty);
        if (!File.Exists(SpriteFilePath)) {
            File.WriteAllText(SpriteFilePath, string.Empty);
        }

    }


    private void Start() {
        if (RunPythonFile) {
            prepareAndStartJob();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckFiles();

        CheckJob();


    }

    public void CheckFiles() {

        Input.ControlledUpdate();


        if (Input.HasInput()) {
            string input = File.ReadAllText(InputFilePath);
            if (input.Length == 0) {
                InputFile file = Input.Collect();
                string json = JsonUtility.ToJson(file);
                File.WriteAllText(InputFilePath, json);
            }
        }

        string sprite = File.ReadAllText(SpriteFilePath);

        if (sprite.Length>0) {
            UpdateFile update = JsonUtility.FromJson<UpdateFile>(sprite);
            Visuals.UpdateAll(update);
            File.WriteAllText(SpriteFilePath, string.Empty);
            Text.UpdateTexts(update.text_changes);
        }
    }

    public void CheckJob() {
        if (pythonJob != null) {
            if (pythonJob.Update()) {
                // Alternative to the OnFinished callback
                finishJob();
                pythonJob = null;
            } else {

            }
        }
    }


    void prepareAndStartJob() {

        pythonJob = new PythonJob();
        pythonJob.PythonPath = PythonPath;
        pythonJob.FilePath = RunFilePath;


        pythonJob.Start();
    }

    void finishJob() {
        Debug.Log("Job finished");

    }

    private void OnDestroy() {
        pythonJob?.Abort();
    }



}
