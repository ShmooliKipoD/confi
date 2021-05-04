using System;
using Moq;

namespace Confi
{
    class Program
    {
        static void Main(string[] args)
        {
            Mock<TubeSettings> mock = new Mock<TubeSettings> { DefaultValueProvider = new ConfiValueProvider("Configuration") };
            TubeSettings tubeSettings = mock.Object;  // => "?"
            Console.WriteLine(tubeSettings.Power_voltage);
            Console.WriteLine(tubeSettings.Power_hvps);
        }
    }
}
