using EventStore.Client;

namespace QuartzAndEventWorkerService;

public class ClientHelper {
 
    //创建 SingleObject 的一个对象
    private static readonly ClientHelper Instance = new ClientHelper();
 
    //让构造函数为 private，这样该类就不会被实例化
    private ClientHelper(){}
 
    //获取唯一可用的对象
    public static ClientHelper GetInstance(){
        return Instance;
    }
 
    public EventStoreClient GetClient(){
        var settings = EventStoreClientSettings
            .Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");
        var eventStoreClient = new EventStoreClient(settings);
        return eventStoreClient;
    }
}