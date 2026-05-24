# .NET MAUI Layouts & Responsive Design

## 📐 Layout Types

### 1. Grid (Most Versatile)

#### Basic Grid
```xml
<Grid RowDefinitions="Auto,*,Auto"
      ColumnDefinitions="*,2*"
      RowSpacing="10"
      ColumnSpacing="10"
      Padding="15">
    
    <!-- Header spans both columns -->
    <Label Text="Header" 
           Grid.Row="0" 
           Grid.ColumnSpan="2" />
    
    <!-- Sidebar -->
    <StackLayout Grid.Row="1" Grid.Column="0">
        <!-- Sidebar content -->
    </StackLayout>
    
    <!-- Main content -->
    <ScrollView Grid.Row="1" Grid.Column="1">
        <!-- Main content -->
    </ScrollView>
    
    <!-- Footer spans both columns -->
    <Button Text="Action" 
            Grid.Row="2" 
            Grid.ColumnSpan="2" />
</Grid>
```

#### Responsive Grid
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="60" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="2*" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
</Grid>
```

### 2. StackLayout (Simple Stacking)

#### Vertical Stack
```xml
<StackLayout Spacing="15" Padding="20">
    <Label Text="Title" FontSize="24" />
    <Entry Placeholder="Input" />
    <Button Text="Submit" />
</StackLayout>
```

#### Horizontal Stack
```xml
<StackLayout Orientation="Horizontal" Spacing="10">
    <Image Source="icon.png" WidthRequest="40" />
    <Label Text="Label" VerticalOptions="Center" />
</StackLayout>
```

### 3. FlexLayout (Flexible Wrapping)

#### Wrap Layout
```xml
<FlexLayout Wrap="Wrap" 
            JustifyContent="SpaceAround"
            AlignItems="Center"
            Padding="10">
    <Frame WidthRequest="100" HeightRequest="100" Margin="5" />
    <Frame WidthRequest="100" HeightRequest="100" Margin="5" />
    <Frame WidthRequest="100" HeightRequest="100" Margin="5" />
    <Frame WidthRequest="100" HeightRequest="100" Margin="5" />
</FlexLayout>
```

#### Responsive FlexLayout
```xml
<FlexLayout Direction="Row"
            Wrap="Wrap"
            JustifyContent="Start"
            AlignContent="Start">
    <Frame FlexLayout.Basis="45%" 
           FlexLayout.Grow="1"
           Margin="5">
        <!-- Card content -->
    </Frame>
</FlexLayout>
```

### 4. AbsoluteLayout (Precise Positioning)

```xml
<AbsoluteLayout>
    <!-- Background image -->
    <Image Source="background.jpg"
           AbsoluteLayout.LayoutBounds="0,0,1,1"
           AbsoluteLayout.LayoutFlags="All"
           Aspect="AspectFill" />
    
    <!-- Floating button -->
    <Button Text="+"
            AbsoluteLayout.LayoutBounds="0.95,0.95,60,60"
            AbsoluteLayout.LayoutFlags="PositionProportional"
            CornerRadius="30" />
</AbsoluteLayout>
```

## 📱 Responsive Design Patterns

### 1. Adaptive Layouts with OnIdiom

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="{OnIdiom Phone=Auto, Tablet=200, Desktop=300}" />
    </Grid.RowDefinitions>
    
    <Label FontSize="{OnIdiom Phone=14, Tablet=18, Desktop=22}" />
</Grid>
```

### 2. Platform-Specific Layouts

```xml
<ContentPage>
    <OnPlatform x:TypeArguments="View">
        <On Platform="iOS">
            <StackLayout Padding="0,20,0,0">
                <!-- iOS specific layout -->
            </StackLayout>
        </On>
        <On Platform="Android">
            <StackLayout>
                <!-- Android specific layout -->
            </StackLayout>
        </On>
    </OnPlatform>
</ContentPage>
```

### 3. Orientation-Aware Layouts

```csharp
// In code-behind or ViewModel
protected override void OnSizeAllocated(double width, double height)
{
    base.OnSizeAllocated(width, height);
    
    if (width > height)
    {
        // Landscape
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    }
    else
    {
        // Portrait
        MainGrid.ColumnDefinitions.Clear();
        MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    }
}
```

### 4. Breakpoint-Based Layouts

```csharp
public class ResponsiveLayout : Grid
{
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        
        if (width < 600) // Mobile
        {
            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
        else if (width < 1200) // Tablet
        {
            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
        else // Desktop
        {
            ColumnDefinitions.Clear();
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
    }
}
```

## 🎨 Common Layout Patterns

### 1. Card Grid Layout

```xml
<ScrollView>
    <FlexLayout Wrap="Wrap" 
                JustifyContent="SpaceAround"
                Padding="10">
        <Frame FlexLayout.Basis="45%"
               FlexLayout.Grow="1"
               Margin="5"
               CornerRadius="15"
               HasShadow="True">
            <StackLayout>
                <Image Source="image1.jpg" Aspect="AspectFill" HeightRequest="150" />
                <Label Text="Card Title" FontSize="16" FontAttributes="Bold" Margin="10,10,10,0" />
                <Label Text="Description" FontSize="14" TextColor="Gray" Margin="10,5,10,10" />
            </StackLayout>
        </Frame>
        <!-- More cards -->
    </FlexLayout>
</ScrollView>
```

### 2. Master-Detail Layout

```xml
<Grid ColumnDefinitions="300,*">
    <!-- Master (List) -->
    <CollectionView ItemsSource="{Binding Items}"
                    Grid.Column="0">
        <!-- Item template -->
    </CollectionView>
    
    <!-- Detail -->
    <ScrollView Grid.Column="1" Padding="20">
        <StackLayout>
            <Label Text="{Binding SelectedItem.Title}" FontSize="24" />
            <Label Text="{Binding SelectedItem.Description}" />
        </StackLayout>
    </ScrollView>
</Grid>
```

### 3. Form Layout

```xml
<ScrollView>
    <StackLayout Padding="20" Spacing="15">
        <!-- Section Header -->
        <Label Text="Personal Information" 
               FontSize="20" 
               FontAttributes="Bold"
               Margin="0,10,0,5" />
        
        <!-- Input Group -->
        <Grid RowDefinitions="Auto,Auto" RowSpacing="5">
            <Label Text="Full Name" FontSize="12" TextColor="Gray" />
            <Border StrokeThickness="1"
                    Stroke="#E0E0E0"
                    StrokeShape="RoundRectangle 10"
                    Grid.Row="1">
                <Entry Text="{Binding FullName}" Placeholder="Enter your name" />
            </Border>
        </Grid>
        
        <!-- Two Column Layout -->
        <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
            <Grid RowDefinitions="Auto,Auto" RowSpacing="5" Grid.Column="0">
                <Label Text="Age" FontSize="12" TextColor="Gray" />
                <Border StrokeThickness="1" Stroke="#E0E0E0" StrokeShape="RoundRectangle 10" Grid.Row="1">
                    <Entry Text="{Binding Age}" Keyboard="Numeric" />
                </Border>
            </Grid>
            
            <Grid RowDefinitions="Auto,Auto" RowSpacing="5" Grid.Column="1">
                <Label Text="Gender" FontSize="12" TextColor="Gray" />
                <Border StrokeThickness="1" Stroke="#E0E0E0" StrokeShape="RoundRectangle 10" Grid.Row="1">
                    <Picker SelectedItem="{Binding Gender}">
                        <Picker.Items>
                            <x:String>Male</x:String>
                            <x:String>Female</x:String>
                            <x:String>Other</x:String>
                        </Picker.Items>
                    </Picker>
                </Border>
            </Grid>
        </Grid>
        
        <!-- Submit Button -->
        <Button Text="Submit" 
                BackgroundColor="#1565C0"
                TextColor="White"
                CornerRadius="25"
                HeightRequest="50"
                Margin="0,20,0,0" />
    </StackLayout>
</ScrollView>
```

### 4. Dashboard Layout

```xml
<ScrollView>
    <Grid RowDefinitions="Auto,Auto,*" 
          RowSpacing="15"
          Padding="15">
        
        <!-- Header -->
        <StackLayout Grid.Row="0">
            <Label Text="Dashboard" FontSize="28" FontAttributes="Bold" />
            <Label Text="Welcome back!" FontSize="14" TextColor="Gray" />
        </StackLayout>
        
        <!-- Stats Cards -->
        <Grid Grid.Row="1" 
              ColumnDefinitions="*,*" 
              RowDefinitions="Auto,Auto"
              ColumnSpacing="10"
              RowSpacing="10">
            
            <Frame Grid.Row="0" Grid.Column="0" CornerRadius="15" HasShadow="True">
                <StackLayout>
                    <Label Text="🚗" FontSize="32" HorizontalOptions="Center" />
                    <Label Text="24" FontSize="28" FontAttributes="Bold" HorizontalOptions="Center" />
                    <Label Text="Active Vehicles" FontSize="12" TextColor="Gray" HorizontalOptions="Center" />
                </StackLayout>
            </Frame>
            
            <Frame Grid.Row="0" Grid.Column="1" CornerRadius="15" HasShadow="True">
                <StackLayout>
                    <Label Text="💰" FontSize="32" HorizontalOptions="Center" />
                    <Label Text="$1,234" FontSize="28" FontAttributes="Bold" HorizontalOptions="Center" />
                    <Label Text="Total Earnings" FontSize="12" TextColor="Gray" HorizontalOptions="Center" />
                </StackLayout>
            </Frame>
            
            <Frame Grid.Row="1" Grid.Column="0" CornerRadius="15" HasShadow="True">
                <StackLayout>
                    <Label Text="📊" FontSize="32" HorizontalOptions="Center" />
                    <Label Text="12" FontSize="28" FontAttributes="Bold" HorizontalOptions="Center" />
                    <Label Text="Active Rentals" FontSize="12" TextColor="Gray" HorizontalOptions="Center" />
                </StackLayout>
            </Frame>
            
            <Frame Grid.Row="1" Grid.Column="1" CornerRadius="15" HasShadow="True">
                <StackLayout>
                    <Label Text="⭐" FontSize="32" HorizontalOptions="Center" />
                    <Label Text="4.8" FontSize="28" FontAttributes="Bold" HorizontalOptions="Center" />
                    <Label Text="Rating" FontSize="12" TextColor="Gray" HorizontalOptions="Center" />
                </StackLayout>
            </Frame>
        </Grid>
        
        <!-- Recent Activity -->
        <StackLayout Grid.Row="2" Spacing="10">
            <Label Text="Recent Activity" FontSize="18" FontAttributes="Bold" Margin="0,10,0,0" />
            <CollectionView ItemsSource="{Binding RecentActivity}">
                <!-- Activity items -->
            </CollectionView>
        </StackLayout>
    </Grid>
</ScrollView>
```

### 5. List with Header and Footer

```xml
<Grid RowDefinitions="Auto,*,Auto">
    <!-- Header -->
    <Frame Grid.Row="0" 
           CornerRadius="0,0,20,20"
           BackgroundColor="#1565C0"
           Padding="20">
        <StackLayout>
            <Label Text="My Vehicles" 
                   FontSize="24" 
                   FontAttributes="Bold"
                   TextColor="White" />
            <SearchBar Placeholder="Search..."
                       BackgroundColor="White"
                       Margin="0,10,0,0" />
        </StackLayout>
    </Frame>
    
    <!-- List -->
    <CollectionView ItemsSource="{Binding Vehicles}"
                    Grid.Row="1"
                    Margin="10">
        <!-- Item template -->
    </CollectionView>
    
    <!-- Footer / FAB -->
    <Button Text="+"
            Grid.Row="2"
            BackgroundColor="#1565C0"
            TextColor="White"
            CornerRadius="28"
            WidthRequest="56"
            HeightRequest="56"
            HorizontalOptions="End"
            Margin="20"
            Command="{Binding AddVehicleCommand}" />
</Grid>
```

## 🎯 Layout Performance Tips

### 1. Avoid Deep Nesting
```xml
<!-- ❌ BAD: Too many nested layouts -->
<StackLayout>
    <StackLayout>
        <StackLayout>
            <StackLayout>
                <Label Text="Deep" />
            </StackLayout>
        </StackLayout>
    </StackLayout>
</StackLayout>

<!-- ✅ GOOD: Flat structure with Grid -->
<Grid RowDefinitions="Auto,Auto,Auto,Auto">
    <Label Text="Flat" Grid.Row="0" />
    <Label Text="Structure" Grid.Row="1" />
    <Label Text="Is" Grid.Row="2" />
    <Label Text="Better" Grid.Row="3" />
</Grid>
```

### 2. Use Appropriate Layout
- **Grid**: Complex layouts with rows and columns
- **StackLayout**: Simple linear stacking
- **FlexLayout**: Wrapping and flexible sizing
- **AbsoluteLayout**: Overlapping or precise positioning

### 3. Virtualization
Always use CollectionView for lists (it virtualizes automatically)

### 4. Lazy Loading
```xml
<CarouselView ItemsSource="{Binding Images}"
              Loop="False"
              PeekAreaInsets="20">
    <CarouselView.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding Url}"
                   Aspect="AspectFill"
                   CachingEnabled="True" />
        </DataTemplate>
    </CarouselView.ItemTemplate>
</CarouselView>
```

## 📏 Spacing Guidelines

### Consistent Spacing System
```
4px  - Tiny (between related elements)
8px  - Small (within components)
12px - Medium (between components)
16px - Large (section spacing)
24px - XLarge (major sections)
32px - XXLarge (page margins)
```

### Example Usage
```xml
<StackLayout Padding="16" Spacing="12">
    <Label Text="Title" Margin="0,0,0,8" />
    <Entry Margin="0,0,0,16" />
    <Button Margin="0,24,0,0" />
</StackLayout>
```

---

**Pro Tip**: Test your layouts on different screen sizes and orientations. Use the device toolbar in Visual Studio to preview multiple devices.
