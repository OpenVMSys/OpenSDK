using System;
using MongoDB.Bson;
using MongoDB.Driver;
namespace OpenSDK.DevKit;

public abstract class OpenPlugin
{
    protected Version? PluginVersion;

    protected Version? Version => PluginVersion;
    public abstract object OnServiceStart();
    public abstract object OnServiceStop();
    public object? OnVersionGet()
    {
        return Version;
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
public class ServiceBase
{
    protected static MongoClient DataClient=new();
    protected static IMongoDatabase Database=DataClient.GetDatabase("");
    protected static IMongoCollection<BsonDocument> Collection=Database.GetCollection<BsonDocument>("");

    /**
         * Get All Objects
         * Returns JSON string
         */
    public object Get()
    {
        return Collection.Find(new BsonDocument()).ToList().ToJson();
    }

    /**
         * Get object with certain identifier
         */
    public object Get(string identifier)
    {
        return Collection.Find(Builders<BsonDocument>.Filter.Eq("Identifier", identifier)).ToList().ToJson();
    }

    /**
         * Create an new instance
         * void
         */
    public void Create(BsonDocument obj)
    {
        Collection.InsertOne(obj);
    }

    /**
         * Delete instance with certain identifier
         * long
         */
    public long Delete(string identifier)
    {
        return Collection.DeleteOne(Builders<BsonDocument>.Filter.Eq("Identifier", identifier)).DeletedCount;
    }

    public static object? CallMessage(string channel, string plugin, params object[] data)
    {
        return OpenPluginCore.PluginMessageTransport(channel, plugin, data);
    }
}

public class ModelBase
{
    protected string? Identifier;
    protected string? Belonging;
    protected bool Active;
}