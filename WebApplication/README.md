xunit
xunit.runner.visualstudio
FluentAssertions

## Test
https://fluentassertions.com/introduction
https://github.com/moq/moq4

1. things need to do
- [x] quartz + event store
- [x] quartz + event store + worker service + DI
- [x] quartz + service bus + worker service + DI
- [x] background service + service bus + worker service + DI
- [ ] background service + service bus receive + quartz send + worker service + DI
- [ ] asp.net + quartz + event store + ef + unit/integration test + ef migration
- [ ] asp.net + quartz + event store + nhibernate + unit/integration test + FluentMigrator
- [ ] fluent Nhibernate
- [ ] integrate Auth0 and OpenAM
auto mapper
Json serializer
fluent assertions
oauth0
openam
.gitignore not work when update .gitignore file
use
```bash
git rm -r --cached .
```
to remove all cached files, then use
```bash
git add .
```
commit again

### 0. 进入项目路径
### 1. 清除本地当前的Git缓存
git rm -r --cached .

### 2. 应用.gitignore等本地配置文件重新建立Git索引
git add .

### 3. （可选）提交当前Git版本并备注说明
git commit -m 'update .gitignore'

## Azure Service Bus
https://learn.microsoft.com/zh-cn/azure/service-bus-messaging/explorer
https://learn.microsoft.com/zh-cn/azure/service-bus-messaging/service-bus-dotnet-how-to-use-topics-subscriptions?tabs=connection-string
https://docs.azure.cn/zh-cn/articles/azure-operations-guide/internet-of-things/aog-internet-of-things-howto-select-message-event-service-part-2
https://stackoverflow.com/questions/68688838/how-to-register-servicebusclient-for-dependency-injection
https://learn.microsoft.com/en-us/dotnet/azure/sdk/dependency-injection

## background service
https://learn.microsoft.com/zh-cn/dotnet/core/extensions/windows-service

## Test
[FluentAssertions](https://fluentassertions.com/introduction)

## Asp.net
https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0
https://zhuanlan.zhihu.com/p/467441139

## Git
```bash
git push -f origin main
```
[删除或修改所有git提交历史记录中的文件或内容](https://github.com/newren/git-filter-repo/blob/main/INSTALL.md)
https://blog.gitguardian.com/rewriting-git-history-cheatsheet/
## windows11
快捷键冲突 ctrl+shift+f 切换简繁体,在输入法设置中更改 时间和语言>语言和区域>Microsoft拼音>按键>热键

## EF(Entity Framework Core)
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
# 创建迁移
dotnet ef migrations add InitialCreate
#更新数据库
dotnet ef database update
#更新数据库到对应的版本，如果是旧的版本则会执行drop方法
dotnet ef database update 20210902100000_InitialCreate # (也就是对应的版本)
```

## Middleware
中间件就相当于是应用、数据与用户之间的纽带，它们可以访问应用程序请求和响应的管道，处理请求，然后将请求传递给下一个中间件，或者直接返回响应。
没有什么是加一层解决不了的
[消息中间件对比](https://juejin.cn/post/7137352763058421797)
[什么是消息中间件](https://www.redhat.com/zh/topics/middleware/what-is-middleware)
## asp.net middleware
(处理http请求)
[Middleware](https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0)
