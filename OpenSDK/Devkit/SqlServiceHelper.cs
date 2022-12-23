using MySqlX.XDevAPI;

namespace OpenSDK.DevKit;
/// <summary>
/// Helper for Sql database CRUD actions
/// </summary>
public class SqlServiceHelper
{
    private readonly Session _session;
    private readonly Schema _schema;

    public SqlServiceHelper(string connectionString,string schemaName)
    {
        try
        {
            _session = MySQLX.GetSession(connectionString);
            _schema = _session.GetSchema(schemaName);
        }
        catch (Exception e)
        {
            Logger<SqlServiceHelper>.Error("Error when initializing the Helper", e.Message);
            throw;
        }
    }
    
    public struct QueryBody
    {
        public string FindItem { get; }
        public object FindValue { get; }
        public object? TargetItem { get; }
        public object? TargetValue { get; }

        /// <summary>
        /// 构建一个新的queryBody
        /// </summary>
        /// <param name="findItem">查找的条件，如"_id = :id"</param>
        /// <param name="findValue">定义条件，如new { "id" = "testId" }</param>
        /// <param name="targetItem">要修改的项</param>
        /// <param name="targetValue">要修改为的值</param>
        public QueryBody(string findItem, object findValue, object? targetItem, object? targetValue)
        {
            FindValue = findValue;
            FindItem = findItem;
            TargetItem = targetItem;
            TargetValue = targetValue;
        }
    }
    /// <summary>
    /// Get items from the given collection name.
    /// 从指定表中获取数据元素
    /// </summary>
    /// <param name="collectionName">表名称</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>object or null</returns>
    public object? Get(string collectionName,QueryBody queryBody)
    {
        var collection = _schema.GetCollection(collectionName);
        var foundDocs = collection.Find(queryBody.FindItem).Bind(new DbDoc(queryBody.FindValue)).Execute();
        return foundDocs;
    }

    /// <summary>
    /// Insert an new document to given collection name.
    /// 向指定表插入新数据元素
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="item2BInsert">要插入的新元素</param>
    public void Insert(string collectionName, params object[] item2BInsert)
    {
        var collection = _schema.GetCollection(collectionName);
        collection.Add(item2BInsert).Execute();
    }

    /// <summary>
    /// Update an existing document, returns the count of affected items.
    /// 修改已存在的数据元素，并返回受影响的行数
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>受影响的行数</returns>
    public ulong? Set(string collectionName, QueryBody queryBody)
    {
        var collection = _schema.GetCollection(collectionName);
        var docParams = new DbDoc(queryBody.FindValue);
        if (queryBody.TargetItem == null || queryBody.TargetValue == null)
        {
            Logger<SqlServiceHelper>.Warning(
                "Trying to perform",
                "an setting query without",
                "a targetItem or targetValue!",
                "In case of any unexpected error,",
                "this action will not be performed",
                "and a null has been returned.");
        }
        else
        {
            var r = collection.Modify(queryBody.FindItem)
                .Bind(docParams)
                .Set(queryBody.TargetItem.ToString(), queryBody.TargetValue)
                .Execute();
            return r.AffectedItemsCount;
        }
        return null;
    }

    /// <summary>
    /// Delete an existing item in the given collection.
    /// 删除指定表中的指定数据元素
    /// *必须指定：QueryCondition | BindDocParams
    /// </summary>
    /// <param name="collectionName">表名称</param>
    /// <param name="queryBody">查找的条件</param>
    /// <returns>受影响的行数</returns>
    public ulong Delete(string collectionName, QueryBody queryBody)
    {
        var collection = _schema.GetCollection(collectionName);
        var r = collection.Remove(queryBody.FindItem).Bind(queryBody.FindValue).Execute();
        return r.AffectedItemsCount;
    }

    /// <summary>
    /// Destroy this session
    /// </summary>
    public void Destroy()
    {
        _session.Close();
    }
}