

Uses very simple CSS-style theme file to alter class properties. Property values must match parameters for a constructor of the property type. Colors come from the System.Drawing.KnownColors enum.

Install using [nuget](//www.nuget.org)

```
Install-Package Paradoxlost.UX.WinForms 
```

Example:

```css
@vals {
    myFont: Segoe UI, 9.0, Regular;
}

@modules {
	// Default is always System.Windows.Forms
	MyModule: some.assembly;
}

Form {
    Font: $myFont;
    BackColor: Blue;
}

MyCustomForm {
	$module: MyModule;
	BackColor: Green;
}
```
