# 数据库软删除示例

本示例演示了数据实体上隐藏“IsDeleted”属性，在EF层实现数据软删除、查找已删除的数据和恢复操作。

## 环境

* MySQL 8.0
* .Net Core 2.2 Release
* Pomelo.EntityFrameworkCore.MySql 2.1.4

## 数据库

默认数据库连接信息为：

1. 数据库地址`localhost`
2. 数据库`test`
3. 账号`root`
4. 密码`123456`

请根据你数据库设置修改`appsettings.json`文件中的数据库连接信息。

建议使用Docker启动MySQL容器，命令如下。

```sh
docker run -it --rm -p3306:3306 -e MYSQL_ROOT_PASSWORD="123456" mysql --default-authentication-plugin=mysql_native_password
```

## 运行

    git clone https://github.com/scfido/softdelete.git
    cd softdelete
    dotnet restore
    dotnet run

## 演示

因为此项目没有页面，需要使用HTTP请求工具调用Web API来演示。我使用的工具是`PostMan`，可以在Chrome扩展中安装它。

通过以下操作可以观察到数据库`User`表的`IsDeleted`字段值的变化。再查看我们的`User`实体和普通的EF查询语句，并没有添加`IsDeleted`属性和它的过滤条件。也就是是说在不改变对象实体和查询条件的前提下，统一的方式实现了数据的软删除。

```csharp
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }
    }

```

## 添加用户

```http
POST /api/user HTTP/1.1
Host: localhost:5000
Content-Type: application/x-www-form-urlencoded

Name=%22fuyun%22&UserId=0
```

## 获取用户

GET http://localhost:5000/api/user

返回：

```json
    [
        {
        "userId": 1,
        "name": "\"fuyun\""
        }
    ]
```

## 删除用户

    DELETE http://localhost:5000/api/user/1

返回：200 OK

## 获取被删除的用户

    GET http://localhost:5000/api/user/RecycleBin

返回:

```json
    [
        {
        "userId": 1,
        "name": "\"fuyun\""
        }
    ]
```

## 恢复被删除的用户

    POST localhost:5000/api/user/Recover/1

返回：200 OK

## 参考

* 原文：[Entity Framework Core: Soft Delete using Query Filters](https://www.meziantou.net/2017/07/10/entity-framework-core-soft-delete-using-query-filters)
* 中文翻译版：[Entity Framework Core 软删除与查询过滤器](http://www.cnblogs.com/tdfblog/p/entity-framework-core-soft-delete-using-query-filters.html)