# AGENTS.md

本文档为 Claude Code（claude.ai/code）以及其他 AI 编码代理（Cursor、Aider、Copilot CLI 等）在本仓库工作时提供项目指引。

## 仓库概述

这是 **Oshima Studios 为 FunGame 引擎制作的模组（addon pack）**，并非独立可运行的应用程序。每个项目都会生成一个 DLL，由宿主 FunGame 运行时作为插件加载。上游引擎仓库：<https://github.com/project-redbud/FunGame-Core>。

代码中的大多数标识符、角色 / 技能 / 物品类名以及游戏内字符串均为**中文（CJK）**。源文件统一使用 UTF‑8 编码，请勿"规范化"这些标识符名称。

## 构建与运行

解决方案目标框架为 **`.NET 10.0`**（WPF 项目使用 `net10.0-windows7.0`）。使用标准 .NET SDK 构建：

```sh
dotnet build OshimaGameModule.sln              # 构建全部项目
dotnet build OshimaWebAPI/OshimaWebAPI.csproj  # 构建单个项目
dotnet run   --project OshimaWebAPI            # 开发服务器: https://localhost:11108, http://localhost:11109
```

### 必需的同级仓库

`OshimaGameModule.sln` 通过相对路径引用了**本目录以外**的项目：

| 引用                                                   | 被谁依赖                          |
| ------------------------------------------------------ | --------------------------------- |
| `../FunGame.Core/FunGame.Core.csproj`                  | 所有项目                          |
| `../FunGame.Extension/FunGame.SQLQueryExtension/...`   | `OshimaWebAPI`、`OshimaServers`   |

构建前请将这些仓库克隆为 `OshimaGameModule/` 的**同级目录**，否则 restore 阶段会失败。如果只想跑起整套环境，仓库根目录的 `setup.bat` / `start-web-api.bat` 会自动完成克隆、编译、插件 DLL 分发与启动——详见 [README.md](./README.md)。

### 输出目录布局

除 `OshimaCore` 外的 4 个项目（`OshimaMaps` / `OshimaModules` / `OshimaServers` / `OshimaWebAPI`）均设置 `<BaseOutputPath>..\bin\</BaseOutputPath>`，因此它们的构建产物会汇集到解决方案根目录的 **`OshimaGameModule/bin/`**，而不是各项目自己的 `bin/` 文件夹。`OshimaCore` 本身没设置此属性，但因被其它 4 个项目 `ProjectReference`，其 DLL 也会被复制进同一个聚合目录。宿主 FunGame 运行时正是从此聚合目录中发现并加载插件。

### 平台说明

- 当前 `.sln` 中所有项目都以 `net10.0`（非 `-windows`）为目标，整套构建**跨平台**可执行。
- `OshimaWebAPI` 是 ASP.NET Core 项目（`FrameworkReference Microsoft.AspNetCore.App`），无额外平台依赖。

仓库中**没有测试项目**。

## 架构：各插件如何协作

```
依赖图（箭头读作"被依赖"）：

OshimaCore ─┬─► OshimaMaps    ─┐
            ├─► OshimaModules ─┼─► OshimaServers ─► OshimaWebAPI
            └──────────────────┘   (WebAPI 同时直接 ProjectReference Core + Modules)
```

实际 csproj `ProjectReference` 关系：

| 项目 | 直接依赖的本仓库项目 |
| --- | --- |
| `OshimaCore`    | —                                       |
| `OshimaMaps`    | `OshimaCore`                            |
| `OshimaModules` | `OshimaCore`                            |
| `OshimaServers` | `OshimaCore`、`OshimaMaps`、`OshimaModules` |
| `OshimaWebAPI`  | `OshimaCore`、`OshimaModules`、`OshimaServers` |

每个项目的 `RootNamespace` 都按 `Oshima.FunGame.$(MSBuildProjectName)` 的模板生成（`OshimaCore` 例外 → `Oshima.Core`）。

### 各项目职责

| 项目             | 职责                                                                                                                                          | 插件类与 FunGame 基类                                                                              |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| `OshimaCore`     | 常量、基于 JSON 的设置（`GeneralSettings`、`Daily`、`SayNo`、`Ignore`、`QQOpenID`、`BlackList`）                                                | `OSMCore.InitOSMCore()` 启动入口；本身不是插件类                                                   |
| `OshimaModules`  | 实体定义（角色、技能、物品、特效、地区、单位）                                                                                                | `CharacterModule`、`SkillModule`、`ItemModule` —— 继承自 FunGame 的 `Library.Common.Addon.*Module` |
| `OshimaMaps`     | `GameMap` 实现                                                                                                                                | `FastAutoMap`、`AnonymousMap`                                                                      |
| `OshimaServers`  | 游戏服务器插件 + 业务服务层（`Service/FunGameService.cs`、`FunGameSimulation.cs`、`OnlineService.cs`、`SQLService.cs`、`CSBettingService.cs`） | `OshimaServer`（`ServerPlugin`）、`FastAutoServer` 与 `AnonymousServer`（`GameModuleServer`）      |
| `OshimaWebAPI`   | ASP.NET Core 控制器 + 定时任务 + QQ / RainBOT 集成                                                                                            | `OshimaWebAPI : WebAPIPlugin, IHotReloadAware, ILoginEvent`                                        |

### 插件标识符集中在常量表中

所有插件名、addon ID 以及 `GameModuleDepend` 依赖图都集中在 **`OshimaCore/Constant/Constant.cs`** 的 `OshimaGameModuleConstant` 类中。新增一种 addon 时，请在此注册它的 kebab-case ID —— 宿主引擎正是通过这些字符串来连接依赖关系的。

### 实体注册模式（重要）

`CharacterModule` / `SkillModule` / `ItemModule` 各自覆写一个 `*Factory()` 方法，返回的委托通过一个庞大的 `switch` 按整数 ID 分派（见 `OshimaModules/Modules/*.cs`）。要新增一个角色 / 技能 / 物品，需要：

1. 在合适的目录下定义实体类（如 `Characters/`、`Skills/魔法/`、`Items/Weapon/`）。
2. 在对应的 ID 枚举中加一项（`SkillID`、`MagicID`、`SuperSkillID`、`PassiveID`、`ItemPassiveID`、`WeaponID` 等 —— 见 `Skills/SkillID.cs`、`Items/ItemID.cs`）。
3. 在该模块的工厂 `switch` 中加一行 `case (long)YourEnum.X => new X()`。

仓库中**没有**基于反射的自动发现机制；这些 `switch` 表就是唯一的事实来源。

### 持久化

通过宿主 FunGame 运行时提供两层持久化：

- **`PluginConfig`**（位于宿主配置目录下的 key/value JSON 文件，例如 `new PluginConfig("rainbot", "config")`）—— 用于 `GeneralSettings`、每日 / 黑名单状态、各服务器的统计数据（`FunGameSimulation.StatsConfig`、`FastAutoServer.StatsConfig`）。
- **`SQLHelper`**（在插件中通过 `Controller.GetSQLHelper()` 获取，配合 `NewTransaction()` / `Rollback()` / `Commit()` 使用）—— 用于用户、库存、角色 / 物品所有权。`OshimaWebAPI/Constant/*.sql` 下的脚本是游戏专属表（如 `CSBetting.sql`）的参考 SQL。

### 生命周期钩子（FunGame addon 契约）

所有插件都实现 `IHotReloadAware`。扩展时请留意以下方法：

- `AfterLoad(loader, params object[] objs)` —— 接入 DI / `SQLHelper` / 事件处理器。对于 `OshimaWebAPI`，`objs[0]` 是 `WebApplicationBuilder` —— 在此注册 scoped 服务。
- `OnWebAPIStarted(params object[] objs)` —— 对 `WebAPIPlugin` 而言，`objs[0]` 是已运行的 `WebApplication`。此处会调用 `FunGameConstant.InitFunGame()`、`FunGameSimulation.InitFunGameSimulation()`，并注册**所有 `TaskScheduler.Shared.AddTask(...)` / `AddRecurringTask(...)` 定时任务**（见 `OshimaWebAPI.cs`）。
- `OnBeforeUnload()` —— 必须按名称移除自己曾添加的每个定时任务，否则热重载会泄露重复任务。

### 定时任务（在 `OshimaWebAPI.cs` 注册）

任务名为中文；新增任务时记得在 `OnBeforeUnload` 中按相同名称移除：

```
"重置每日运势"   每日 00:00 — Daily.ClearDaily / 活动缓存 / 公告 / 商店预刷新
"上九" / "下三"  每日 09:00 / 15:00 — 物品交易冷却重置 + 地区天气刷新
"刷新存档缓存"   每 1 分钟    — RefreshSavedCache、RefreshClubData、RoomsAutoDisband
"刷新每日任务"   每日 04:00   — 每日任务、签到、商店、市场、活动缓存、公告
"刷新boss"      每 1 小时
"刷新活动缓存"   每 4 小时
```

### 认证（WebAPI）

`OshimaWebAPI` 接入了 FunGame 的 `WebAPIAuthenticator.WebAPICustomBearerTokenAuthenticator`。token 仅与 **`GeneralSettings.TokenList`**（从 `rainbot/config` 的 `PluginConfig` 加载）中的列表做匹配。带 `[Authorize(AuthenticationSchemes = "CustomBearer")]` 的控制器需要鉴权（见 `Controllers/FunGameController.cs`）；标记 `[AllowAnonymous]` 的接口为有意公开（`/funGame/test`、`/funGame/last`、`/funGame/stats`）。

### 两类不同的 "Server" 插件，容易混淆

- **`OshimaServer : ServerPlugin`** —— 接入宿主服务器的控制台（`.osm` 指令、商店事件）。
- **`FastAutoServer` / `AnonymousServer : GameModuleServer`** —— 真正的游戏房间逻辑。`FastAutoServer` 通过 `MixGamingQueue` 跑自动对战循环（见 `FastAutoServer.cs` 的 `StartGame`）；`AnonymousServer` 是供外部 bot 使用的、基于 token 的 WebSocket 桥。

两类插件会同时加载进同一个运行时 —— 互不替代。

## 值得遵循的约定

- 新的字符串 ID 一律加入 `OshimaGameModuleConstant`；不要在代码中散布 `"oshima.fungame.xxx"` 这类字面量。
- 新 addon 必须**同时**更新实体工厂和对应的 ID 枚举。
- 在 `OnWebAPIStarted` 中通过 `TaskScheduler.Shared.AddTask` 注册的任务，必须在 `OnBeforeUnload` 中以同名 `RemoveTask` 注销 —— 热重载会反复执行两边，否则任务会成倍累积。
- 大量使用中文标识符是有意为之。`.csproj` 中关闭了 `CS8981`（小写标识符）与 `IDE1006`（命名风格）正是这个原因 —— 请勿重新启用。
- WebAPI 开发配置使用 `ASPNETCORE_ENVIRONMENT=Development`，监听 `https://localhost:11108;http://localhost:11109`（见 `OshimaWebAPI/Properties/launchSettings.json`）。
