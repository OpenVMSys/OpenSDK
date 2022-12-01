using MongoDB.Bson;
using MongoDB.Driver;
namespace OpenSDK.DevKit;

public abstract class OpenPlugin
{
    private Version? PluginVersion { get; set; }

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
public class ServiceBase
{
    protected static MongoClient DataClient { get; set; }
    private static IMongoDatabase Database { get; set; }
    protected static IMongoCollection<BsonDocument> Collection { get; set; }

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
    
    public long Transfer(string identifier, string target)
    {
        return Collection.UpdateOne(
            Builders<BsonDocument>.Filter.Eq("Identifier", identifier),
            Builders<BsonDocument>.Update.Set("Belonging", target)).ModifiedCount;
    }
}

public class ModelBase
{
    protected string? Identifier;
    protected string? Belonging;
    protected bool Active;
}