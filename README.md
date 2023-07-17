##TIC-WIN1 CSHARP

Install Entity Framework CLI:

``dotnet tool install --global dotnet-ef``

Install the dependencies from tic_win1.csproj:

``dotnet restore``

Launch the API in watch mode:

``dotnet watch``

The SWAGGER documentation can then be accessed [here](http://localhost:4242/swagger)

A sample admin is available for authentication with the credentials:
> email: realadmin@admin.com
> password: string

For step 7, a sample user (id n°5) is also available with the credentials:
> email: test@test.com
> password: string

The token needs to be passed into swagger in the following format: "bearer {token}".

You can try to delete user 3 with the admin account and it will succeed.
You can try to delete user 4 with the user 5 account and it will fail.
Then try deleting user 5 with the user 5 account and it will succeed.




