using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace TfGuiTool.Utils
{
    class CommandUtils
    {
        private const string CMD_PATH = @"C:\Windows\System32\cmd.exe";

        public static void Run(string cmd)
        {
            // Note: No matter execute success or not execute exit
            // otherwise when calling ReadToEnd() will freeze the program
            Debug.WriteLine("Run: " + cmd);
            cmd = cmd.Trim().TrimEnd('&') + "&exit";

            using Process p = new Process();
            p.StartInfo.FileName = CMD_PATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();

            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;
            p.WaitForExit();
            p.Close();
        }

        public static void Run(string cmd, out string output)
        {
            Debug.WriteLine("Run: " + cmd);
            cmd = cmd.Trim().TrimEnd('&') + "&exit";

            using Process p = new Process();
            p.StartInfo.FileName = CMD_PATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            // Get output, trim the cmd
            output = p.StandardOutput.ReadToEnd();
            int startIndex = output.IndexOf(cmd) + cmd.Length + 2;
            output = output.Substring(startIndex);

            p.WaitForExit();
            p.Close();
        }

        public static void Run(string cmd, out string output, out string error)
        {
            Debug.WriteLine("Run: " + cmd);
            cmd = cmd.Trim().TrimEnd('&') + "&exit";

            using Process p = new Process();
            p.StartInfo.FileName = CMD_PATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            // Get output, trim the cmd
            output = p.StandardOutput.ReadToEnd();
            int startIndex = output.IndexOf(cmd) + cmd.Length + 2;
            output = output.Substring(startIndex);

            // Get error
            error = p.StandardError.ReadToEnd();

            p.WaitForExit();
            p.Close();
        }

        public static void Run(string cmd, out string output, bool doTrim)
        {
            Debug.WriteLine("Run: " + cmd);
            cmd = cmd.Trim().TrimEnd('&') + "&exit";

            using Process p = new Process();
            p.StartInfo.FileName = CMD_PATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Verb = "runas";
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            // Get output
            output = p.StandardOutput.ReadToEnd();
            if (doTrim)
            {
                int startIndex = output.IndexOf(cmd) + cmd.Length + 2;
                output = output.Substring(startIndex);
            }

            p.WaitForExit();
            p.Close();
        }

        public static void RunAsync(string cmd)
        {
            Debug.WriteLine("Run: " + cmd);
            cmd = cmd.Trim().TrimEnd('&') + "&exit";

            ProcessStartInfo psi = new ProcessStartInfo(CMD_PATH)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            Process p = Process.Start(psi);
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.AutoFlush = true;

            // Progress<T> implementation of IProgress<T> capture current SynchronizationContext,
            // so if you create Progress<T> instance in UI thread, then passed delegate
            // will be invoked in UI thread and you will be able to interact with UI elements.
            Progress<string> writeOutput = new Progress<string>();

            // You possibly want asynchronous wait here, but for simplicity I will use synchronous wait.
            Task stdout = TextReaderAsync(p.StandardOutput, writeOutput);
            stdout.Wait();

            //Task stderr = TextReaderAsync(p.StandardError, writeToConsole); // For curl will output the debug info
            //stderr.Wait();
            p.WaitForExit();
        }

        private static async Task TextReaderAsync(StreamReader standardOutput, IProgress<string> writeOutput)
        {
            char[] buffer = new char[1024];
            while (true)
            {
                int count = await standardOutput.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (count == 0) break;
                string output = new string(buffer, 0, count);
                Debug.WriteLine(output);
                writeOutput.Report(output);
            }
        }
    }
}