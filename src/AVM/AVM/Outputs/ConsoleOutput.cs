using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AVM.Outputs
{
    public class ConsoleOutput : IOutput
    {
        public void Write(string output)
        {
            Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(output),
                Formatting.Indented));
        }
    }
}
