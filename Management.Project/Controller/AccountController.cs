using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student.Management.Domain.Entities;

namespace Management.Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra nếu đã đăng nhập thì chuyển thẳng vào Admin (tuỳ chọn)
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            return View();

        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                Console.WriteLine($"Login attempt received - Email: {model?.Email}");

                if (model == null || string.IsNullOrEmpty(model.Email))
                {
                    return Json(new { success = false, message = "Email is required" });
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return Json(new { success = false, message = "User not found" });
                }

                Console.WriteLine($"User found: {user.Email}, Role: {user.Role}");

                // Đăng nhập trực tiếp bằng email và password
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

                if (!result.Succeeded)
                {
                    Console.WriteLine("Password sign in failed");
                    return Json(new { success = false, message = "Invalid credentials" });
                }

                Console.WriteLine("Login successful, getting roles...");

                // Lấy roles của user
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User roles: {string.Join(", ", roles)}");

                // Điều hướng theo role
                string redirectUrl;
                if (user.Role == "Admin" || roles.Contains("Admin"))
                {
                    redirectUrl = "/Admin/Admin";
                    Console.WriteLine("Redirecting to Admin dashboard");
                }
                else if (user.Role == "Teacher" || roles.Contains("Teacher"))
                {
                    redirectUrl = "/Teacher/Home/Index";
                    Console.WriteLine("Redirecting to Teacher dashboard");
                }
                else
                {
                    redirectUrl = "/Student/Home/Index";
                    Console.WriteLine("Redirecting to Student dashboard");
                }

                return Json(new { success = true, redirect = redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Login failed: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email))
                {
                    return Json(new { success = false, message = "Email is required" });
                }

                // Kiểm tra user đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return Json(new { success = false, message = "User already exists" });

                // Tạo user mới
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role,
                    DateCreated = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Thêm role cho user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Đăng nhập luôn sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    return Json(new
                    {
                        success = true,
                        message = "Registration successful",
                        redirect = model.Role == "Admin" ? "/Admin/Admin" : "/Home/Index"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Registration failed: " + ex.Message });
            }
        }

        [HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index", "Home");
        //}

        [HttpGet]
        public async Task<IActionResult> CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user);

                return Json(new
                {
                    isAuthenticated = true,
                    user = new { user.Email, user.Role },
                    roles
                });
            }

            return Json(new { isAuthenticated = false });
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // If already authenticated you may redirect to a dashboard instead.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Optional: return RedirectToAction("Index", "Admin");
            }

            // Reuse the existing Index view for the login page (Views/Account/Index.cshtml)
            return View("Index");
        }
    }

    // Model classes
    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
    }
}