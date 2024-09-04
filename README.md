# qluLib.net

[qluLib.net](https://github.com/iyo11/qluLib.net)，C#开发的齐鲁工业大学图书馆自动预约程序

---

## 通过Docker部署到Linux服务器

- 下载项目到本地并上传到服务器目录
- 填写 **Program.cs**中需要填写的信息 **[👤userName,🔒passWord]**
- Shell下进入项目 **qluLib.net/Dockerfile** 同级目录下
- 执行 **docker build -t 【自定义Docker镜像名】 .** 命令
- **docker run --name 【自定义Container命名】 【自定义Docker镜像名】** 启动程序