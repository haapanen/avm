using AVM.Options;

namespace AVM.Azure
{
    public interface IUrlStore
    {
        string GetListUrl(AvmObjectType objectType);
        string GetObjectUrl(AvmObjectType objectType, string id);
    }
}
