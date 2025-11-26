## NovaFramework - Unity 启动库

NovaFramework的启动库，提供程序启动的标准流程。  
如果您需要定制个性化的启动流程，可以参考该库中驱动引擎的具体操作方式，并自行扩展前后的自定义逻辑。  

关于更多引擎介绍，请前往[NovaFramework](https://github.com/yoseasoft/NovaFramework)查看。

## 使用文档

## 注意事项

使用方式(任选其一)

1. 直接在 `manifest.json` 的文件中的 `dependencies` 节点下添加以下内容：
    ```json
        {"com.novaframework.unity.core.boot": "https://github.com/yoseasoft/com.novaframework.unity.core.boot.git"}
    ```

2. 在Unity 的`Packages Manager` 中使用`Git URL` 的方式添加库,地址为：
https://github.com/yoseasoft/com.novaframework.unity.core.boot.git

3. 直接下载仓库放置到Unity 项目的`Packages` 目录下，会自动加载识别。
