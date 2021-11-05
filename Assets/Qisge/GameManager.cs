// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //only set to false for testing unity scene
    public bool RunPythonFile = true;


    public QisgeConfig Config;

    /*

    public string InputFile = "input";
    public string SpriteFile = "sprite";
    public string PythonBaseFile = "run";
    public string GameFolder = "game";

    */

    //Linking
    public VisualManager Visuals;
    public InputManager Input;
    public WritingManager Writing;
    public TextManager Text;
    public SoundManager Sound;
    public CameraManager Camera;



    //const string spriteFolder = "Testing";


    //const string filePath = "Testing";

    //Minimum value which can be assigned to positions
    public const int MinPosition = -1000;

    //default values
    const string inputFile = "input.txt";
    const string spriteFile = "sprite.txt";
    const string runFile = "run.py";
    const string runFileTemp = "runTemp.py";

    const string gameFolder = "Data/game";


    public static string DataFolder;

    public static string InputFilePath = "";
    public static string SpriteFilePath = "";

    public static string RunFilePath = "";
    public static string TempRunFilePath = "";
    public static string PythonPath = "";

    PythonJob pythonJob;


    const string dataFolder = "Data";
    const string exchange = @"Exchange/";
#if UNITY_STANDALONE_WIN
    const string pythonEXE = @"/.q/python.exe";

#elif UNITY_STANDALONE_OSX
        const string pythonEXE = @"/.q/bin/python";

#else
        //const string pythonEXE = @"/.q/python.exe";
        //notimplemented yet

#endif


    void Awake() {

        string configPath = Path.Combine(Application.streamingAssetsPath, exchange) + "qisge.config";
        if (File.Exists(configPath)) {
            Debug.Log("config found");
            string json = File.ReadAllText(configPath);
            Config = JsonUtility.FromJson<QisgeConfig>(json);
        }

        if (Config.InputFile.Length < 3) {
            Config.InputFile = inputFile;
        } else {
            if (!Config.InputFile.EndsWith(".txt")) {
                Config.InputFile += ".txt";
            }
        }

        if (Config.SpriteFile.Length < 3) {
            Config.SpriteFile = spriteFile;
        } else {
            if (!Config.SpriteFile.EndsWith(".txt")) {
                Config.SpriteFile += ".txt";
            }
        }

        if (Config.PythonBaseFile.Length < 3) {
            Config.PythonBaseFile = runFile;
        } else {
            if (!Config.PythonBaseFile.EndsWith(".py")) {
                Config.PythonBaseFile += ".py";
            }
        }

        if (Config.GameFolder.Length < 3) {
            Config.GameFolder = gameFolder;
        } else {
            Config.GameFolder = Path.Combine(dataFolder, Config.GameFolder);
        }

        InputFilePath = Path.Combine(Application.streamingAssetsPath, exchange, Config.InputFile);
        SpriteFilePath = Path.Combine(Application.streamingAssetsPath, exchange, Config.SpriteFile);
        RunFilePath = Path.Combine(Application.streamingAssetsPath, exchange, Config.PythonBaseFile);


        DataFolder = Path.Combine(Application.streamingAssetsPath, exchange, Config.GameFolder);

        Debug.Log(gameFolder);

        PythonPath = Application.streamingAssetsPath + pythonEXE;


        //Creating temp file with correct path

        TempRunFilePath = RunFilePath.Replace(Config.PythonBaseFile, runFileTemp);

        string runFileContent = File.ReadAllText(RunFilePath);
        runFileContent=runFileContent.Replace(gameFolder, Config.GameFolder);

        Debug.Log("Create new file");

        Debug.Log(TempRunFilePath + " " + runFileContent);

        File.WriteAllText(TempRunFilePath, runFileContent);


        //inputFilePath = Path.Combine(Application.dataPath, filePath, inputFile);
        //spriteFilePath = Path.Combine(Application.dataPath, filePath, spriteFile);
        //SpriteFolder = Path.Combine(Application.dataPath, spriteFolder);
        Application.targetFrameRate = 60;

        File.WriteAllText(InputFilePath, string.Empty);
        File.WriteAllText(SpriteFilePath, string.Empty);
        //Debug.Log("input cleared");

        Debug.Log("All files prepared");
    }


    private void Start() {
        if (RunPythonFile) {
            prepareAndStartJob();
        }
    }

    // Update is called once per frame
    void Update() {
        CheckFiles();

        CheckJob();

        if (UnityEngine.Input.GetKeyDown( KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void CheckFiles() {

        Input.ControlledUpdate();


        if (Input.HasInput()) {
            using (FileStream inputReader = new FileStream(InputFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {

                if (inputReader.Length <= 10) {
                    using (StreamWriter writer = new StreamWriter(inputReader)) {
                        //string input = inputReader.ToString();
                        //Debug.Log(input);
                        InputFile file = Input.Collect();
                        string json = JsonUtility.ToJson(file);
                        writer.Write(json);
                        //File.WriteAllText(InputFilePath, json);
                    }
                }
            }
        }


        using (FileStream dataReader = new FileStream(SpriteFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {

            if (dataReader.Length > 10) {
                using (StreamReader reader = new StreamReader(dataReader)) {
                    string data = reader.ReadToEnd();
                    //string data = File.ReadAllText(SpriteFilePath);
                    //try {
                    dataReader.SetLength(0);
                    dataReader.Close();
                    UpdateFile update = JsonUtility.FromJson<UpdateFile>(data);
                    Visuals.UpdateAll(update);

                    //File.WriteAllText(SpriteFilePath, string.Empty);
                    Text.UpdateTexts(update.text_changes);
                    Sound.UpdateSounds(update);
                    Camera.UpdateCamera(update.camera_changes);
                    //}catch {
                    //dataReader.SetLength(0);
                    //dataReader.Close();
                    //Debug.LogError("Catch involved " + data);
                    //}
                }
            }

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

        Debug.Log("Start job");

        pythonJob = new PythonJob();
        pythonJob.PythonPath = PythonPath;
        pythonJob.FilePath = TempRunFilePath;

        Debug.LogWarning(PythonPath + " " + TempRunFilePath);

        pythonJob.Start();
    }

    void finishJob() {
        Debug.Log("Job finished");

    }

    private void OnDestroy() {
        pythonJob?.Abort();


        File.WriteAllText(InputFilePath, string.Empty);
        File.WriteAllText(SpriteFilePath, string.Empty);
        File.Delete(TempRunFilePath);
    }



}
