using System;
using System.Collections.Generic;
using System.Text;
using AVM.Options;

namespace AVM.Azure
{
    public class UrlStore : IUrlStore
    {
        private readonly EnvironmentVariables _variables;

        public UrlStore(EnvironmentVariables variables)
        {
            _variables = variables;
        }

        public string GetObjectUrl(AvmObjectType objectType, string id)
        {
            string url;

            switch (objectType)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url = $"https://dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/build/definitions/{id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url = $"https://vsrm.dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/release/definitions/{id}?api-version=5.0";
                    break;
                case AvmObjectType.VariableGroup:
                    url = $"https://dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/distributedtask/variablegroups/{id}?api-version=5.0-preview.1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return url;
        }

        public string GetListUrl(AvmObjectType objectType)
        {
            string url;

            switch (objectType)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url = $"https://dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/build/definitions?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url = $"https://vsrm.dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/release/definitions?api-version=5.0";
                    break;
                case AvmObjectType.VariableGroup:
                    url = $"https://dev.azure.com/{_variables.Organization}/{_variables.Project}/_apis/distributedtask/variablegroups?api-version=5.0-preview.1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return url;
        }
    }
}
