using MongoDB.Bson;
using MongoDB.Driver;
namespace OpenSDK.DevKit;

/// <summary>
/// Helper for MongoDB CRUD actions
/// </summary>
public class MongoServiceHelper
{
    private readonly IMongoDatabase _database;
    private IMongoCollection<BsonDocument>? _collection;

    public MongoServiceHelper(string connectionString, string databaseName)
    {
        try
        {
            var dataClient = new MongoClient(connectionString);
            _database = dataClient.GetDatabase(databaseName);
        }
        catch (Exception e)
        {
            Logger<MongoServiceHelper>.Error("Error when initializing the Helper:",e.Message);
            throw;
        }
    }
    public struct QueryBody
    {
        public string FindItem { get; }
        public string FindValue { get; }
        public string? TargetItem { get; }
        public string? TargetValue { get; }

        /// <summary>
        /// Construct an new QueryBody.  创建一个新的QueryBody实例
        /// </summary>
        /// <param name="findValue">要查找的项对应的值</param>
        /// <param name="findItem">要查找的项</param>
        /// <param name="targetValue">要修改为的值</param>
        /// <param name="targetItem">要修改的项</param>
        public QueryBody(string findItem,string findValue, string? targetItem, string? targetValue)
        {
            TargetValue = targetValue;
            TargetItem = targetItem;
            FindValue = findValue;
            FindItem = findItem;
        }
    }
    
    /// <summary>
    /// Get items from the given collection name.
    /// 从指定表中获取数据元素
    /// </summary>
    /// <param name="collectionName">表名称</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>object or null</returns>
    public object Get(string collectionName, QueryBody? queryBody)
    {
        _collection = _database.GetCollection<BsonDocument>(collectionName);
        return queryBody!=null ? 
            _collection.Find(
                Builders<BsonDocument>.Filter.Eq(queryBody.Value.FindItem,queryBody.Value.FindValue)
                ).ToList().ToJson() 
            : _collection.Find(
                new BsonDocument()
                ).ToList().ToJson();
    }
    
    /// <summary>
    /// Insert an new document to given collection name.
    /// 向指定表插入新数据元素
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="documents">要插入的新元素</param>
    public void Insert(string collectionName, params BsonDocument[] documents)
    {
        _collection = _database.GetCollection<BsonDocument>(collectionName);
        _collection.InsertMany(documents);
    }
    
    /// <summary>
    /// Update an existing document, returns the count of affected items.
    /// 修改已存在的数据元素，并返回受影响的行数
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>受影响的行数</returns>
    public long? Set(string collectionName, QueryBody queryBody)
    {
        if (queryBody.TargetItem == null || queryBody.TargetValue == null)
        {
            Logger<SqlServiceHelper>.Warning(
                "Trying to perform",
                "an setting query without",
                "targetItem or targetValue!",
                "In case of any unexpected error,",
                "this action will not be performed",
                "and a null has been returned.");
        }
        else
        {
            _collection = _database.GetCollection<BsonDocument>(collectionName);
            return _collection.UpdateOne(
                Builders<BsonDocument>.Filter.Eq(queryBody.FindItem, queryBody.FindValue),
                Builders<BsonDocument>.Update.Set(queryBody.TargetItem, queryBody.TargetValue)
            ).ModifiedCount;
        }

        return null;
    }
    
    /// <summary>
    /// Delete an existing item in the given collection.
    /// 删除指定表中的指定数据元素
    /// *必须指定：FindItem | FindValue
    /// </summary>
    /// <param name="collectionName">表名称</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>受影响的行数</returns>
    public long Delete(string collectionName, QueryBody queryBody)
    {
        _collection = _database.GetCollection<BsonDocument>(collectionName);
        return _collection.DeleteOne(
            Builders<BsonDocument>.Filter.Eq(queryBody.FindItem, queryBody.FindValue)
            ).DeletedCount;
    }
}