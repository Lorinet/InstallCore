using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InstallCorePackagingTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Lorinet InstallCore 0.1f\nInstallation Package Creation Tool\n\nPackaging:\n");
            List<string> files = new List<string>();
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: icpt <image.nim> [file1 file2 ...]");
                Environment.Exit(-1);
            }
            else if (args.Length == 1)
            {
                files = DirSearch(Environment.CurrentDirectory);
            }
            else
            {
                for (int i = 1; i < args.Length; i++)
                {
                    files.Add(args[i]);
                }
            }
            List<byte> pkg = new List<byte>(new byte[] { 0x4E, 0x46, 0x53 });
            int offset = 7;
            for (int i = 0; i < files.Count; i++)
            {
                offset += files[i].Length + 9;
            }
            pkg.AddRange(BitConverter.GetBytes(offset));
            Dictionary<string, byte[]> fileContents = new Dictionary<string, byte[]>();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine(files[i] + " -> " + args[0]);
                if (File.Exists(files[i]))
                    fileContents.Add(files[i], File.ReadAllBytes(files[i]));
                else
                {
                    Console.WriteLine("ERROR: File not found: " + files[i]);
                    Environment.Exit(-2);
                }
            }
            offset = 0;
            for (int i = 0; i < files.Count; i++)
            {
                pkg.AddRange(Encoding.ASCII.GetBytes(files[i]));
                pkg.Add(0);
                pkg.AddRange(BitConverter.GetBytes(offset));
                offset += fileContents[files[i]].Length;
                pkg.AddRange(BitConverter.GetBytes(offset));
            }
            for (int i = 0; i < files.Count; i++)
            {
                pkg.AddRange(fileContents[files[i]]);
            }
            Console.WriteLine("Writing image file " + args[0] + " ...");
            File.WriteAllBytes(args[0], pkg.ToArray());
            Console.WriteLine("The operation completed successfully.");
        }
        static List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (Path.GetFileName(f) != "icpt.exe") files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
            return files;
        }
    }
}
