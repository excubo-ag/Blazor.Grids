## Excubo.Blazor.Grids

[![Nuget](https://img.shields.io/nuget/v/Excubo.Blazor.Grids)](https://www.nuget.org/packages/Excubo.Blazor.Grids/)
[![Nuget](https://img.shields.io/nuget/dt/Excubo.Blazor.Grids)](https://www.nuget.org/packages/Excubo.Blazor.Grids/)
[![GitHub](https://img.shields.io/github/license/excubo-ag/Blazor.Grids)](https://github.com/excubo-ag/Blazor.Grids)

Excubo.Blazor.Grids is a native-Blazor grid and dashboard component library.

[Demo on github.io using Blazor Webassembly](https://excubo-ag.github.io/Blazor.Grids/)

## Key features

- Convenient usage of a css grid
- Aspect ratio
- Movable elements
- Resizable elements
- Easy way to add rows and columns, either individually or in bulk
- Events on move or resize of elements

## How to use

### 1. Install the nuget package Excubo.Blazor.Grids

Excubo.Blazor.Grids is distributed [via nuget.org](https://www.nuget.org/packages/Excubo.Blazor.Grids/).
[![Nuget](https://img.shields.io/nuget/v/Excubo.Blazor.Grids)](https://www.nuget.org/packages/Excubo.Blazor.Grids/)

#### Package Manager:
```ps
Install-Package Excubo.Blazor.Grids
```

#### .NET Cli:
```cmd
dotnet add package Excubo.Blazor.Grids
```

#### Package Reference
```xml
<PackageReference Include="Excubo.Blazor.Grids" />
```

### 2. Add the `Grid` (or a `Dashboard`) component to your app

```html
@using Excubo.Blazor.Grids

<Grid RowGap="1em" ColumnGap="1em">
    <RowDefinition Height="1fr" />
    <RowDefinition Height="1fr" />
    <ColumnDefinition Width="auto" />
    <ColumnDefinition Width="auto" />
    <Element Column="0" Row="0">
        <div style="background-color: purple; min-width: 2em; min-height: 2em"></div>
    </Element>
    <Element Column="0" Row="1">
        <div style="background-color: pink; min-width: 2em; min-height: 2em"></div>
    </Element>
    <Element Column="1" Row="0">
        <div style="background-color: gray; min-width: 2em; min-height: 2em"></div>
    </Element>
    <Element Column="1" Row="1">
        <div style="background-color: green; min-width: 2em; min-height: 2em"></div>
    </Element>
</Grid>
```

or create a dashboard:

```html
@using Excubo.Blazor.Grids
<Dashboard AspectRatio="1.5" ColumnCount="6">
    <Element Column="0" Row="0" Title="A heading">
        I'm in a dashboard, therefore movable and resizable.
    </Element>
</Dashboard>
```

Have a look at the fully working examples provided in [the sample project](https://github.com/excubo-ag/Blazor.Grids/tree/main/TestProject_Components).

## Design principles

- Blazor API

The API should feel like you're using Blazor, not a javascript library.

- Minimal js, minimal css, lazy-loaded only when you use the component

The non-C# part of the code of the library should be as tiny as possible. We set ourselves a maximum amount of 10kB for combined js+css.
The current payload is less than 1kB, and only gets loaded dynamically when the component is actually used.

## Breaking changes

### Version 3.X.Y

Targets net6.0 exclusively from now on.

### Version 2.X.Y

Events were changed such that the callback parameter is not an `Element` anymore, but `ElementMoveArgs` or `ElementResizeArgs`. To upgrade your code, apply the changes like this:

```diff
-    private void OnMove(Element element)
-    {
-        GridEvents.Add(("moved", element.Title, element.Row, element.Column));
-    }
+    private void OnMove(ElementMoveArgs args)
+    {
+        GridEvents.Add(("moved", args.Element.Title, args.NewRow, args.NewColumn));
+    }
-    private void OnResize(Element element)
-    {
-        GridEvents.Add(("moved", element.Title, element.Row, element.Column));
-    }
+    private void OnResize(ElementMoveArgs args)
+    {
+        GridEvents.Add(("moved", args.Element.Title, args.NewRow, args.NewColumn));
+    }
```
