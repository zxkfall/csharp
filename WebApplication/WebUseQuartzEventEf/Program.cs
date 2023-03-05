using WebUseQuartzEventEf.Domain;

namespace WebUseQuartzEventEf;

public static class Program
{
    public static void Main(string[] args)
    {
        var app = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).Build();
        //only use for demo test
        //如果有上下文的数据库，则 EnsureCreated 方法不执行任何操作。 如果没有数据库，则它将创建数据库和架构。 EnsureCreated 启用以下工作流来处理数据模型更改：
        // 删除数据库。 任何现有数据丢失。
        // 更改数据模型。 例如，添加 EmailAddress 字段。
        // 运行应用。
        // EnsureCreated 创建具有新架构的数据库。
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<AccountContext>();
            context.Database.EnsureCreated();
            // DbInitializer.Initialize(context);
        }
        app.Run();
    }
}