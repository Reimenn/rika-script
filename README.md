NEW URL [Gitee](https://gitee.com/LrikaM/rika-script)

![logo](Logo.png)



![Title](Title.png)

## 起步

快速创建一个 RikaScript 运行环境：

```c#
var rs = new Engine(new ConsoleLogger());
rs.Execute("log('Hello, World')");
```

运行即可在控制台看到 Hello, World。

快速构建一个 RikaScript 尝鲜环境：

```c#
var rs = new Engine(new ConsoleLogger());
while (true)
    rs.Execute(Console.ReadLine());
```

通过 RikaScript ，可以快速调用预先准备好的 C# 方法并进行简单的编程计算：

```js
log("Hello, World")
// [INFO] Hello, World
set num = 2.33
log(num)
// [INFO] 2.33 
```

## 中缀表达式和函数调用

这个语言一个特点就是“函数=运算符”，看似是运算符的操作其实都是语法糖：

```js
log(1 + 1)
// 等同于
log(+(1,1))
```

只不过对函数进行了额外的封装，使其可以正确计算优先级关系：

```js
log(1 + 2 * 3) 
// 7
```

这个语法产生的缺点，导致运算符两侧必须加上空格，`1+1`是不正确的写法。

另外注意只有两个参数的函数可以这样写：

```js
make 1 // 错误
1 make // 错误
1 + 1 // 正确
1 log 2 //正确，因为log有一个两个参数的重载
```

## 数据类型

RikaScript 是弱类型的脚本语言，数据在 C# 运行环境中都保存为object类型，在执行函数时会转换成相应的数据类型。

RikaScript 中默认数据类型是 `long` `double` 和`string`，用 long 和 double 不用担心性能浪费问题，~~因为解释这个语言本身就已经浪费了很多性能~~

```js
log(type(1))
// [INFO] System.Int64
log(type(1.0))
// [INFO] System.Double
log(1 / 2)
// [INFO] 0
```

类型转换用`int` `float` `str`三个函数，虽然它们叫做int、float，但结果还是转换成了对应的 `long` `double` `string` 类型。

## 流程控制

#### func\while\if

RikaScript 以 func 定义过程：

```js
func Hello{
	log("这里是Hello过程")
}
```

定义好的过程会存在内存中，通过`call`关键字调用

```js
call Hello
// [INFO] 这里是Hello过程
```

RikaScript 支持 if 和 while 语句，他们的代码体可以是一个 func：

```js
func loopMe{
	log(num)
    set num += 1
}
set num = 0
while num < 5 loopMe
// 输出 0 1 2 3 4
```

当然这样写很憨憨，所以我还是通过语法糖实现了正常的 while 或 if 写法

```js
set num = 0
while num < 10 {
	if num % 2 == 0{
    	log(num)
    }
    set num += 1
}
// 输出 0 2 4 6 8
```

注意这个语言的代码块开头括号`{`必须写到定义代码块的那一行最后，不能写在下一行，结束代码块的`}`要求单独写一行，前后不能有其他东西，除了空格。

> RikaScript 中，过程、变量、函数、类库，都存放在不同的字典中，所以他们之间可以重名

#### if xxx return

使用 `if xxx return` 的方式可以退出当前代码体，所以不建议将某个方法命名为 return。

这种操作在 while 中使用就类似 continue：

```js
set n = 0
while n < 10{
    set n += 1
    log(n)
    if n > 4 return
    log('----------')
}
// 前几行带有横线分隔，后面几行没有。
```

在 if 中使用就忽然结束这段代码了：

```js
if true{
	log(1)
    if true return
    log(2)
}
// 只有 1 ，没有 2
```

在 func 中使用就类似在 if 中使用的效果，或类似 return：

```js
func test{
    log(1)
    if true return
    log(2)
}
call test
// 只有 1 ，没有 2
```

不再任何代码段里写，就可以结束当前文件的执行，其实还是类似 return。

## 类库

每个 RikaScript 运行时都会自带上 std 类库，这个库在 C# 中对应的 class 是 RikaScript.Libs.StandardLib ，其中包含了 RikaScript 几乎必备的函数，且这些函数都不能被覆盖。

```js
std.log("Woooooooo~")
// [INFO] Woooooooo~
```

如果要使用其他类库，可以在 C# 代码中对 Runtime 运行环境调用 AddLib 方法添加一个类库：

```c#
// C# 代码，实例化异步 RikaScript 执行引擎
var run = new AsyncEngine(new ConsoleLogger());
// 添加文件操作类库
run.Runtime.AddLib(new FileLib()); // 这里可以给类库一个别名，这个类库别名默认是 file
```

> 可惜的是 v0.6 版本中删除了 FileLib 类库，计划 v0.7 中重做

然后在 RikaScript 中通过别名调用（当然我没有改过别名）：

```js
file.write("D:\test.txt", 'Wooooooo~')
// 然后你的磁盘可用空间就少了12字节
```

> 每个类库都会尽量提供一个help()方法用来显示帮助，例如 file 类库：
>
> ```scala
> file.help()
> ```
>
> ```
> FileLib:file - RikaScript 基本文件交互库
>          read(path) - 读取文件内容并返回
>          write(path, text) - 覆盖写入文件
>          append(path, text) - 追加写入文件
>          set_encoding(encoding) - 设定类库采用的文件编码，例如utf-8、gbk
>          set_new_line(str) - 指定追加文件时的前置符号，默认是换行符
> ```

如果不想在 C# 中导入类库，还可以在代码中使用 std 类库的 import 方法导入，这要求类库必须有一个可用的空参构造方法：

```js
// 随机数类库作为例子，这个库的默认别名是random，嫌长就可以改成r来用
std.import("RikaScript.Libs.RandomLib", "r") // 可惜的是这个随机数类库也在v0.6中删除了
std.log(r.range(0, 100))
// [INFO] 12.5500561262248
```

当然你可能会纳闷，为啥之前写代码都没加`std`前缀，这是因为导入每一个类库函数的时候，都会存两份，一份有类库前缀，一份没有类库前缀，所以两种方式都可以调用类库。

当然如果函数名重名了，后导入的无前缀函数名会覆盖之前那个，这时就只能用带前缀的方式调用函数了。

> 写C#类库时，可以通过给 Method 特征加上 Keep = true 来让这个函数不会被覆盖，例如 std 类库中的全部方法都是这样做的。

## C# 代码架构介绍

### 运行环境

运行环境主要指 `Runtime`、`Engine` 这两个类

整个系统最核心的就是 Runtime 类，这个类实现了代码的逐行执行、变量报错、参数传递、注释忽略等工作，但它不能支持结构性的语句，比如func、if、call等，只能调用函数和保存变量，这个类的 Execute 方法可以一次遍历解决一条字符串代码。

Engine 对 Runtime 进行一些包装，实现了复杂的语法，可以完整的执行上文介绍过的那些代码，例如 if、while、func等。这里还存储了过程、提供了set、if xxx {}这样的语法糖支持

还有个 AsyncEngine 继承自 Engine ，实现了异步执行，让 RikaScript 不干扰主线程任务。但没怎么测试过稳定性，请谨慎使用。

### 扩展类库

RikaScript 的类库都要继承自 `ScriptLibBase` 类。这个父类中实现了一些每个类库的基本功能，比如类库信息和获取帮助相关代码。

给 class 增加 Library 特征标签来设定一些额外信息，这几乎是必须加上的

RikaScript 导入类库时，不论是通过在C#中AddLib还是 RikaScript 中 import ，实质上都是实例化了一个类库对象，因为可以通过 imprt 时指定别名来同时引入多个相同的类库。

基于 ScriptLibBase 扩展自己的函数时，需要给方法增加 Method 特征标签来暴露函数，这种函数要求返回值和参数必须都是object类型。需要注意 RikaScript 可直接调用的函数的返回值和参数类型必须都是 object，因此这些函数重载只能通过不同参数数量来实现，并且参数数量最大不能超过4个。

Method 中包含别名设置、优先级设置、持久设置和帮助设置，例如 std 库的方法：

```c#
[Method(Name = "+", Priority = 10, Keep = true)] // 别名、优先级、持久
public object add(object a, object b){...}
[Method(Name = "*", Priority = 100, Keep = true)]
public object mul(object a, object b){...}
[Method(Keep = true, Help = "显示一下全部变量")] // 加了帮助
public void show_vars(){...}
```

在代码中使用时：

```js
log(1 + 2 * 3) // 结果肯定是7了
log(+(1,2) * 3) // 结果是9
```

### 日志输出

Runtime 类会用到 `LoggerBase` 类输出信息，继承 LoggerBase 来实现自己的输出，例如为 Unity 实现一个输出类：

```c#
using UnityEngine;
using RikaScript.Logger;

public class UnityLogger : LoggerBase
{
    public override void Print(object message) {
        Debug.Log(message);
    }

    public override void Info(object message) {
        Debug.Log(message);
    }

    public override void Warning(object message) {
        Debug.LogWarning(message);
    }

    public override void Error(object message) {
        Debug.LogError(message);
    }
}
```

然后在创建 RikaScript 时指定上这个类即可：

```c#
var rs = new Engine(new UnityLogger());
```

### 工具类

工具类是 `ScriptTools`，可以查看源码获得帮助

在扩展类库开发中，把 object 类型数据转换成基本数据类型的方法就写在了这个工具里

## RikaScript 语法总览


|语法|意义|
| ------ | ---- |
|set *var* = *value*|变量赋值|
|func *name* {|开始定义名为 name 的方法|
|if *value* {|开始定义一个if代码段|
|while *value* {|开始定义一个条件循环代码段|
|}|结束代码段方法|
|call *func*|执行一个方法|
|if *value* return|根据一个变量，判断是否停止当前方法或文件|
|if *value* *func*|根据一个变量，判断是否执行一个方法|
|while *value* *func*|根据一个变量，判断是否反复执行一个方法|
|exec *path*|执行一个 RikaScript 文件|
|help|显示std类库的帮助|

