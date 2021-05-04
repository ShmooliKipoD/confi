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
            Console.WriteLine(tubeSettings.Power_Voltage);
            Console.WriteLine(tubeSettings.Power_HVPS);
            Console.WriteLine(tubeSettings.IsOn);

        }
    }
}
