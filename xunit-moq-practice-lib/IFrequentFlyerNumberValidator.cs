using System;

namespace RK.Practice.Examples.Xunit.Lib
{
    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
    }
}