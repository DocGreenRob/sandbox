using System;
using System.Collections.Generic;
using System.IO;

namespace This.Utils
{
    public static class FileWriter
    {
        private const string DIR_PATH = @"C:\_dev\_sandbox";
        private const string FILE_PATH = @"C:\_dev\_sandbox\Data.txt";

        public static bool Write(string text)
        {
            try
            {
                if (!Directory.Exists(DIR_PATH))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(DIR_PATH);
                }
            }
            catch (IOException ioex)
            {
                throw new Exception("Unable to create directory, please run as Administrator!");
            }

            try
            {
                bool fileExists = File.Exists(FILE_PATH);

                if (!fileExists)
                {
                    // WriteAllLines creates a file, writes a collection of strings to the file,
                    // and then closes the file.  You do NOT need to call Flush() or Close().
                    File.WriteAllText(FILE_PATH, text);

                    //File.Create(FILE_PATH);
                    //TextWriter tw = new StreamWriter(FILE_PATH);
                    //tw.WriteLine(text);
                    //tw.Close();
                }
                else
                {
                    //TextWriter tw = new StreamWriter(path);
                    //tw.WriteLine("The next line!");
                    //tw.Close();

                    // Example #4: Append new text to an existing file.
                    // The using statement automatically flushes AND CLOSES the stream and calls 
                    // IDisposable.Dispose on the stream object.

                    using (StreamWriter file =
                        new StreamWriter(FILE_PATH, true))
                    {
                        file.WriteLine(text);
                    }
                }

                return true;
            }catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public static string[] Read()
        {
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = File.ReadAllLines(FILE_PATH);
            return lines;
        }

        public static bool Delete()
        {
            try
            {
                File.WriteAllText(FILE_PATH, string.Empty);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        
    }
}
