using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVM.Models;
using AVM.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVM.Commands
{
    public class SetCommand : BaseCommand, ICommand
    {
        private readonly SetOptions _options;

        public SetCommand(EnvironmentVariables variables, SetOptions options) : base(variables)
        {
            _options = options;
        }

        public async Task<int> ExecuteAsync()
        {
            string url = null;

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                case AvmObjectType.BuildVariables:
                    url = $"https://dev.azure.com/{Organization}/{Project}/_apis/build/definitions/{_options.Id}?api-version=5.0";
                    break;
                case AvmObjectType.Release:
                case AvmObjectType.ReleaseVariables:
                    url = $"https://vsrm.dev.azure.com/{Organization}/{Project}/_apis/release/definitions/{_options.Id}?api-version=5.0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var existing = JsonConvert.DeserializeObject<JObject>(await Get(url));

            switch (_options.Type)
            {
                case AvmObjectType.Build:
                    break;
                case AvmObjectType.Release:
                    break;
                case AvmObjectType.BuildVariables:
                    var newValues =
                        JsonConvert.DeserializeObject<Dictionary<string, Variable>>(await File.ReadAllTextAsync(_options.SourceFilePath,
                            Encoding.UTF8));

                    foreach (var kvp in newValues)
                    {
                        var existingVariable = existing["variables"][kvp.Key];
                        if (existingVariable == null)
                        {
                            existingVariable = new JObject();
                            existing["variables"][kvp.Key] = existingVariable;
                        }

                        if (kvp.Value == null)
                        {
                            existing["variables"][kvp.Key] = null;
                        }
                        else
                        {
                            existingVariable["value"] = kvp.Value.Value;
                            existingVariable["allowOverride"] = kvp.Value.AllowOverride;
                            existingVariable["isSecret"] = kvp.Value.IsSecret;
                        }
                    }

                    break;
                case AvmObjectType.ReleaseVariables:
                    var releaseVariables =
                        JsonConvert.DeserializeObject<ReleaseVariables>(
                            await File.ReadAllTextAsync(_options.SourceFilePath, Encoding.UTF8));

                    foreach (var kvp in releaseVariables.Variables)
                    {
                        var existingVariable = existing["variables"][kvp.Key];
                        if (existingVariable == null)
                        {
                            existingVariable = new JObject();
                            existing["variables"][kvp.Key] = existingVariable;
                        }
                        if (kvp.Value == null)
                        {
                            existing["variables"][kvp.Key] = null;
                        }
                        else
                        {
                            existingVariable["value"] = kvp.Value.Value;
                            existingVariable["allowOverride"] = kvp.Value.AllowOverride;
                            existingVariable["isSecret"] = kvp.Value.IsSecret;
                        }
                    }

                    foreach (var env in releaseVariables.Environments)
                    {
                        var environments = existing["environments"].AsJEnumerable();
                        var matchingEnv = environments.FirstOrDefault(e => (int) e["id"] == env.Id);

                        if (matchingEnv != null)
                        {
                            foreach (var kvp in env.Variables)
                            {
                                var existingVariable = matchingEnv["variables"][kvp.Key];
                                if (existingVariable == null)
                                {
                                    existingVariable = new JObject();
                                    matchingEnv["variables"][kvp.Key] = existingVariable;
                                }
                                if (kvp.Value == null)
                                {
                                    matchingEnv["variables"][kvp.Key] = null;
                                }
                                else
                                {
                                    existingVariable["value"] = kvp.Value.Value;
                                    existingVariable["allowOverride"] = kvp.Value.AllowOverride;
                                    existingVariable["isSecret"] = kvp.Value.IsSecret;
                                }
                                existingVariable["value"] = kvp.Value.Value;
                                existingVariable["allowOverride"] = kvp.Value.AllowOverride;
                                existingVariable["isSecret"] = kvp.Value.IsSecret;
                            }
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var response = await Put(url, JsonConvert.SerializeObject(existing));

            return 0;
        }

        private Func<dynamic, bool> GetMatch(int envId)
        {
            return o => o.id == envId;
        }
    }
}
