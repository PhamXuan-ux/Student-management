$(document).ready(function () {
    // Login form submission
    $("#loginForm").submit(function (e) {
        e.preventDefault();

        var email = $("#loginEmail").val();
        var password = $("#loginPassword").val();

        console.log('Attempting login with:', email);

        // Tạo object đúng format
        var loginData = {
            Email: email,  // Phải viết hoa chữ E
            Password: password  // Phải viết hoa chữ P
        };

        console.log('Sending login data:', loginData);

        $.ajax({
            url: '/Account/Login',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(loginData),
            success: function (result) {
                console.log('Login response:', result);

                if (result.success) {
                    console.log('Redirecting to:', result.redirect);
                    window.location.href = result.redirect;
                } else {
                    alert('Login failed: ' + result.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('Login error:', error);
                console.log('Response text:', xhr.responseText);
                alert('Login failed: ' + error);
            }
        });
    });

    // Register form submission
    $("#registerForm").submit(function (e) {
        e.preventDefault();
        var data = {
            Email: $("#regEmail").val(),
            Password: $("#regPassword").val(),
            FullName: $("#regName").val(),
            Role: "Student"
        };

        console.log('Sending register data:', data);

        $.ajax({
            url: '/Account/Register',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                alert(res.message);
                if (res.success) {
                    $("#signIn").click();
                }
            },
            error: function (xhr, status, error) {
                console.error('Register error:', error);
                alert('Registration failed: ' + error);
            }
        });
    });

    // Chuyển form
    $("#signUp").click(function () {
        $("#container").addClass("right-panel-active");
    });
    $("#signIn").click(function () {
        $("#container").removeClass("right-panel-active");
    });

    // Debug: Test kết nối
    console.log("Account page loaded");
});