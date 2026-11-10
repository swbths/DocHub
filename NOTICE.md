# NOTICE · 文枢（DocHub）第三方组件与开源合规声明

本文件面向「文枢（DocHub）」**4.5.20** 版本，说明本软件在构建、分发与运行过程中涉及的第三方组件、其许可证与相应合规义务。文枢本身以 **MIT** 许可证发布（见 [LICENSE](LICENSE)），作者为 **AI**，软件构思由 **chao** 提出。**本文件随软件一同分发**：4.5.20 的安装包与绿色版均在应用目录内附带 `LICENSE` 与 `NOTICE.md` 的副本；任何再分发本软件的个人或组织应一并保留这两份文件。

---

## 一、合规结论

1. **无第三方源代码拷贝。** 全部业务逻辑与界面代码（22 个 C# 文件，命名空间 `DocHub`）均为原创实现，仓库内不存在从其他开源项目复制而来的代码片段、版权头或 SPDX 标识。
2. **直接第三方依赖仅 1 个。** 唯一的直接 NuGet 依赖是 **PDFsharp 6.2.4**，采用 **MIT** 许可证，与本项目的 MIT 许可完全兼容，不存在许可冲突。
3. **自包含发布的 .NET 运行时属随包分发组件。** 安装包携带 .NET 10.0.12 运行时与 Windows Desktop 运行时（均为 MIT），分发时须保留 .NET Foundation 的版权与许可声明。
4. **安装器为本项目自研的 .NET 自包含程序，不再使用 NSIS。** `dist/文枢_安装程序_v4.5.20.exe` 由 `installer/Setup.csproj` 编译（源码 `installer/Program.cs`，约 1300 行原创 C#），以当前用户身份安装、不需要管理员权限；构建流程中不再调用 `makensis`，仓库内也不含任何 NSIS 脚本，因而不涉及 zlib/libpng 许可义务。
5. **Office 与 LibreOffice 为运行时外部程序，不随包分发。** 本软件仅以进程调用方式使用用户本机已安装的 Microsoft Word 或 LibreOffice，不捆绑、不修改、不分发其任何文件，因此不构成衍生作品，不产生许可证传染。
6. **不存在强著佐权（copyleft）组件。** 全量依赖树中无 GPL / AGPL / LGPL 组件；LibreOffice 虽为 MPL-2.0，但仅以子进程方式调用其可执行文件，未链接其代码、未分发其文件，MPL-2.0 义务不触发。

---

## 二、随软件分发的第三方组件

以下组件实际存在于发布产物（`bin/Release/net10.0-windows10.0.19041.0/win-x64/` 及安装包）中，版本号经 `DocHub.deps.json` 与 NuGet `*.nuspec` 核实。

| 组件                                                                       | 版本            | 用途                                   | 许可证                                                                                                 | 分发形式                                                                                                                                                                                                     | 合规义务                                                                       |
| ------------------------------------------------------------------------ | ------------- | ------------------------------------ | --------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| PDFsharp                                                                 | 6.2.4         | PDF 页级复制与合并、图片成页输出（`MergeEngine.cs`） | MIT                                                                                                 | 随程序附带 `PdfSharp.dll`，以及 `PdfSharp.BarCodes` / `PdfSharp.Charting` / `PdfSharp.Cryptography` / `PdfSharp.Quality` / `PdfSharp.Shared` / `PdfSharp.Snippets` / `PdfSharp.System` / `PdfSharp.WPFonts` 等程序集 | 保留 `Copyright (c) 2005-2026 empira Software GmbH` 与 MIT 许可声明；不得以作者名义为衍生品背书 |
| Microsoft.Extensions.Logging.Abstractions                                | 8.0.3         | PDFsharp 的传递依赖：日志抽象接口                | MIT                                                                                                 | 随程序附带 `Microsoft.Extensions.Logging.Abstractions.dll`                                                                                                                                                    | 保留版权与 MIT 许可声明                                                             |
| Microsoft.Extensions.DependencyInjection.Abstractions                    | 8.0.2         | PDFsharp 的传递依赖：依赖注入抽象接口              | MIT                                                                                                 | 随程序附带 `Microsoft.Extensions.DependencyInjection.Abstractions.dll`                                                                                                                                        | 保留版权与 MIT 许可声明                                                             |
| .NET 运行时（`Microsoft.NETCore.App.Runtime.win-x64`）                        | 10.0.12       | 自包含发布的基础运行时（BCL、JIT、GC 等）            | MIT                                                                                                 | 随安装包分发完整运行时文件集合                                                                                                                                                                                          | 保留 `Copyright (c) .NET Foundation and Contributors` 与 MIT 声明               |
| .NET Windows Desktop 运行时（`Microsoft.WindowsDesktop.App.Runtime.win-x64`） | 10.0.12       | WinForms 窗体、GDI+ 与系统绘制支撑             | MIT                                                                                                 | 随安装包分发完整桌面运行时文件集合                                                                                                                                                                                        | 保留 .NET Foundation 版权与 MIT 声明                                              |
| Microsoft.Windows.SDK.NET.Ref（Windows SDK 投影）                            | 10.0.19041.57 | WinRT 互操作（`Windows.Data.Pdf` 等）      | Windows SDK 许可条款（`licenseUrl`: <https://aka.ms/WinSDKLicenseURL；\`requireLicenseAcceptance\`=true）> | 随包分发 `WinRT.Runtime.dll`、`Microsoft.Windows.SDK.NET.dll` 等                                                                                                                                               | 遵守该 SDK 许可条款；该包元数据未声明 MIT，不得按 MIT 引用                                       |

> 说明：上表中 `.NET` 运行时条目在自包含发布中会展开为数百个托管程序集与原生库（如 `coreclr.dll`、`System.Private.CoreLib.dll`、`WindowsBase.dll` 等），它们同属一个 MIT 许可的运行时产品，合规义务一致，故合并列示。
>
> 安装程序 `DocHubSetup.exe`（`installer/Setup.csproj`）本身同样是自包含的 .NET 应用，其内嵌的运行时与上表条目同源同许可；它与主程序使用同一套 MIT 组件，不引入任何额外的第三方构建工具。



---

## 三、运行时调用的外部程序（未随包分发）

| 外部程序                                      | 调用方式                                                                 | 是否随本软件分发 | 为何不构成衍生作品                                                                 |
| ----------------------------------------- | -------------------------------------------------------------------- | -------- | ------------------------------------------------------------------------- |
| Microsoft Word                            | COM 自动化，ProgID `Word.Application`（`MergeEngine.cs`），将 Word 文档导出为 PDF | 否        | 本软件不提供、不捆绑、不激活 Office；仅在用户本机已安装 Word 时通过公开自动化接口调用，属"程序间正常交互"，非链接、非修改、非再分发 |
| LibreOffice                               | 子进程调用本机 `soffice.exe` 完成 Word → PDF 转换（Word 缺失时的备选引擎）                | 否        | 仅以独立进程方式调用可执行文件，未链接其任何库、未修改其代码、未分发其文件，因此 MPL-2.0 的源代码开放义务不被触发             |
| Windows.Data.Pdf / DWM / user32 / gdi32 等 | 通过 WinRT 投影与 P/Invoke 调用操作系统 API                                     | 否        | 属 Windows 操作系统组件，由系统授权用户使用；本软件仅调用公开 API                                   |

PdfViewerForm 使用 `Windows.Data.Pdf` 渲染 PDF 预览；当系统未安装任何 PDF 阅读器时，本软件通过注册表探测本机 PDF 关联并以系统能力兜底，不引入额外第三方库。

---

## 四、代码溯源与审计方法

本次审计针对 4.1.0 版本的完整工作区执行，方法如下：

1. **版权头与 SPDX 扫描**：对全部 22 个业务源码文件（`DocHub` 命名空间，仓库根目录 `*.cs`）执行 `grep`，检索 `Copyright`、`SPDX-License-Identifier`、`Licensed under` 等标识 —— 结果为空，确认无外部代码携带的版权声明；安装器与启动器另有 3 个 `.cs`（`installer/Program.cs`、`installer/launcher/Launcher.cs`、`installer/launcher/LauncherAssemblyInfo.cs`）同样核查，其中仅 `LauncherAssemblyInfo.cs` 含本项目自有的 `AssemblyCopyright("© 2026 AI")` 程序集属性，无任何外部版权头，结果一致为空。
2. **来源注释与链接扫描**：检索 `http://`、`https://`、`Source:`、`Copied from`、`Stack Overflow` 等痕迹 —— 业务代码中无外部来源引用（仅 README 与项目文件包含官方站点链接）。
3. **依赖树核实**：解析发布产物中的 `DocHub.deps.json`，逐项列出实际参与编译与分发的程序集及其精确版本，避免"仅声明未使用"或"隐式引入"的漏项。
4. **许可证字段核实**：读取 NuGet 缓存中 `~/.nuget/packages/pdfsharp/6.2.4/pdfsharp.nuspec`，确认 `<authors>PDFsharp Team</authors>`、`<license type="expression">MIT</license>`、`<projectUrl>https://docs.pdfsharp.net/</projectUrl>`。
5. **逐文件 `using` 引用核对**：确认每个源文件的命名空间引用均指向 .NET BCL、WinForms、PDFsharp 或本项目内部类型，无未知第三方命名空间。
6. **素材生成方式核查**：提示音由 `SoundLibrary.cs` 以正弦波 + 包络在内存中合成 WAV（5 种音效），无外部音频文件；应用图标由 `Art.DrawMark` 代码矢量绘制，`app.ico` 经 `tools/make_icon.ps1` 用 `System.Drawing` 程序化生成，均无第三方素材授权问题；模糊玻璃效果由 `GlassEffects.cs` 自研降采样高斯模糊（`FrostFactor = 9`），未使用系统 Acrylic / Mica。
7. **构建脚本核查**：`tools/package.ps1` 仅调用 `dotnet publish`、.NET Framework 的 `csc.exe`（编译精简启动器）与 `Unblock-File`，无网络下载行为；仓库内已无 NSIS 脚本。

**审计日期：2026-11-04**｜**审计范围：4.1.0 全量依赖与源码**｜**审计结论：合规，无阻塞项。**

---

## 五、许可证原文

### 5.1 MIT License — PDFsharp

适用组件：PDFsharp 6.2.4 及其子程序集。

```
MIT License

Copyright (c) 2005-2026 empira Software GmbH

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

> 脚注：`PdfSharp.dll` 内含 ICC 色彩配置数据（内嵌 3024 字节 `sRGB2014` 配置文件，其 `cprt` 标签为 `Copyright International Color Consortium, 2015`），随 PDFsharp 6.2.4 原样分发，本项目未作修改。

### 5.2 MIT License — .NET Runtime

适用组件：Microsoft.NETCore.App.Runtime.win-x64 10.0.12、Microsoft.WindowsDesktop.App.Runtime.win-x64 10.0.12、Microsoft.Extensions.Logging.Abstractions 8.0.3、Microsoft.Extensions.DependencyInjection.Abstractions 8.0.2。

```
MIT License

Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

> 说明：`WinRT.Runtime.dll` 与 `Microsoft.Windows.SDK.NET.dll` 源自微软 C#/WinRT 项目，但承载它们的 `Microsoft.Windows.SDK.NET.Ref` NuGet 包元数据挂的是 Windows SDK 许可条款且要求接受许可，二者须分别确认；本项目仅引用其投影类型，未修改其内容。

### 5.3 安装程序不涉及第三方安装器许可

4.1.0 起，`dist/文枢_安装程序_v4.5.20.exe` 由本项目自研的 .NET 安装器（`installer/Setup.csproj`）编译产出，  
构建链中不再使用 NSIS 或其他第三方安装器框架，因此不存在需要在此列示的安装器许可证原文。  
安装器内嵌的运行库载荷与 §5.2 所列 .NET 组件同源同许可。

---

## 六、合规建议与待办

1. **安装目录随附声明（4.1.0 已完成）**：`tools/package.ps1` 在 `dotnet publish` 之后把仓库根的 `LICENSE` 与 `NOTICE.md` 复制进 layout（根目录与 `app\` 应用目录各一份）。绿色版 zip 由该 layout 直接打包，已实测：根目录含 `LICENSE`、`NOTICE.md`，另有 `app\LICENSE`、`app\NOTICE.md`。安装包把同一 layout 压成 `installer/payload.zip` 后内嵌进安装器 exe，安装后位于 `%LOCALAPPDATA%\Programs\DocHub\` 根目录与 `app\` 子目录。后续版本沿用该打包逻辑即可自动携带，无需人工补拷。
2. **发布前代码签名**：当前未购买代码签名证书，SmartScreen 会对未签名程序给出提示；取得证书后应对 `DocHub.exe`、启动器与安装程序统一签名，既改善用户体验，也便于用户核对发布者。
3. **升级依赖时复核许可证**：PDFsharp 若从 6.2.4 升级，须重新核对 `nuspec` 中的 `<license>` 字段，并同步更新本文件的版本号与原文段落。
4. **复核 `Microsoft.Windows.SDK.NET.Ref` 的可再分发条款**：该包元数据仅提供 `licenseUrl`（<https://aka.ms/WinSDKLicenseURL）且> `requireLicenseAcceptance=true`，未以 SPDX 标识声明 MIT；自包含发布将其若干程序集随包分发，建议发布前阅读该条款，必要时向微软确认再分发授权范围，并在确认后回填本节结论。
5. **引入新组件的前置评审**：新增第三方依赖前评估许可证类型，**禁止**引入 AGPL；LGPL 需采用动态链接并保留替换能力；MPL 组件若被修改或随包分发，须按其要求公开相应源代码。
6. **LibreOffice 使用边界**：保持"仅以子进程调用、不分发其文件、不修改其安装"的现状；若未来改为随包附带 LibreOffice，则必须重新评估 MPL-2.0 义务并在安装包内附其许可证与源代码获取方式。
7. **安装器载荷维护**：`installer/payload.zip` 是构建期中间产物（已加入 `.gitignore`），由 `tools/package.ps1` 生成并在打包结束后删除；若手工构建安装器，需先运行打包脚本或自行生成该文件，否则安装器会在启动时报「安装包内容缺失」。
8. **文档同步**：作者署名变更（作者 = AI，软件构思 = chao）已同步至 LICENSE、`DocHub.csproj`、安装程序项目（`installer/Setup.csproj`）、安装器界面文案、启动器程序集信息、六种语言的关于页文案与 README；后续如再调整署名，需同样全量同步本文件与上述位置。

---

## 七、变更记录

| 日期         | 版本    | 说明                                                                                                                                                                                                       |
| ---------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2026-11-04 | 4.1.0 | 首次建立，完成全量依赖审计（源码溯源、deps.json 依赖树、nuspec 许可证字段、素材生成方式、构建脚本核查），并随 4.1.0 版本分发；同日经复核修订：`Microsoft.Windows.SDK.NET.Ref` 改为 Windows SDK 许可条款、PDFsharp 版权行改为 `2005-2026 empira Software GmbH`、补 ICC 数据脚注、收窄审计口径 |
| 2026-11-04 | 4.1.0 | 打包方式由 NSIS 改为 .NET 自包含安装器：删除组件表中的 NSIS 行、删除 §5.3 zlib/libpng 许可原文段、§1 结论第 4 条与 §四 第 7 条改写、§六 待办第 7 条改为安装器载荷维护说明；`tools/installer.nsi` 已从仓库删除，构建链不再包含任何第三方安装器框架                                           |
