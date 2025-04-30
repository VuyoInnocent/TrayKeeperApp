using CommunityToolkit.Maui;
using SQLite;
using TrayKeeper.BL;
using TrayKeeper.BL.Interfaces;
using TrayKeeper.DL;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;
using TrayKeeper.ViewModel;
using TrayKeeper.Views;

namespace TrayKeeper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Set up the SQLite database connection
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TrayKepeer.db3");
            builder.Services.AddSingleton(new SQLiteAsyncConnection(dbPath));


            // Register dependency injection
            //repository
            builder.Services.AddSingleton<IGenericRepository<User>, GenericRepository<User>>();
            builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
            builder.Services.AddSingleton<ILogInRepository, LogInRepository>();
            builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
            builder.Services.AddSingleton<ISalesRepository, SaleRepository>();

            //services
            builder.Services.AddSingleton<IInventoryService, InventoryService>();
            builder.Services.AddSingleton<ILogInService, LoginService>();
            builder.Services.AddSingleton<IOrderService, OrderService>();
            builder.Services.AddSingleton<ISalesService, SalesService>();

            //ViewModels
            builder.Services.AddTransient<OrdersHistoryViewModel>();
            builder.Services.AddTransient<InventoryViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<OrderViewModel>();
            builder.Services.AddTransient<SalesViewModel>();
            builder.Services.AddTransient<AppViewModel>();

            //Views
            builder.Services.AddTransient<OrdersHistoryPage>();
            builder.Services.AddTransient<InventoryPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<OrderPage>();
            builder.Services.AddTransient<SalesPage>();

            return builder.Build();
        }
    }
}
