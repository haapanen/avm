using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AVM.Azure
{
    public interface IAzureClient
    {
        Task<string> GetAsync(string url);
        Task<string> PutAsync(string url, string content);
    }
}
