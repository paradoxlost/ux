

Uses very simple CSS-style theme file to alter class properties. Property values must match parameters for a constructor of the property type. Colors come from the System.Drawing.KnownColors enum.

Example:

```css
@var {
    myFont: Segoe UI, 9.0, Regular;
}

@module {
    Assembly: System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL;
}

Form {
    Font: $myFont;
    BackColor: Blue;
}
```
