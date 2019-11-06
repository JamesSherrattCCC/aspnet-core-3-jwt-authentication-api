# aspnet-core-3-jwt-authentication-api

ASP.NET Core 3.0 - JWT Authentication API

For documentation and instructions check out https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api


Getting an authentication and refresh token
----------------------------------------------

Request type: POST

URL: http://localhost:4000/Users/authenticate

json body:

```json
{
	"username": "test",
	"password": "test"
}
```

returns (example):

```json
{
    "id": 1,
    "firstName": "Test",
    "lastName": "User",
    "username": "test",
    "password": null,
    "authenticateToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1NzMwMzg0MjgsImV4cCI6MTU3MzAzODQ0OCwiaWF0IjoxNTczMDM4NDI4fQ.insGao8l53c3m8cOe2yTCkH76Y9el0jGefzVRvQZwAY",
    "refreshToken": "GACCCJMlSXYu/DJ6+80N66S6Q8P6fVM63TrIR0k1Pgg="
}
```

Getting data
-----------------

Request type: GET

URL: http://localhost:4000/users

Header: Authorization: Bearer <token>

returns (example):

```json
[
    {
        "id": 1,
        "firstName": "Test",
        "lastName": "User",
        "username": "test",
        "password": null,
        "authenticateToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1NzMwMzg0NjUsImV4cCI6MTU3MzAzODQ4NSwiaWF0IjoxNTczMDM4NDY1fQ.l_qZtHCmJzG2U80kSnbdy9MgrmfMYoofFJ6YPcsTX4I",
        "refreshToken": "1lgIivhOn9XhoikhGsyQ4Im962iqyI50WrkyNScKzAM="
    }
]
```

Refreshing the token
---------------------

Request type: POST

URL: http://localhost:4000/users/refresh

json body (example):

```json
{ 
    "authenticateToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1NzMwMzg0MjgsImV4cCI6MTU3MzAzODQ0OCwiaWF0IjoxNTczMDM4NDI4fQ.insGao8l53c3m8cOe2yTCkH76Y9el0jGefzVRvQZwAY",
    "refreshToken": "GACCCJMlSXYu/DJ6+80N66S6Q8P6fVM63TrIR0k1Pgg="
}
```

returns (example):

```json
{
    "id": 1,
    "firstName": "Test",
    "lastName": "User",
    "username": "test",
    "password": null,
    "authenticateToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJuYmYiOjE1NzMwMzg0NjUsImV4cCI6MTU3MzAzODQ4NSwiaWF0IjoxNTczMDM4NDY1fQ.l_qZtHCmJzG2U80kSnbdy9MgrmfMYoofFJ6YPcsTX4I",
    "refreshToken": "1lgIivhOn9XhoikhGsyQ4Im962iqyI50WrkyNScKzAM="
}
```