
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using System.IO;
using IniParser;
using IniParser.Model;

namespace Confi
{
    public class ConfiValueProvider : LookupOrFallbackDefaultValueProvider
    {
        private string _path;
        public ConfiValueProvider(string path)
        {
            _path = path;

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            base.Register(
                typeof(string), 
                (type, mock) => {
                    StackTrace st = new StackTrace();
                    string moq = mock.ToString().Replace("Mock<", "").Replace(":1>", "");

                    //Console.WriteLine($"===={moq}====");

                    var frames = st.GetFrames()
                                    .Select(f => new
                                    {
                                        Method = f.GetMethod()
                                                  .Name
                                                  .Replace("get_", ""),
                                        Classy = f.GetMethod()
                                                    .DeclaringType
                                                    .Name
                                                    .Replace("Proxy", "")
                                    })
                                    .Where(f => f.Classy == moq);

                    
                    //frames.ToList().ForEach(f => Console.WriteLine($"{f.Method}: {f.Classy}"));

                    var frame = frames.First();

                    string filePath = $"{Path.Combine(_path, frame.Classy)}.ini";
                    if (!File.Exists(filePath))
                    {
                        using (FileStream fs = File.Create(filePath))
                            fs.Close();
                    }

                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(filePath);

                    if (!data["Main"].ContainsKey(frame.Method))
                    {
                        data["Main"][frame.Method] = "";
                        parser.WriteFile(filePath, data);
                    }

                    return ( data["Main"][frame.Method] );
                });
            base.Register(typeof(List<>), (type, mock) => Activator.CreateInstance(type));
        }

        public string StringType(string name)
        {

                return name;
            
        }
    }
}