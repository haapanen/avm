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
            try
            {
                Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(output),
                    Formatting.Indented));
            }
            catch (JsonException)
            {
                Console.WriteLine(output);
            }
        }
    }
}
