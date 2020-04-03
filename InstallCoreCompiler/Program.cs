using InstallCoreDeploy;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace InstallCore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: icdc <image.nim> <setup.exe> <installinfo.fb>");
                Environment.Exit(-1);
            }
            Console.WriteLine("InstallCore Setup Compiler Version 0.1f\n\n" + args[0] + " => " + args[1]);
            Console.WriteLine("Loading setup information...");
            string[] inf = File.ReadAllLines(args[2]);
            string appName = FlowBase.GetAction("Name", inf);
            string appDescription = FlowBase.GetAction("Description", inf);
            string developer = FlowBase.GetAction("Developer", inf);
            string installPath = FlowBase.GetAction("InstallPath", inf).Replace("\\", "\\\\");
            string[] postActions = FlowBase.GetBase("PostInstallActions", inf);
            Console.WriteLine("Configuring setup information...");
            string installerData = "public static string InstallPath = \"" + installPath + "\";\npublic static string AppName = \"" + appName + "\";\npublic static string AppDescription = @\"" + appDescription.Replace("\"", "\"\"") + "\";\npublic static string AppProvider = \"" + developer + "\";\npublic static string InstallImageName = \"" + args[0] + "\";\npublic static string[] PostInstallCommands = {";
            foreach (string s in postActions) installerData += "\"" + s + "\", ";
            installerData += "\n};";
            MinInstaller.InstallerCode = MinInstaller.InstallerCode.Replace("{INSTALLERDATA_PLACEHOLDER}", installerData);
            Console.WriteLine("Embedding resources...");
            File.WriteAllBytes("background.png", MinInstaller.GradientBackground);
            File.WriteAllBytes("icon.ico", MinInstaller.Icon);
            File.WriteAllText("app.manifest", MinInstaller.SetupManifest);
            string[] referenceAssemblies = { "System.dll", "System.Drawing.dll", "System.Windows.Forms.dll" };
            CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters compilerParameters = new CompilerParameters(referenceAssemblies, "");
            compilerParameters.OutputAssembly = args[1];
            compilerParameters.GenerateExecutable = true;
            compilerParameters.GenerateInMemory = false;
            compilerParameters.WarningLevel = 3;
            compilerParameters.TreatWarningsAsErrors = false;
            compilerParameters.CompilerOptions = "/optimize /target:winexe /win32icon:icon.ico /win32manifest:app.manifest";
            compilerParameters.EmbeddedResources.Add("background.png");
            compilerParameters.EmbeddedResources.Add(args[0]);
            string errors = null;
            Console.WriteLine("Compiling generated C# source...");
            try
            {
                CompilerResults compilerResults = null;
                compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, MinInstaller.InstallerCode);
                if (compilerResults.Errors.Count > 0)
                {
                    errors = "";
                    foreach (System.CodeDom.Compiler.CompilerError CompErr in compilerResults.Errors)
                    {
                        errors += "Line number " + CompErr.Line +
                        ", Error Number: " + CompErr.ErrorNumber +
                        ", '" + CompErr.ErrorText + ";\r\n\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }
            if (errors == null)
            {
                Console.WriteLine("Setup program compiled successfully!");
            }
            else
            {
                Console.WriteLine("Error occurred during compilation : \r\n" + errors);
            }
            Console.WriteLine("Cleaning up...");
            if (File.Exists("background.png")) File.Delete("background.png");
            if (File.Exists("icon.ico")) File.Delete("icon.ico");
            if (File.Exists("app.manifest")) File.Delete("app.manifest");
        }
    }
}
