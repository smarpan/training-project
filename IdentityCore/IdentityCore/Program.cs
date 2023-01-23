using IdentityCore.DbLayer.DbServices;
using IdentityCore.DbLayer.Entity;
using IdentityCore.Services.Contracts;
using IdentityCore.Services.Implementations;
using IdentityCore.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdentityCore.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContextPool<MainContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnect")));
builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

})
    .AddEntityFrameworkStores<MainContext>();

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ValidateIssuerSigningKey = true
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateRole", policy => policy.RequireClaim("hasAccessToCreateRole"));
    options.AddPolicy("AssignRole", policy => policy.RequireClaim("hasAccessToAssignRole"));
    options.AddPolicy("RevokeRole", policy => policy.RequireClaim("hasAccessToRevokeRole"));
    options.AddPolicy("ViewRole", policy => policy.RequireClaim("hasAccessToViewRole"));
    options.AddPolicy("DeleteRole", policy => policy.RequireClaim("hasAccessToDeleteRole"));
    options.AddPolicy("UpdateUserData", policy => policy.RequireClaim("hasAccessToUpdateUserData"));
    options.AddPolicy("ViewUserData", policy => policy.RequireClaim("hasAccessToViewUserData"));
    options.AddPolicy("AddPost", policy => policy.RequireClaim("hasAccessToAddPost"));
    options.AddPolicy("ViewUserList", policy => policy.RequireClaim("hasAccessToViewUserList"));
    options.AddPolicy("ViewPostList", policy => policy.RequireClaim("hasAccessToViewPostList"));
    options.AddPolicy("ViewUserClaimsAndRoles", policy => policy.RequireClaim("hasAccessToViewUserClaimsAndRoles"));
    options.AddPolicy("AddClaimToUser", policy => policy.RequireClaim("hasAccessToAddClaimToUser"));
    options.AddPolicy("RemoveClaimFromUser", policy => policy.RequireClaim("hasAccessToRemoveClaimFromUser"));

});
 

builder.Services.AddScoped<IAccountContract, AccountImplementation>();
builder.Services.AddScoped<IRoleContract, RoleImplementation>();
builder.Services.AddScoped<IUserContract, UserImplementation>();
builder.Services.AddScoped<IPostContract, PostImplementation>();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddCors(options => options.AddPolicy("PolicyToApp", app =>
{
         app.WithOrigins("http://localhost:4200") // the Angular app url
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();

}));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("PolicyToApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/api/Chat");
app.Run();
