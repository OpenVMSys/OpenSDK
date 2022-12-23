using MongoDB.Bson;
using MongoDB.Driver;
namespace OpenSDK.DevKit;

public interface IOpenPlugin
{
    
}
public abstract class Base
{
    protected Version? PluginVersion { get; set; }

    public abstract object OnServiceStart();
    public abstract object OnServiceStop();
    public object? OnVersionGet()
    {
        return PluginVersion;
    }
}

public interface IGet
{
    object OnGet(object data);
}
public interface IPost
{
    object OnPost(object data);
}
public interface IPatch
{
    object OnPatch(object data);
}
public interface IDelete
{
    object OnDelete(object data);
}
public interface IPut
{
    object OnPut(object data);
}
public class ModelBase
{
    protected string? Identifier;
    protected string? Belonging;
    protected bool Active;
}