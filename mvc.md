# ASP.NET MVC Cheatsheet

---

## 🗂️ Project Structure

```
MyApp/
├── Controllers/         # Request handlers
├── Models/              # Data models & ViewModels
├── Views/               # Razor .cshtml files
│   ├── Shared/          # _Layout.cshtml, _ViewStart, partials
│   └── {Controller}/    # Views per controller
├── wwwroot/             # Static files (css, js, images)
├── Data/                # DbContext & migrations
├── Services/            # Business logic
├── Program.cs           # App entry point & config
├── appsettings.json     # Configuration
└── MyApp.csproj         # Project file
```

---

## ⚙️ Program.cs (Minimal Hosting)

```csharp
var builder = WebApplication.CreateBuilder(args);

// --- Register Services ---
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IMyService, MyService>();

var app = builder.Build();

// --- Middleware Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// --- Routes ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

---

## 🔀 Routing

### Convention-Based (Program.cs)
```csharp
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
```

### Attribute Routing
```csharp
[Route("products")]
public class ProductController : Controller
{
    [HttpGet("{id}")]          // GET /products/5
    public IActionResult Get(int id) { ... }

    [HttpPost("create")]       // POST /products/create
    public IActionResult Create(Product p) { ... }
}
```

### Route Constraints
```
{id:int}          // int only
{slug:alpha}      // letters only
{id:min(1)}       // minimum value
{name:maxlength(20)}
{id:range(1,100)}
```

---

## 🎮 Controllers

```csharp
public class HomeController : Controller
{
    private readonly IMyService _svc;

    // Constructor Injection
    public HomeController(IMyService svc) => _svc = svc;

    // Return a View
    public IActionResult Index() => View();

    // View with model
    public IActionResult Details(int id)
    {
        var item = _svc.GetById(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // POST with anti-forgery
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MyModel model)
    {
        if (!ModelState.IsValid) return View(model);
        _svc.Add(model);
        return RedirectToAction(nameof(Index));
    }
}
```

### Common Return Types

| Method | Description |
|---|---|
| `View()` | Render a Razor view |
| `View(model)` | Render view with model |
| `RedirectToAction("Action")` | 302 redirect |
| `RedirectToAction("Action", "Ctrl")` | Redirect to another controller |
| `Json(obj)` | Return JSON |
| `NotFound()` | 404 |
| `BadRequest()` | 400 |
| `Ok(obj)` | 200 with data |
| `Content("text")` | Plain text |
| `File(bytes, "type")` | File download |
| `PartialView("_Partial", model)` | Render partial |

### Passing Data to Views

```csharp
// 1. Strongly-typed model (preferred)
return View(myModel);

// 2. ViewBag (dynamic)
ViewBag.Title = "Home";

// 3. ViewData (dictionary)
ViewData["Message"] = "Hello";

// 4. TempData (survives redirects)
TempData["Success"] = "Item created!";
```

---

## 📦 Models & Validation

```csharp
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Product Name")]
    public string Name { get; set; }

    [Range(0.01, 99999.99)]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [EmailAddress]
    public string ContactEmail { get; set; }

    [Url]
    public string Website { get; set; }

    [RegularExpression(@"^[A-Z]{2}\d{4}$")]
    public string Code { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
```

### Common Annotations

| Attribute | Purpose |
|---|---|
| `[Required]` | Field must have a value |
| `[StringLength(max)]` | Max (and optional min) length |
| `[Range(min, max)]` | Numeric range |
| `[EmailAddress]` | Email format |
| `[Phone]` | Phone format |
| `[Url]` | URL format |
| `[Compare("Other")]` | Must match another field |
| `[RegularExpression]` | Regex pattern |
| `[Display(Name = "")]` | Label text |
| `[DataType(DataType.X)]` | Hint for rendering |
| `[Key]` | Primary key (EF) |
| `[NotMapped]` | Exclude from DB (EF) |
| `[ForeignKey("NavProp")]` | Foreign key (EF) |

---

## 🖼️ Views & Razor Syntax

### Razor Basics
```html
<!-- Output expression -->
<p>Hello, @Model.Name</p>

<!-- Code block -->
@{
    var greeting = "Hello";
    ViewData["Title"] = "Home";
}

<!-- Conditionals -->
@if (Model.Items.Any())
{
    <ul>
    @foreach (var item in Model.Items)
    {
        <li>@item.Name - @item.Price.ToString("C")</li>
    }
    </ul>
}
else
{
    <p>No items found.</p>
}

<!-- Raw HTML output (unencoded) -->
@Html.Raw(Model.HtmlContent)

<!-- Comments -->
@* This is a Razor comment *@
```

### Layout & Sections

**_Layout.cshtml**
```html
<!DOCTYPE html>
<html>
<head>
    <title>@ViewData["Title"] - MyApp</title>
    @RenderSection("Styles", required: false)
</head>
<body>
    <nav><!-- nav here --></nav>
    <main>@RenderBody()</main>
    @RenderSection("Scripts", required: false)
</body>
</html>
```

**Child View**
```html
@model MyViewModel
@{ ViewData["Title"] = "Details"; }

<h1>@Model.Title</h1>

@section Scripts {
    <script src="~/js/details.js"></script>
}
```

### Partial Views
```html
<partial name="_ProductCard" model="product" />
@await Html.PartialAsync("_ProductCard", product)
```

---

## 🏷️ Tag Helpers

```html
<!-- Links -->
<a asp-controller="Home" asp-action="Index">Home</a>
<a asp-controller="Product" asp-action="Details" asp-route-id="5">View</a>

<!-- Forms -->
<form asp-controller="Product" asp-action="Create" method="post">
    <label asp-for="Name"></label>
    <input asp-for="Name" class="form-control" />
    <span asp-validation-for="Name" class="text-danger"></span>

    <select asp-for="CategoryId"
            asp-items="@(new SelectList(ViewBag.Categories, "Id", "Name"))">
    </select>
    <button type="submit">Save</button>
</form>

<!-- Validation Summary -->
<div asp-validation-summary="All"></div>

<!-- Image with cache-busting -->
<img src="~/images/logo.png" asp-append-version="true" />
```

---

## 🗃️ Entity Framework Core

### DbContext
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId);
        });
    }
}
```

### Common LINQ Queries
```csharp
// Get all
var items = await _db.Products.ToListAsync();

// Find by ID
var item = await _db.Products.FindAsync(id);

// Filter + sort
var cheap = await _db.Products
    .Where(p => p.Price < 50)
    .OrderBy(p => p.Name)
    .ToListAsync();

// Include related data
var products = await _db.Products.Include(p => p.Category).ToListAsync();

// Pagination
var page = await _db.Products
    .Skip((pageNum - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Add / Update / Delete
_db.Products.Add(product);
_db.Products.Update(product);
_db.Products.Remove(product);
await _db.SaveChangesAsync();

// Exists / Count
bool exists = await _db.Products.AnyAsync(p => p.Name == "X");
int count  = await _db.Products.CountAsync();
```

### EF CLI Commands
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet ef migrations remove
dotnet ef migrations script
dotnet ef dbcontext scaffold "ConnString" Microsoft.EntityFrameworkCore.SqlServer
```

---

## 💉 Dependency Injection

```csharp
// Lifetimes
builder.Services.AddTransient<IService, Service>();   // New every time
builder.Services.AddScoped<IService, Service>();      // Once per request
builder.Services.AddSingleton<IService, Service>();   // Once per app

// Inject in Controller
public MyController(IService svc) => _svc = svc;

// Inject in View
@inject IMyService MyService
```

---

## 🔐 Authentication & Authorization

### Cookie Auth Setup
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Account/Login";
        opt.AccessDeniedPath = "/Account/AccessDenied";
        opt.ExpireTimeSpan = TimeSpan.FromHours(1);
    });
```

### Sign In / Out
```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, "Admin")
};
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

await HttpContext.SignOutAsync();
```

### Authorize Attribute
```csharp
[Authorize]                          // Logged-in users only
[Authorize(Roles = "Admin")]         // Admin role only
[Authorize(Policy = "MinAge18")]     // Custom policy
[AllowAnonymous]                     // Override controller-level [Authorize]
```

---

## 🧩 Middleware

```csharp
// Inline
app.Use(async (context, next) =>
{
    // before
    await next();
    // after
});

// Custom class
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
        await _next(context);
    }
}
// app.UseMiddleware<LoggingMiddleware>();
```

### Pipeline Order (matters!)
```
UseExceptionHandler → UseHsts → UseHttpsRedirection →
UseStaticFiles → UseRouting → UseCors →
UseAuthentication → UseAuthorization → MapControllers
```

---

## 📋 Configuration

```json
{
  "ConnectionStrings": { "Default": "Server=.;Database=MyDb;..." },
  "AppSettings": { "PageSize": 20 }
}
```
```csharp
var cs = builder.Configuration.GetConnectionString("Default");

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// Inject: IOptions<AppSettings>
```

---

## 🔗 Common Patterns

### ViewModel
```csharp
public class ProductListVM
{
    public List<Product> Products { get; set; }
    public string SearchTerm { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
```

### PRG (Post-Redirect-Get)
```csharp
[HttpPost]
public IActionResult Create(Product model)
{
    if (!ModelState.IsValid) return View(model);
    _db.Products.Add(model);
    _db.SaveChanges();
    TempData["Success"] = "Created!";
    return RedirectToAction(nameof(Index));
}
```

---

## 🛠️ CLI Commands

```bash
dotnet new mvc -n MyApp        # New MVC project
dotnet watch run               # Run with hot reload
dotnet add package <name>      # Add NuGet package
dotnet build                   # Build
dotnet publish -c Release      # Publish
dotnet list package            # List packages
dotnet test                    # Run tests
```
