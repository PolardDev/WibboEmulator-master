using System;
using System.Collections.Generic;
using System.IO;

namespace Butterfly.Core
{
    public class ConfigurationData
    {
        public Dictionary<string, string> data;
        public bool fileHasBeenRead;

        public ConfigurationData(string filePath, bool maynotexist = false)
        {
            this.data = new Dictionary<string, string>();
            if (!File.Exists(filePath))
            {
                if (!maynotexist)
                    throw new ArgumentException("Unable to locate configuration file at '" + filePath + "'.");
            }
            else
            {
                this.fileHasBeenRead = true;
                try
                {
                    using (StreamReader streamReader = new StreamReader(filePath))
                    {
                        string str;
                        while ((str = streamReader.ReadLine()) != null)
                        {
                            if (str.Length >= 1 && !str.StartsWith("#"))
                            {
                                int length = str.IndexOf('=');
                                if (length != -1)
                                    this.data.Add(str.Substring(0, length), str.Substring(length + 1));
                            }
                        }
                        streamReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Could not process configuration file: " + ex.Message);
                }
            }
        }
    }
}
