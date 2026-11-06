# 文枢 · 方案 B 界面改造说明

> 状态：代码已写入 `MainForm.cs` / `UiControls.cs` / `SettingsForm.cs` / `GlassEffects.cs` / `AppSettings.cs`。
> **本次环境无法编译**（PowerShell 启动报 `0xC0000142`，`dotnet build` 跑不起来），所以请在本机执行一次编译确认。

## 1. 新的主窗口结构（1000 × 660 客户区）

```
┌──────┬─────────────────────────────────┬──────────────────┐
│      │  文件列表  3 个文件   ↑ ↓ 🗑 ⌫ +添加 │  输出文件 [____].pdf │
│ 品牌 │ ┌─────────────────────────────┐ │  保存到   [____][浏览]│
│ 合并 │ │ ①  PDF  采购合同_主协议.pdf   │ │  ────────────────  │
│ 设置 │ │ ②  WORD 技术方案_v3.docx      │ │  就绪 · 按列表顺序… │
│ 关于 │ │ …（悬停行浮出 ↑ ↓ ✕）        │ │  ▓▓▓▓▓▓░░░░  60%  │
│      │ └─────────────────────────────┘ │  开始合并（主按钮） │
│ v4.0 │                                 │  本地处理 · 文件不外传│
└──────┴─────────────────────────────────┴──────────────────┘
```

- 左侧 `GlassCard` 导航栏（宽 74）：`LogoBadge` + 合并 / 设置 / 关于三枚 `RailItem`（纯图标，悬停有 Tooltip 与底色），底部程序集版本号。
- 中间队列 `GlassCard`：`DocumentListBox`（行高 62）+ 四个 `IconButton`（上移 / 下移 / 删除 / 清空）+ 主按钮「添加文件」。
- 右侧输出 `GlassCard`（Strong，颜色更实）：文件名输入 + `.pdf` 后缀、保存位置 + 浏览、分隔线、状态、进度条、查看结果、本地处理说明、大号「开始合并」。

原有功能一个都没动：拖入、批量添加、拖动排序、右键置顶/置底、多选、4 种命名、3 种保存位置、进度与状态、完成提示、皮肤、6 种语言、水印。窗口最小尺寸由 `900×690` 调整为 `940×620`。

## 2. 磨砂玻璃怎么实现的

新增 `GlassEffects.cs`：

| 组成 | 说明 |
| --- | --- |
| `GlassPaint.BuildWallpaper` | 用皮肤里的两个渐变色 + 三团 `PathGradientBrush` 柔光画出"壁纸"（清晰层） |
| `GlassPaint.Frost` | 降采样到 1/9 再用双三次插值放大 → 高斯模糊近似，毫秒级 |
| `GlassPaint.Paint` | 把窗口位图按控件的屏幕坐标裁一块画上去，再叠加半透明 `tint` |
| `IGlassScene` | 窗体向子控件提供 `SharpBackdrop` / `FrostedBackdrop` 两张图 |
| `GlassCard` | 玻璃卡片：磨砂底 + 半透明填充 + 1px 描边 + 顶部高光（`LinearGradientBrush` 渐隐） |

窗口背景画**清晰**壁纸，卡片画同坐标的**模糊**版本 + 半透明色，于是卡片之间能看见清晰底纹、卡片内部是雾面，形成毛玻璃观感。

**为什么不用系统 Acrylic/Mica**：WinForms 的 GDI 绘制不支持逐像素透明，`SetWindowCompositionAttribute` 只能让整窗（包括文字）半透明，观感会发灰、且在 Win10 上不稳定。现在这套是自绘合成，任何 Windows 版本 / 任何皮肤都能一致工作，且可控。

**想要"透过窗口看见桌面模糊"**（真·亚克力）需要改成 `WS_EX_LAYERED` + `UpdateLayeredWindow` 的全自绘窗口，代价是不能再用子控件、要把整窗绘制接管——可以作为下一步单独做。

`SkinPalette` 新增的玻璃色：`WallpaperStart/End`、`WallpaperGlowA/B/C`、`GlassFill`、`GlassStrong`、`GlassBorder`、`GlassEdge`、`GlassTint`、`GlassHover`、`GlassInput`、`RailFill`，四套皮肤各自配好（经典蓝/网易红=浅色玻璃，夜间=深色玻璃，云白=暖白玻璃）。

## 3. 改动文件

| 文件 | 改动 |
| --- | --- |
| `GlassEffects.cs` | **新增**，磨砂玻璃渲染 + DWM 深色标题栏 / 圆角 |
| `AppSettings.cs` | `SkinPalette` 增加 13 个玻璃色字段，并为 4 套皮肤赋值 |
| `UiControls.cs` | `Theme` 暴露玻璃色；新增 `IconKind`/`IconPainter`、`GlassCard`、`RailItem`、`IconButton`、`LogoBadge`；`RoundedButton` 支持前置图标；`DocumentListBox` 改成行高 62 + 方形序号 + 悬停行内操作；`EmptyStatePanel`/`SlimProgressBar` 改为玻璃底 |
| `MainForm.cs` | 布局整体改为 B 方案；删除 `HeaderPanel` 顶栏；新增壁纸重建、列表底色采样、玻璃接口实现 |
| `SettingsForm.cs` | 改为玻璃窗口（自带壁纸），页面容器换成 `GlassCard`，标题带改为半透明玻璃，导航选中态改为半透明强调色 |

## 4. 编译与验证

```powershell
dotnet build DocHub.csproj -c Release
# 或
dotnet run --project DocHub.csproj
```

建议重点看：

1. 拖动窗口缩放时玻璃是否跟随（`OnResize` 有 140ms 防抖后重建壁纸）。
2. 切换皮肤（设置 → 外观）后，壁纸与卡片颜色是否即时更新。
3. 六种语言下右侧面板标签是否溢出（`FitWidth` 已按文字宽度计算「添加文件」和「浏览」）。
4. 列表行悬停时 ↑ ↓ ✕ 是否出现、点击是否生效。
5. 空列表时虚线投放区是否居中。

## 5. 已知风险（未编译验证的部分）

- 我无法在本机编译，若出现编译错误，最可能集中在：`GlassEffects.cs` 的 GDI+ 重载、`MainForm.cs` 里新字段与旧方法名的衔接、`UiControls.cs` 新增控件的属性名。
- `DocumentListBox` 的底色是"磨砂平均色采样"（`GetPixel` 每 20px 取一点），极端情况下换肤瞬间可能有一帧色差。
- 列表行内操作点击后会先选中该行再执行，`RemoveSelected()` 之后悬停索引会被重置，属于预期行为。

## 6. 回滚

本目录没有版本库，改动前建议先备份这几个文件：

```
MainForm.cs  UiControls.cs  SettingsForm.cs  AppSettings.cs
```

`GlassEffects.cs` 是新增文件，删掉并还原上面四个文件即可回到改造前。
