# FunGame 定制模组

Oshima Studios 巨献

设定可参考：[Oshima Core 文档 | 喔 是吗？](https://docs.milimoe.com) 

FunGame 原项目地址：[https://github.com/project-redbud/FunGame-Core](https://github.com/project-redbud/FunGame-Core) 

---

# OshimaGameModule + FunGame.Server 一键部署指南

本仓库提供了批处理脚本，用于**自动完成**以下所有部署工作：

- 从 GitHub 克隆所需的 4 个仓库（`FunGame-Core`、`FunGame-Server`、`OshimaGameModule`、`SQLQueryExtension`）
- 整理目录结构
- 编译所有解决方案
- 复制插件、配置文件、SQL 脚本
- 启动 Web API 服务（`FunGameWebAPI.exe`）

你**不需要**提前克隆任何东西，只需获取**两个**脚本文件即可。

---

## 系统要求

- **操作系统**：Windows（脚本使用 `mkdir`、`copy`、`xcopy`、`start` 等命令）
- **[.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)**（`dotnet --version` 确认）
- **[Git for Windows](https://git-scm.com/downloads)**（已添加到 `PATH`）

---

## 部署步骤（只需两步）

### 1️⃣ 获取脚本

将以下两个脚本保存到**同一个空文件夹**中：

- `setup.bat`  
- `start-web-api.bat`

### 2️⃣ 运行 `setup.bat`

双击 `setup.bat`（或在命令行中执行）即可。

整个过程**全自动**，包括：
- 克隆四个仓库（该步骤需要网络）
- 编译三个 `.sln` 解决方案
- 复制所有依赖文件到正确位置
- 自动启动 `FunGameWebAPI.exe`

如果编译失败，请查看 `build_logs\build_log.txt` 中的详细日志。

后续运行只需要直接运行 `start-web-api.bat` 即可。

---

## 执行后的目录结构

```
当前文件夹/
├── build_logs/                      # 编译日志
├── FunGame.Core/                    # 核心库
├── FunGame.Extension/
│   └── FunGame.SQLQueryExtension/   # SQL扩展
├── FunGame.Server/                  # Web API服务
│   └── bin/Debug/net10.0/
│       ├── FunGameWebAPI.exe
│       ├── fungame.sql / fungame_sqlite.sql
│       ├── configs/                 # 插件配置文件
│       ├── maps/                    # 地图相关DLL
│       ├── modules/                 # 模块DLL
│       └── plugins/
│           ├── OshimaServer/
│           └── OshimaWebAPI/
├── OshimaGameModule/                # 插件源码
├── setup.bat
└── start-web-api.bat
```

---

## 常见问题

### ❌ `dotnet build` 失败

- 确认已安装符合项目版本的 **.NET SDK**（`dotnet --info`）
- 查看 `build_logs\build_log.txt` 定位具体错误

### ❌ git clone 失败

- 检查网络，确保能访问 `github.com`
- 如果网络限制 HTTPS，可改为 SSH 地址（需提前配置 SSH key）

---

## 手动启动（不重新编译）

如果只需要启动服务（不再编译），可以执行：

```bash
cd FunGame.Server\bin\Debug\net10.0
FunGameWebAPI.exe
```

---

## 许可证

各子项目遵循其各自仓库中的许可证。

---

## 需要帮助？

请查看各子项目的 GitHub Issues，或联系维护者。  
日志文件位于 `build_logs\build_log.txt`，请附上该文件内容以便排查问题。
