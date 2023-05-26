using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.QpData;

public class SiteStructureSerializer
{
    private IAbstractItemContextStorageBuilder _builder;
    private ILogger _logger;

    private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(
        new JsonSerializerSettings
        {
            ContractResolver = new WritablePropertiesOnlyResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        });

    public SiteStructureSerializer(IAbstractItemContextStorageBuilder builder)
    {
        _builder = builder;
    }


    public MemoryStream SerializeData<T>(T data)
    {
        var stream = new MemoryStream();
        var compressedStream = new GZipOutputStream(stream);
        using var writer = new StreamWriter(compressedStream, Encoding.UTF8, bufferSize: -1, leaveOpen: true);
        using var jsonWriter = new JsonTextWriter(writer);

        _serializer.Serialize(jsonWriter, data, typeof(T));
        jsonWriter.Flush();
        compressedStream.Finish();

        return stream;
    }

    public T DeserializeData<T>(byte[] data)
    {
        using var stream = new MemoryStream(data, false);
        using var decompressedStream = new GZipInputStream(stream);
        using var reader = new StreamReader(decompressedStream, Encoding.UTF8);
        using var jsonReader = new JsonTextReader(reader);

        return _serializer.Deserialize<T>(jsonReader);
    }
}
