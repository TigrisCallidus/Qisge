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
using Qiskit;
using System.Diagnostics;
using System.IO;

public class PythonJob : ThreadedJob {

    public string PythonPath;
    public string FilePath;

    Process process;

    protected override void ThreadFunction() {
        // Do your threaded task. DON'T use the Unity API here
        RunPythonFile();
    }
    protected override void OnFinished() {
        // This is executed by the Unity main thread when the job is finished
        
    }


    public string RunPythonFile() { //, ref double[] probabilities) {
        UnityEngine.Debug.Log("Starting thread");


        process = new Process();
        string pythonPath = PythonPath;
        string filePath = FilePath;        


        process.StartInfo.FileName = pythonPath;
        process.StartInfo.Arguments = filePath;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;



        process.Start();


        //setting real time (verry high) priority for the process
        process.PriorityClass = ProcessPriorityClass.RealTime;

        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();


        process.WaitForExit();
        int exitcode=process.ExitCode;
        UnityEngine.Debug.Log(output);
        if (exitcode>0) {
            UnityEngine.Debug.LogError(process.StandardError.ReadToEnd());
        }

        process.Close();
        //Debug.Log(output);
        return output;
        //ReadProbabilities(ref probabilities, output);
    }

    public override void Abort() {
        //process?.CloseMainWindow();
        process?.Kill();
        base.Abort();
    }
}