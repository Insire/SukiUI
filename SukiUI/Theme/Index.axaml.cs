using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using SukiUI.Enums;
using SukiUI.Models;

namespace SukiUI;

public partial class SukiTheme : Styles
{
    public static readonly StyledProperty<SukiColor> ThemeColorProperty =
        AvaloniaProperty.Register<SukiTheme, SukiColor>(nameof(Color), defaultBindingMode: BindingMode.TwoWay,
            defaultValue: SukiColor.Blue);

    public SukiColor ThemeColor
    {
        get => GetValue(ThemeColorProperty);
        set
        {
            SetValue(ThemeColorProperty, value);
            SetColorThemeResources();
        }
    }
    
    /// <summary>
    /// Called whenever the application's <see cref="SukiColor"/> is changed.
    /// Useful where controls cannot use "DynamicResource"
    /// </summary>
    public static Action<SukiColorTheme>? OnColorThemeChanged { get; set; }
    
    public static Action<ThemeVariant>? OnBaseThemeChanged { get; set; }

    /// <summary>
    /// Currently active <see cref="SukiColorTheme"/>
    /// </summary>
    public static SukiColorTheme ActiveColorTheme { get; private set; }
    
    /// <summary>
    /// All available Color Themes.
    /// </summary>
    public static readonly IReadOnlyList<SukiColorTheme> ColorThemes;
    
    internal static readonly IReadOnlyDictionary<SukiColor, SukiColorTheme> ColorThemeMap;
    
    private static SukiTheme? _instance;
    
    static SukiTheme()
    {
        ColorThemes = new[]
        {
            new SukiColorTheme(SukiColor.Orange, Color.Parse("#ED8E12"), Color.Parse("#15176CE8")),
            new SukiColorTheme(SukiColor.Red, Color.Parse("#D03A2F"), Color.Parse("#152FC5D0")),
            new SukiColorTheme(SukiColor.Green, Colors.ForestGreen, Color.Parse("#15B24DB0")),
            new SukiColorTheme(SukiColor.Blue, Color.Parse("#0A59F7"), Color.Parse("#15F7A80A"))
        };
        ColorThemeMap = ColorThemes.ToDictionary(x => x.Theme);
    }
    
    public void SetColorThemeResources()
    {
        _instance ??= this;
        if (Application.Current is null) return;
        if (!ColorThemeMap.TryGetValue(ThemeColor, out var colorTheme))
            throw new Exception($"{ThemeColor} has no defined color theme.");
        Application.Current.Resources["SukiPrimaryColor"] = colorTheme.Primary;
        Application.Current.Resources["SukiAccentColor"] = colorTheme.Accent;
        ActiveColorTheme = colorTheme;
    }
    
    /// <summary>
    /// Attempts to change the theme to the given value.
    /// </summary>
    /// <param name="sukiColor">The <see cref="SukiColor"/> to change to.</param>
    public static void TryChangeColorTheme(SukiColor sukiColor)
    {
        if (_instance is null) return;
        _instance.ThemeColor = sukiColor;
        OnColorThemeChanged?.Invoke(ActiveColorTheme);
    }

    /// <summary>
    /// <inheritdoc cref="TryChangeColorTheme(SukiUI.Enums.SukiColor)"/>
    /// </summary>
    /// <param name="sukiColorTheme"></param>
    public static void TryChangeColorTheme(SukiColorTheme sukiColorTheme) => 
        TryChangeColorTheme(sukiColorTheme.Theme);

    /// <summary>
    /// Tries to change the base theme to the one provided, if it is different.
    /// </summary>
    /// <param name="baseTheme"></param>
    public static void TryChangeBaseTheme(ThemeVariant baseTheme)
    {
        if (Application.Current is null) return;
        if (Application.Current.ActualThemeVariant == baseTheme) return;
        Application.Current!.RequestedThemeVariant = baseTheme;
        OnBaseThemeChanged?.Invoke(baseTheme);
    }

    /// <summary>
    /// Simply switches from Light -> Dark and visa versa.
    /// </summary>
    public static void SwitchBaseTheme()
    {
        if (Application.Current is null) return;
        var newBase = Application.Current.ActualThemeVariant == ThemeVariant.Dark 
            ? ThemeVariant.Light 
            : ThemeVariant.Dark;
        Application.Current.RequestedThemeVariant = newBase;
        OnBaseThemeChanged?.Invoke(newBase);
    }
}