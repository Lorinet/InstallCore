using System;
using System.Collections.Generic;
using System.IO;

namespace System
{
    /// <summary>
    /// FlowBase configuration file processing library.
    /// </summary>
    public class FlowBase
    {
        /// <summary>
        /// Gets the value of an action parameter.
        /// </summary>
        /// <param name="name">The identifier of the action.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns a string containing the value of the specified action.</returns>
        public static string GetAction(string name, string[] code)
        {
            string retval = "";
            foreach(string s in code)
            {
                string[] line = s.Split(' ');
                if(line[0] == "action" && line[1] == name)
                {
                    bool ovr = false;
                    if (line[2].StartsWith("@"))
                    {
                        line[2] = line[2].Remove(0, 2);
                        ovr = false;
                    }
                    else line[2] = line[2].Remove(0, 1);
                    for(int i = 2; i < line.Length; i++)
                    {
                        if (!ovr) retval += line[i].Replace("\\n", "\n").Replace("\\\n", "\\\\n") + " ";
                        else retval += line[i] + " ";
                    }
                }
            }
            return retval.Remove(retval.Length - 2, 2);
        }
        /// <summary>
        /// Gets all items from a base.
        /// </summary>
        /// <param name="name">The identifier of the base.</param>
        /// <param name= "code">The FlowBase code to process.</param>
        /// <returns>Returns an array of strings containing all items from the base.</returns>
        public static string[] GetBase(string name, string[] code)
        {
            List<string> content = new List<string>();
            bool isbase = false;
            foreach(string s in code)
            {
                string[] line = s.Split(' ');
                if (!isbase)
                {
                    if (line[0] == "base" && line[1] == name)
                    {
                        isbase = true;
                    }
                }
                else
                {
                    if (s.StartsWith("."))
                    {
                        content.Add(s.Remove(0, 1));
                    }
                    else if (s == "endbase") isbase = false;
                }
            }
            return content.ToArray();
        }
        /// <summary>
        /// Read all lines of an external file specified in the FlowBase code
        /// </summary>
        /// <param name="name">The identifier of the external.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns an array of strings that contains the lines of the external file.</returns>
        public static string[] GetExternalAllLines(string name, string[] code)
        {
            string[] retval = { };
            foreach (string s in code)
            {
                string[] line = s.Split(' ');
                if (line[0] == "external" && line[1] == name)
                {
                    retval = File.ReadAllLines(line[2]);
                }
            }
            return retval;
        }
        /// <summary>
        /// Read all text of an external file specified in the FlowBase code
        /// </summary>
        /// <param name="name">The identifier of the external.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns a string that contains the contents of the external file.</returns>
        public static string GetExternalAllText(string name, string[] code)
        {
            string retval = "";
            foreach (string s in code)
            {
                string[] line = s.Split(' ');
                if (line[0] == "external" && line[1] == name)
                {
                    retval = File.ReadAllText(line[2]);
                }
            }
            return retval;
        }
        /// <summary>
        /// name the Sharpown file.
        /// </summary>
        /// <param name="name">The identifier of the extend sequence.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns a string array containing the new extended FlowBase code.</returns>
        public static string[] Extend(string name, string[] code)
        {
            List<string> retval = new List<string>();
            retval.AddRange(code);
            foreach (string s in code)
            {
                string[] line = s.Split(' ');
                if (line[0] == "extend" && line[1] == name)
                {
                   retval.AddRange(File.ReadAllLines(line[2]));
                }
            }
            return retval.ToArray();
        }
        /// <summary>
        /// Get the document type of the file that is using FlowBase.
        /// </summary>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns a string containing the type of FlowBase document.</returns>
        public static string GetType(string[] code)
        {
            string retval = "";
            foreach (string s in code)
            {
                string[] line = s.Split(' ');
                if (line[0] == ".type")
                {
                    retval = line[1];
                }
            }
            return retval;
        }
        /// <summary>
        /// Removes a base from a FlowBase code.
        /// </summary>
        /// <param name="name">The identifier of the base to remove.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns an array of strings containing the FlowBase code without the selected base.</returns>
        public static string[] RemoveBase(string name, string[] code)
        {
            List<string> basestocheck = new List<string>(code);
            int baseindex = 0;
            bool isbase = false;
            foreach(string s in basestocheck)
            {
                string[] splat = s.Split(' ');
                if(!isbase)
                {
                    if (splat[0] == "base" && splat[1] == name)
                    {
                        isbase = true;
                        basestocheck.RemoveAt(baseindex);
                        baseindex -= 1;
                    }
                }
                else
                {
                    if (splat[0] != "endbase")
                    {
                        basestocheck.RemoveAt(baseindex);
                        baseindex -= 1;
                    }
                    else
                    {
                        isbase = false;
                        basestocheck.RemoveAt(baseindex);
                        baseindex -= 1;
                    }
                }
                baseindex += 1;
            }
            return basestocheck.ToArray();
        }
        /// <summary>
        /// Removes an action from the FlowBase code.
        /// </summary>
        /// <param name="name">The identifier of the action to remove.</param>
        /// <param name="code">The FlowBase code to process.</param>
        /// <returns>Returns an array of strings containing the FlowBase code without the selected action.</returns>
        public static string[] RemoveAction(string name, string[] code)
        {
            List<string> actionstocheck = new List<string>(code);
            int actionindex = 0;
            foreach (string s in actionstocheck)
            {
                string[] splat = s.Split(' ');
                if(splat[0] == "action" && splat[1] == name)
                {
                    actionstocheck.RemoveAt(actionindex);
                }
                actionindex += 1;
            }
            return actionstocheck.ToArray();
        }
        /// <summary>
        /// Generates an action.
        /// </summary>
        /// <param name="ActionIdentifier">The identifier of the action.</param>
        /// <param name="ActionValue">The value of the action.</param>
        /// <returns>Returns a string containing the generated action.</returns>
        public static string Action(string ActionIdentifier, string ActionValue)
        {
            return "action " + ActionIdentifier + " " + ActionValue;
        }
        /// <summary>
        /// Generates a base.
        /// </summary>
        /// <param name="BaseIdentifier">The identifier of the base.</param>
        /// <param name="BaseValue">The contents of the base.</param>
        /// <returns>Returns an array of strings containing the generated base.</returns>
        public static string[] Base(string BaseIdentifier, string[] BaseValue)
        {
            List<string> basetoret = new List<string>();
            basetoret.Add("base " + BaseIdentifier);
            foreach(string item in BaseValue)
            {
                basetoret.Add("." + item);
            }
            basetoret.Add("endbase");
            return basetoret.ToArray();
        }
    }
}
