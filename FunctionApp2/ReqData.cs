using System;
using System.Collections.Generic;

namespace FunctionApp2
{
    public class ReqData
    {
        public DateTime? StartOn { get; set; }
        public DateTime? EndOn { get; set; }

        public string[] GetValidationResult()
        {
            var errorList = new List<string>();
            if (StartOn == null) errorList.Add($"Field {nameof(StartOn)} is required");
            if (EndOn == null) errorList.Add($"Field {nameof(EndOn)} is required");

            return errorList.ToArray(); 
        }
    }
}
