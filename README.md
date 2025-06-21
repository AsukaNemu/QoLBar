# QoL Bar
A [XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) plugin for creating custom menus for commands, similar to hotbars and macros.

一款基于FF14 Dalamua的插件，用于创建命令的自定义菜单，类似于快捷栏和宏。适用于由Dalamua XIVLauncher.

## Getting Started 使用方式
Upon first installing through `/xlplugins`, you will be greeted by a blank bar at the bottom of your screen, and the Demo Bar, which will showcase the various features that can be utilized. This bar will be updated over time to showcase any new features and may be freely deleted at any point, as it can be recreated by right clicking the `Import` button on the plugin config.

通过 `/xlplugins` 首次安装后，你将在屏幕底部看到一个空白条形框和演示条形框，后者将展示可利用的各种功能。该条形框将随时间更新以展示新功能，并且可以随时自由删除，因为它可以通过在插件配置中右键点击 `Import` 按钮重新创建。

- Terminology
  - Bar: These are the custom menus that contain shortcuts, and are similar to hotbars
  - Shortcut: These are the buttons on a bar, and are similar to macros
  - Category: This is a shortcut that creates a popup containing more shortcuts
  - Spacer: This is a shortcut that has no function and exists specifically for information or to align other shortcuts
  - Pie: A bar brought up as a radial menu by holding a hotkey

- 术语
  - 按键(Bar)：这些是包含快捷方式的定制菜单，类似于热键栏
  - 快捷方式(Shortcut)：这些是条上的按钮，类似于宏
  - 子菜单(Category)：这是一个创建包含更多快捷方式的弹出窗口的快捷方式
  - 间隔符(Spacer)：这是一个没有功能的快捷方式，专门用于信息显示或对齐其他快捷方式,无法点击.
  - 饼图(Pie)：通过按住热键弹出的环形菜单形式的条形图(类似T-Pie不知你是否用过 反正我没用过=w=)


- Navigation
  - Left click: Executes shortcuts or moves unlocked bars
  - Right click: Opens the configuration for the hovered shortcut or bar (the surrounding background)
  - Shift + Right click: Adds a new shortcut to the hovered bar or category
  - Double or Control + Left Click: Can be used on various options such as sliders to input custom amounts

- 导航
  - 左键点击：执行快捷方式或移动解锁的窗口
  - 右键点击：打开悬停的快捷方式或条形（周围的背景）
  - Shift + 右键点击：向悬停的条形或类别添加新的快捷方式
  - 双击或按住 Control 键单击：可用于滑块等各种选项以输入自定义数量

You can edit the shortcuts on the Demo Bar in order to see how they function.

您可以在演示栏中编辑快捷方式，以查看它们的功能。(译者仍未找到演示栏指的是哪个)

## Advanced Features
All of the following information is available through tooltips on the corresponding buttons or configuration inputs or in an example on the Demo Bar, but will be explained here in more detail.

## 高级功能
所有以下信息均可通过相应按钮或配置输入的提示框，或在演示条上的示例获取，但此处将更详细地解释。

### Tooltips
To add a tooltip to a shortcut, put `##` at the end of its name to use the following text as a tooltip, e.g. `Plugins##Opens the plugin installer`.

### 提示框
要在快捷方式中添加提示框，在其名称末尾加上 `##` 以使用以下文本作为提示框，例如 `Plugins##打开插件安装器`。

### Icons
To use an icon from the game, use `::#` as the shortcut's name, where # is the icon's ID, e.g. `::1`. These can also utilize tooltips in the same manner as above.

### 图标
要使用游戏中的图标，将 `::#` 作为快捷方式名称，其中 # 是图标的 ID，例如 `::1`。这些也可以像上面一样使用工具提示。

### Icon Arguments
Icons can have modifiers (`::x#`) which will change the way they display, these are detailed in the `Name` input tooltip when editing a shortcut with an icon. The most common use case is adding the glossy frame that the hotbar applies to all icons, i.e. `::f2914`.

### Icon Browser
This plugin comes with a built-in browser for finding various icons, it can be accessed through the magnifying glass in the corner when editing / adding shortcuts, or through `/qolicons`. Clicking on these icons will copy their ID in the format `::#`, so that you can paste them into the name of shortcuts. Additionally, opening a shortcut's configuration while this menu is open will automatically change it to use the last clicked icon.

### Custom Icons
To use a custom icon, follow the instructions on the Icon Browser's `Custom` tab.

### Shortcut Mode
The mode of a shortcut can be changed so that the command is executed differently. `Incremental` will change it so that the shortcut executes the first line on the first press, the second line on the second press, and so on. `Random` will cause the shortcut to execute a random line each time instead. Categories may also have their mode changed and will no longer open a popup upon being pressed. Instead they will act like a shortcut using a different mode, except using the shortcuts contained within instead of lines.

### Bar Dock / Alignment
The dock and alignment of a bar will determine the side of the game window that the bar will position itself relative to. If you want to place a bar outside of the game, you can do so by using the `Undocked` option. This will let you drag the bar out of the game window (assuming the Dalamud option is enabled) and will change the position to be relative to the top left corner of your monitor.

### Pies
By adding a hotkey to a bar, you can bring it up as a pie. Note that a bar can be hidden through `/qolvisible` and its hotkey will still work.

### Condition Sets
By adding a condition set to a bar through the plugin config's `Bar Manager` tab, it will automatically hide or reveal depending on the current game state. These can be created through the `Condition Sets` tab.

### Running Macros
To run a macro from a shortcut, use `//m#` in the command, where # is the slot of the macro, e.g. `//m99`. In order to use the shared tab of macros you need to add 100 to the slot, e.g. `//m100`.

### Custom Macros
`//m` also supports not specifying a macro slot in order to run the following commands through its own macro system.

E.g.
```
//m
/macrolock
/wait 1
/echo Macro finished
//m
```
will function identically to having a macro with those commands in individual slot #0 and just using `//m0`.

### Comments
Shortcuts support comments if you would like to add notes inside of the command, i.e. `// This is a comment`.

### Exporting / Importing
You can share a bar or shortcut by pressing the `Export` button when editing it, this will copy a block of text that you can send to another person. In order to use this block of text, simply copy it and press the `Import` button on the plugin config's `Bar Manager` tab to import the text as a bar. You may also use the `Import` button when adding a new shortcut to import all shortcuts inside of the text into a preexisting bar or category.

## FAQ
> I'm completely lost and don't understand how to make my own bars or shortcuts

Be sure to play around with the Demo Bar for a bit before trying to create your own bar. Additionally, create the bar without trying to utilize advanced features, i.e. just edit the `Name` and `Command` of shortcuts. After the bar contains all of the basic functions you want it to have, then you can start changing the way it looks, or adding advanced features. If you absolutely cannot solve how to do something, feel free to ask in the Dalamud Discord for assistance.


> /wait doesn't appear to work!

Shortcuts do not have the ability to wait or use any other macro specific command. You will need to run a macro (or use the custom macro feature) with the shortcut instead.


> Where can I find bars / shortcuts that other people have made?

The Dalamud Discord has a plugin preset channel that contains various presets that people have created.


> Where is my config located?

`%AppData%\XIVLauncher\pluginConfigs\QoLBar.json`


> How can I suggest a feature?

Ping me on the Dalamud Discord or [create an issue](https://github.com/UnknownX7/QoLBar/issues/new).


> X isn't working!

See above.
