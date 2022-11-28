namespace OpenSDK.Security;

public class Service
{
    public readonly static List<Model> KeyCollection = new();
    private FileStream? _file;
    private StreamReader? _reader;
    private string _keyFilePath = Path.Join("OpenKey");
    /*
     * Security以明文保存于keyFilePath中
     * 读取keyFilePath文件并序列化其内容
     */
    public void Get()
    {
        try
        {
            _file = new(_keyFilePath, FileMode.Open);
            _reader = new StreamReader(_file, System.Text.Encoding.UTF8);
            var rawData = _reader.ReadToEnd();
            var keys = rawData.Split("\n");
            KeyCollection.Clear();
            foreach (var key in keys)
            {
                var obj = key.Split("\t");
                if (obj.Length > 1)
                {
                    KeyCollection.Add(new Model(key.Split("\t")[0], int.Parse(key.Split("\t")[1])));
                }
            }
            //关闭读写流
            _reader.Close();
            _file.Close();
        }
        catch
        {
            File.WriteAllText(_keyFilePath, "");
        }
    }
    /*
     * 核验请求的key是否有效
     * 若key存在且要求permission小于key的permission，则通过
     */
    public bool Auth(string authKey, OmsConfiguration.SecurityPermission permission)
    {
        Get();
        foreach (var key in KeyCollection)
        {
            
            if (key.Key != null && key.Key.Equals(key) && permission.CompareTo(key.Permission) >= 0)
            {
                return true;
            }
        }
        return false;
    }
}