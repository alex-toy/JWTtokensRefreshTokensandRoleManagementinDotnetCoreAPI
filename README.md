# JWT tokens, Refresh tokens and role management in dot net core API

This project is about creating dotnet 6 web apis with jwt authentication, refresh tokens and role-base authorization in .net 6. We will implement login, signup and change password module with the help of asp.net core identity. We will implement the functionality of refresh tokens in dotnet core 6 apis. So that we can maintain login sessions for longer period of time.
We will also implement **Chain of Responsability** design pattern to handle password change requests.


## Package Nuget
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Authentication.JwtBearer
```

## Run the App

- register admin
<img src="/pictures/register_admin.png" title="register admin"  width="900">

- register user
<img src="/pictures/register_user.png" title="register user"  width="900">

- login as user
<img src="/pictures/login_user.png" title="login as user"  width="900">

- login as user with wrong password
<img src="/pictures/login_user2.png" title="login as user with wrong password"  width="900">

- get protected data without authentication
<img src="/pictures/protected_data.png" title="get protected data without authentication"  width="900">

- get protected data with authentication
<img src="/pictures/protected_data2.png" title="get protected data with authentication"  width="900">

- get admin protected data logged in as simple user
<img src="/pictures/protected_data3.png" title="get admin protected data logged in as user"  width="900">

- get admin protected data logged in as admin
<img src="/pictures/protected_data4.png" title="get admin protected data logged in as admin"  width="900">

- change password ok
<img src="/pictures/change_pwd.png" title="change password ok"  width="900">

- change password nok
<img src="/pictures/change_pwd2.png" title="change password nok"  width="900">

