using System;

namespace FunctionApp.Services
{
    public interface IDataverseService
    {
        bool? IsReady { get; }

        void CleanRecords();
        void ConfigureDublicateDetection();
        bool HasDublicate(DateTime start, DateTime end);
        void WhoAmI();
        void AddEntity(DateTime start, DateTime end);
        void CheckDublicates(DateTime start, DateTime end);
    }
}
