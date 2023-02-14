# Entity Framework Core

https://learn.microsoft.com/zh-cn/ef/core/

dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update


Could not load file or assembly 'C:\Users\xingkun.zhang\.dotnet\tools\.store\dotnet-ef\7.0.2\dotnet-ef\7.0.2\tools\net6.0\any\tools\netcorea
pp2.0\any\ef.dll'. The located assembly's manifest definition does not match the assembly reference. (0x80131040)
File name: 'C:\Users\xingkun.zhang\.dotnet\tools\.store\dotnet-ef\7.0.2\dotnet-ef\7.0.2\tools\net6.0\any\tools\netcoreapp2.0\any\ef.dll'

查看csproj文件，看看ef版本和项目版本是否一致，比如都是6.0
https://learn.microsoft.com/en-us/answers/questions/905521/could-not-load-file-or-assembly-class-library-micr

参考这个 要用依赖注入的方式
https://learn.microsoft.com/zh-cn/ef/core/dbcontext-configuration/

https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/startup?view=aspnetcore-7.0
!!! 直接创建一个空的asp.net项目，不要创建webapi 不然会有冲突，很奇怪

!!! 项目名不能叫ef，不然会报错。。。。。