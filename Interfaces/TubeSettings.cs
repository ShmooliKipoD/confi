

namespace Confi
{
    public interface TubeSettings 
    {
        double Power_HVPS { get; set; }

        double Power_Voltage { get; set; }

        bool IsOn { get; set; }
    }
}