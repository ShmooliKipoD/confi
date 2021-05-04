
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Reflection;

namespace Confi
{
    public class ConfiValueProvider : LookupOrFallbackDefaultValueProvider
    {
        private string _path;
        public ConfiValueProvider(string path)
        {
            _path = path;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            base.Register(
                typeof(string),
                (type, mock) =>
                {
                    StackTrace st = new StackTrace();
                    string moq = mock.ToString().Replace("Mock<", "").Replace(":1>", "");
                    return GetValue(st, moq);
                });
            base.Register(typeof(bool), (type, mock) => 
                {
                    StackTrace st = new StackTrace();
                    string moq = mock.ToString().Replace("Mock<", "").Replace(":1>", "");
                    return bool.Parse(GetValue(st, moq, "false"));
                });
            base.Register(typeof(double), (type, mock) => 
                {
                    StackTrace st = new StackTrace();
                    string moq = mock.ToString().Replace("Mock<", "").Replace(":1>", "");
                    return double.Parse(GetValue(st, moq, 0.0.ToString()));
                });
        }

        public string GetValue(StackTrace st, string moq, string defaultValue = "")
        {
            var frames = st.GetFrames()
                            .Select(f => new
                            {
                                Base = f.GetMethod(),

                                Method = f.GetMethod()
                                            .Name
                                            .Replace("get_", ""),

                                Classy = f.GetMethod()
                                            .DeclaringType
                                            .Name
                                            .Replace("Proxy", "")
                            })
                            .Where(f => f.Classy == moq);

            var frame = frames.First();

            string filePath = $"{Path.Combine(_path, frame.Classy)}.ini";
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                    fs.Close();
            }

            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(filePath);

            string[] sectionAndName = frame.Method.Split("_");

            if (sectionAndName.Length == 1)
            {
                if (!data["Main"].ContainsKey(frame.Method))
                {
                    data["Main"][frame.Method] = defaultValue;
                    parser.WriteFile(filePath, data);
                }

                return data["Main"][frame.Method];
            } else if(sectionAndName.Length == 2)
            {
                string section = sectionAndName[0];
                string name = sectionAndName[1];
                if (!data[section].ContainsKey(name))
                {
                    data[section][name] = defaultValue;
                    parser.WriteFile(filePath, data);
                }

                return data[section][name];
            }

            throw new Exception("Bam ... ");
        }

        public string StringType(string name)
        {

            return name;

        }
    }
}