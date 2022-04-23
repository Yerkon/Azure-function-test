using System;

namespace FunctionApp2.Services
{
    public interface IDataverseService
    {
        void CleanRecords();
        void ConfigureDublicateDetection();
        bool HasDublicate(DateTime start, DateTime end);
        void WhoAmI();
        void AddEntity(DateTime start, DateTime end);
        void CheckDublicates(DateTime start, DateTime end);
    }
}
