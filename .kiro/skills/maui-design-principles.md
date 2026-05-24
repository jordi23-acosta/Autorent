# .NET MAUI Design Principles & Best Practices

## 🎨 Core Design Principles

### 1. Platform-Specific Design
- Respect platform conventions (iOS vs Android)
- Use platform-specific resources when needed
- Implement adaptive layouts for different screen sizes

### 2. Performance-First UI
- Use CollectionView instead of ListView for large lists
- Implement virtualization for scrollable content
- Avoid nested StackLayouts (use Grid instead)
- Use compiled bindings for better performance

### 3. Responsive Design
- Design for multiple screen sizes and orientations
- Use relative sizing (Star, Auto) over absolute values
- Test on different device form factors

## 🚀 UI Best Practices

### Layout Optimization
```xml
<!-- ❌ AVOID: Nested StackLayouts -->
<StackLayout>
    <StackLayout>
        <StackLayout>
            <Label Text="Slow" />
        </StackLayout>
    </StackLayout>
</StackLayout>

<!-- ✅ PREFER: Grid with proper definitions -->
<Grid RowDefinitions="Auto,*,Auto">
    <Label Text="Fast" Grid.Row="0" />
    <ScrollView Grid.Row="1">
        <!-- Content -->
    </ScrollView>
    <Button Text="Action" Grid.Row="2" />
</Grid>
```

### Binding Performance
```csharp
// ❌ AVOID: Reflection-based binding (slow)
<Label Text="{Binding UserName}" />

// ✅ PREFER: Compiled bindings (fast)
<Label Text="{Binding UserName, Mode=OneWay}" 
       x:DataType="viewModels:ProfileViewModel" />
```

### Image Optimization
```xml
<!-- ✅ Use appropriate image sizes -->
<Image Source="photo.jpg" 
       Aspect="AspectFill"
       HeightRequest="200"
       WidthRequest="200" />

<!-- ✅ Cache images from web -->
<Image Source="{Binding PhotoUrl}"
       CachingEnabled="True"
       CacheValidity="7" />
```

## 🎯 Modern UI Patterns

### 1. Card-Based Layouts
Use cards for grouping related content:
```xml
<Frame CornerRadius="15" 
       HasShadow="True"
       Padding="15"
       Margin="10">
    <StackLayout>
        <Label Text="Title" FontSize="18" FontAttributes="Bold" />
        <Label Text="Description" FontSize="14" />
    </StackLayout>
</Frame>
```

### 2. Material Design Inputs
```xml
<Border StrokeThickness="1"
        Stroke="{AppThemeBinding Light=#E0E0E0, Dark=#424242}"
        StrokeShape="RoundRectangle 10"
        Padding="10">
    <Entry Placeholder="Email"
           PlaceholderColor="Gray"
           TextColor="{AppThemeBinding Light=Black, Dark=White}" />
</Border>
```

### 3. Loading States
Always show feedback during async operations:
```xml
<ActivityIndicator IsRunning="{Binding IsBusy}"
                   IsVisible="{Binding IsBusy}"
                   Color="#1565C0" />
```

## 🌈 Color & Theme Guidelines

### Dark Mode Support
Always provide both light and dark theme colors:
```xml
<Label TextColor="{AppThemeBinding Light=Black, Dark=White}" />
<Frame BackgroundColor="{AppThemeBinding Light=White, Dark=#2C2C2C}" />
```

### Color Palette
Define consistent colors in Resources:
```xml
<Color x:Key="Primary">#1565C0</Color>
<Color x:Key="Secondary">#FFA726</Color>
<Color x:Key="Success">#4CAF50</Color>
<Color x:Key="Error">#F44336</Color>
<Color x:Key="Warning">#FF9800</Color>
```

## 📱 Touch & Interaction

### Touch Targets
- Minimum touch target: 44x44 points (iOS) / 48x48 dp (Android)
- Add spacing between interactive elements
- Use TapGestureRecognizer for custom touch handling

### Visual Feedback
```xml
<Button Text="Click Me">
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="CommonStates">
            <VisualState x:Name="Normal">
                <VisualState.Setters>
                    <Setter Property="BackgroundColor" Value="#1565C0" />
                </VisualState.Setters>
            </VisualState>
            <VisualState x:Name="Pressed">
                <VisualState.Setters>
                    <Setter Property="BackgroundColor" Value="#0D47A1" />
                </VisualState.Setters>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
</Button>
```

## ⚡ Performance Tips

### 1. Use CollectionView
```xml
<!-- ✅ Better performance for lists -->
<CollectionView ItemsSource="{Binding Vehicles}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <!-- Item template -->
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

### 2. Enable XAML Compilation
```csharp
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}
```

### 3. Optimize Images
- Use appropriate image formats (WebP for photos, SVG for icons)
- Resize images to display size
- Enable image caching

## 🎭 Animation Guidelines

### Subtle Animations
```csharp
// Fade in animation
await myView.FadeTo(1, 250, Easing.CubicIn);

// Scale animation
await myButton.ScaleTo(1.1, 100);
await myButton.ScaleTo(1, 100);

// Slide animation
await myCard.TranslateTo(0, 0, 300, Easing.SpringOut);
```

### Animation Best Practices
- Keep animations under 300ms
- Use easing functions for natural motion
- Don't animate during data loading
- Provide option to disable animations (accessibility)

## ♿ Accessibility

### Semantic Properties
```xml
<Label Text="Welcome"
       SemanticProperties.HeadingLevel="Level1"
       SemanticProperties.Description="Welcome message" />

<Button Text="Submit"
        SemanticProperties.Hint="Submits the form" />
```

### Screen Reader Support
- Provide meaningful descriptions
- Set proper heading levels
- Test with TalkBack (Android) and VoiceOver (iOS)

## 📐 Spacing & Typography

### Consistent Spacing
```xml
<!-- Use multiples of 4 or 8 -->
<StackLayout Spacing="16">
    <Label Margin="0,0,0,8" />
    <Entry Margin="0,0,0,16" />
</StackLayout>
```

### Typography Scale
```xml
<Label FontSize="32" FontAttributes="Bold" /> <!-- H1 -->
<Label FontSize="24" FontAttributes="Bold" /> <!-- H2 -->
<Label FontSize="18" FontAttributes="Bold" /> <!-- H3 -->
<Label FontSize="16" /> <!-- Body -->
<Label FontSize="14" /> <!-- Caption -->
<Label FontSize="12" /> <!-- Small -->
```

## 🔧 Common Patterns

### Empty States
```xml
<StackLayout IsVisible="{Binding HasNoData}"
             VerticalOptions="Center"
             HorizontalOptions="Center">
    <Label Text="📭" FontSize="64" HorizontalOptions="Center" />
    <Label Text="No items yet" FontSize="18" HorizontalOptions="Center" />
    <Label Text="Add your first item to get started" 
           FontSize="14" 
           TextColor="Gray"
           HorizontalOptions="Center" />
</StackLayout>
```

### Error States
```xml
<Frame BackgroundColor="#FFEBEE" 
       BorderColor="#F44336"
       IsVisible="{Binding HasError}">
    <StackLayout Orientation="Horizontal">
        <Label Text="⚠️" FontSize="20" />
        <Label Text="{Binding ErrorMessage}" 
               TextColor="#C62828"
               VerticalOptions="Center" />
    </StackLayout>
</Frame>
```

## 📚 Resources

- [Microsoft MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Material Design Guidelines](https://m3.material.io/)
- [iOS Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)
- [Android Design Guidelines](https://developer.android.com/design)

---

**Remember**: Good design is invisible. Focus on usability, performance, and accessibility first, then add visual polish.
