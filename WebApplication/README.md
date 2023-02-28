xunit
xunit.runner.visualstudio
FluentAssertions

## Test
https://fluentassertions.com/introduction
https://github.com/moq/moq4

1. things need to do
- [x] quartz + event store
- [x] quartz + event store + worker service + DI
- [ ] quartz + service bus + worker service + DI
- [ ] asp.net + quartz + event store + ef + unit/integration test + ef migration
- [ ] asp.net + quartz + event store + nhibernate + unit/integration test + FluentMigrator
- [ ] fluent Nhibernate
- [ ] integrate Auth0 and OpenAM

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

# 0. 进入项目路径
# 1. 清除本地当前的Git缓存
git rm -r --cached .

# 2. 应用.gitignore等本地配置文件重新建立Git索引
git add .

# 3. （可选）提交当前Git版本并备注说明
git commit -m 'update .gitignore'