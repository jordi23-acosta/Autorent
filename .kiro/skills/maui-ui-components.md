# .NET MAUI UI Components Guide

## 🧩 Essential Components

### 1. Buttons

#### Standard Button
```xml
<Button Text="Primary Action"
        BackgroundColor="#1565C0"
        TextColor="White"
        CornerRadius="25"
        HeightRequest="50"
        FontSize="16"
        FontAttributes="Bold"
        Command="{Binding SubmitCommand}" />
```

#### Outlined Button
```xml
<Border StrokeThickness="2"
        Stroke="#1565C0"
        StrokeShape="RoundRectangle 25"
        BackgroundColor="Transparent">
    <Button Text="Secondary Action"
            BackgroundColor="Transparent"
            TextColor="#1565C0"
            Command="{Binding CancelCommand}" />
</Border>
```

#### Icon Button
```xml
<ImageButton Source="icon_add.png"
             BackgroundColor="#1565C0"
             CornerRadius="28"
             HeightRequest="56"
             WidthRequest="56"
             Padding="16"
             Command="{Binding AddCommand}" />
```

### 2. Input Fields

#### Material Design Entry
```xml
<Border StrokeThickness="1"
        Stroke="{AppThemeBinding Light=#E0E0E0, Dark=#424242}"
        StrokeShape="RoundRectangle 10"
        Padding="15,10">
    <Entry Placeholder="Enter email"
           PlaceholderColor="Gray"
           TextColor="{AppThemeBinding Light=Black, Dark=White}"
           Keyboard="Email"
           Text="{Binding Email}" />
</Border>
```

#### Password Entry with Toggle
```xml
<Grid ColumnDefinitions="*,Auto">
    <Border StrokeThickness="1"
            Stroke="#E0E0E0"
            StrokeShape="RoundRectangle 10"
            Padding="15,10"
            Grid.ColumnSpan="2">
        <Entry Placeholder="Password"
               IsPassword="{Binding IsPasswordHidden}"
               Text="{Binding Password}" />
    </Border>
    <ImageButton Source="icon_eye.png"
                 Grid.Column="1"
                 BackgroundColor="Transparent"
                 Padding="10"
                 Command="{Binding TogglePasswordCommand}" />
</Grid>
```

#### Search Bar
```xml
<SearchBar Placeholder="Search vehicles..."
           Text="{Binding SearchQuery}"
           SearchCommand="{Binding SearchCommand}"
           BackgroundColor="{AppThemeBinding Light=#F5F5F5, Dark=#2C2C2C}"
           TextColor="{AppThemeBinding Light=Black, Dark=White}"
           PlaceholderColor="Gray" />
```

### 3. Cards

#### Basic Card
```xml
<Frame CornerRadius="15"
       HasShadow="True"
       Padding="15"
       Margin="10,5"
       BackgroundColor="{AppThemeBinding Light=White, Dark=#2C2C2C}">
    <Grid RowDefinitions="Auto,Auto,Auto" RowSpacing="10">
        <Label Text="Card Title" 
               FontSize="18" 
               FontAttributes="Bold"
               Grid.Row="0" />
        <Label Text="Card description goes here" 
               FontSize="14"
               TextColor="Gray"
               Grid.Row="1" />
        <Button Text="Action" 
                BackgroundColor="#1565C0"
                Grid.Row="2" />
    </Grid>
</Frame>
```

#### Image Card
```xml
<Frame CornerRadius="15" 
       HasShadow="True" 
       Padding="0"
       Margin="10">
    <Grid RowDefinitions="200,Auto">
        <Image Source="{Binding ImageUrl}"
               Aspect="AspectFill"
               Grid.Row="0" />
        <StackLayout Padding="15" Grid.Row="1">
            <Label Text="{Binding Title}" 
                   FontSize="18" 
                   FontAttributes="Bold" />
            <Label Text="{Binding Description}" 
                   FontSize="14"
                   TextColor="Gray" />
        </StackLayout>
    </Grid>
</Frame>
```

### 4. Lists

#### CollectionView with Cards
```xml
<CollectionView ItemsSource="{Binding Items}"
                SelectionMode="Single"
                SelectedItem="{Binding SelectedItem}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame CornerRadius="15" 
                   HasShadow="True"
                   Margin="10,5"
                   Padding="15">
                <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="15">
                    <Image Source="{Binding Icon}"
                           WidthRequest="50"
                           HeightRequest="50"
                           Grid.Column="0" />
                    <StackLayout Grid.Column="1" VerticalOptions="Center">
                        <Label Text="{Binding Title}" 
                               FontSize="16" 
                               FontAttributes="Bold" />
                        <Label Text="{Binding Subtitle}" 
                               FontSize="14"
                               TextColor="Gray" />
                    </StackLayout>
                    <Label Text="›" 
                           FontSize="24"
                           TextColor="Gray"
                           Grid.Column="2"
                           VerticalOptions="Center" />
                </Grid>
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

#### Horizontal ScrollView
```xml
<ScrollView Orientation="Horizontal" 
            HorizontalScrollBarVisibility="Never">
    <HorizontalStackLayout Spacing="10" Padding="10">
        <Frame CornerRadius="15" 
               WidthRequest="150" 
               HeightRequest="200"
               HasShadow="True">
            <!-- Card content -->
        </Frame>
        <!-- More cards -->
    </HorizontalStackLayout>
</ScrollView>
```

### 5. Navigation

#### Tab Bar
```xml
<TabBar>
    <Tab Title="Home" Icon="icon_home.png">
        <ShellContent ContentTemplate="{DataTemplate pages:HomePage}" />
    </Tab>
    <Tab Title="Search" Icon="icon_search.png">
        <ShellContent ContentTemplate="{DataTemplate pages:SearchPage}" />
    </Tab>
    <Tab Title="Profile" Icon="icon_profile.png">
        <ShellContent ContentTemplate="{DataTemplate pages:ProfilePage}" />
    </Tab>
</TabBar>
```

#### Bottom Sheet (Modal)
```xml
<Grid>
    <!-- Main content -->
    
    <!-- Overlay -->
    <BoxView BackgroundColor="Black"
             Opacity="0.5"
             IsVisible="{Binding IsBottomSheetVisible}"
             Grid.RowSpan="2">
        <BoxView.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding CloseBottomSheetCommand}" />
        </BoxView.GestureRecognizers>
    </BoxView>
    
    <!-- Bottom Sheet -->
    <Frame CornerRadius="20,20,0,0"
           VerticalOptions="End"
           IsVisible="{Binding IsBottomSheetVisible}"
           Padding="20">
        <StackLayout>
            <Label Text="Bottom Sheet Title" FontSize="20" FontAttributes="Bold" />
            <!-- Content -->
        </StackLayout>
    </Frame>
</Grid>
```

### 6. Badges & Chips

#### Badge
```xml
<Grid>
    <ImageButton Source="icon_notifications.png" />
    <Frame CornerRadius="10"
           BackgroundColor="Red"
           Padding="5,2"
           HasShadow="False"
           HorizontalOptions="End"
           VerticalOptions="Start"
           Margin="0,-5,-5,0">
        <Label Text="3" 
               TextColor="White" 
               FontSize="10"
               FontAttributes="Bold" />
    </Frame>
</Grid>
```

#### Chip
```xml
<Frame CornerRadius="20"
       BackgroundColor="#E3F2FD"
       Padding="15,8"
       HasShadow="False">
    <HorizontalStackLayout Spacing="5">
        <Label Text="Category" 
               TextColor="#1565C0"
               FontSize="14" />
        <Label Text="×" 
               TextColor="#1565C0"
               FontSize="16">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding RemoveCommand}" />
            </Label.GestureRecognizers>
        </Label>
    </HorizontalStackLayout>
</Frame>
```

### 7. Progress Indicators

#### Linear Progress
```xml
<ProgressBar Progress="{Binding UploadProgress}"
             ProgressColor="#1565C0"
             HeightRequest="4" />
```

#### Circular Progress
```xml
<ActivityIndicator IsRunning="{Binding IsBusy}"
                   Color="#1565C0"
                   HeightRequest="40"
                   WidthRequest="40" />
```

#### Skeleton Loader
```xml
<Frame CornerRadius="10" 
       BackgroundColor="#E0E0E0"
       IsVisible="{Binding IsLoading}">
    <Grid RowDefinitions="20,15,15" RowSpacing="10">
        <BoxView BackgroundColor="#BDBDBD" 
                 CornerRadius="5"
                 HeightRequest="20"
                 Grid.Row="0" />
        <BoxView BackgroundColor="#BDBDBD" 
                 CornerRadius="5"
                 HeightRequest="15"
                 Grid.Row="1" />
        <BoxView BackgroundColor="#BDBDBD" 
                 CornerRadius="5"
                 HeightRequest="15"
                 WidthRequest="150"
                 HorizontalOptions="Start"
                 Grid.Row="2" />
    </Grid>
</Frame>
```

### 8. Dialogs & Alerts

#### Custom Alert
```xml
<Frame BackgroundColor="#FFEBEE"
       BorderColor="#F44336"
       CornerRadius="10"
       Padding="15"
       Margin="10"
       IsVisible="{Binding HasError}">
    <Grid ColumnDefinitions="Auto,*,Auto" ColumnSpacing="10">
        <Label Text="⚠️" 
               FontSize="24"
               Grid.Column="0" />
        <Label Text="{Binding ErrorMessage}"
               TextColor="#C62828"
               VerticalOptions="Center"
               Grid.Column="1" />
        <ImageButton Source="icon_close.png"
                     BackgroundColor="Transparent"
                     Command="{Binding DismissErrorCommand}"
                     Grid.Column="2" />
    </Grid>
</Frame>
```

#### Success Toast
```xml
<Frame BackgroundColor="#4CAF50"
       CornerRadius="10"
       Padding="15"
       Margin="10"
       IsVisible="{Binding ShowSuccessToast}"
       VerticalOptions="End">
    <Label Text="✓ Success!" 
           TextColor="White"
           FontSize="16" />
</Frame>
```

### 9. Switches & Toggles

#### Custom Switch
```xml
<Grid ColumnDefinitions="*,Auto" ColumnSpacing="10">
    <Label Text="Dark Mode" 
           VerticalOptions="Center"
           Grid.Column="0" />
    <Switch IsToggled="{Binding IsDarkMode}"
            OnColor="#1565C0"
            ThumbColor="White"
            Grid.Column="1" />
</Grid>
```

### 10. Rating Component

#### Star Rating
```xml
<HorizontalStackLayout Spacing="5">
    <Label Text="⭐" FontSize="20">
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding RateCommand}" CommandParameter="1" />
        </Label.GestureRecognizers>
    </Label>
    <Label Text="⭐" FontSize="20">
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding RateCommand}" CommandParameter="2" />
        </Label.GestureRecognizers>
    </Label>
    <Label Text="⭐" FontSize="20">
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding RateCommand}" CommandParameter="3" />
        </Label.GestureRecognizers>
    </Label>
    <Label Text="⭐" FontSize="20">
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding RateCommand}" CommandParameter="4" />
        </Label.GestureRecognizers>
    </Label>
    <Label Text="⭐" FontSize="20">
        <Label.GestureRecognizers>
            <TapGestureRecognizer Command="{Binding RateCommand}" CommandParameter="5" />
        </Label.GestureRecognizers>
    </Label>
</HorizontalStackLayout>
```

## 🎨 Component Styling Tips

### 1. Consistent Spacing
Use multiples of 4 or 8 for margins and padding:
- Small: 4-8
- Medium: 12-16
- Large: 20-24

### 2. Shadow Effects
```xml
<Frame HasShadow="True">
    <!-- Or use Shadow property for more control -->
    <Frame.Shadow>
        <Shadow Brush="Black"
                Offset="0,2"
                Radius="8"
                Opacity="0.3" />
    </Frame.Shadow>
</Frame>
```

### 3. Gradient Backgrounds
```xml
<Border>
    <Border.Background>
        <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
            <GradientStop Color="#1565C0" Offset="0.0" />
            <GradientStop Color="#0D47A1" Offset="1.0" />
        </LinearGradientBrush>
    </Border.Background>
</Border>
```

---

**Pro Tip**: Create reusable styles in App.xaml for consistent component appearance across your app.
