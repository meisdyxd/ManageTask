using ManageTask.Application.Abstractions.Data;
using ManageTask.Application.Extensions;
using ManageTask.Application.Models.Pagination;
using ManageTask.Contracts.ApiContracts;
using ManageTask.Domain;
using ManageTask.Infrastructure.Data.Contexts;
using ManageTask.Infrastructure.Data.Entities;
using ManageTask.Infrastructure.Data.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultSharp.Core;
using ResultSharp.Errors;
using System.Threading;

namespace ManageTask.Infrastructure.Data.Repositories
{
    public class UserRepository(DataContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly ILogger<UserRepository> logger = logger;
        private readonly DataContext context = context;

        public async Task<Result<User>> AddAsync(User user, CancellationToken cancellationToken)
        {
            logger.LogInformation("Добавление пользователя с ID: {UserId}", user.Id);
            var entity = GetUserEntity(user);
            await context.Users.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Пользователь успешно добавление с ID: {UserId}", entity.Id);
            return entity.Map();
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Удаление пользователя с ID: {UserId}", id);
            var entity = await context.Users.FindAsync(keyValues: [id], cancellationToken: cancellationToken);

            if(entity is null)
            {
                logger.LogWarning("Пользователь с ID: {UserId}, для удаления не найден", id);
                return Error.NotFound($"Пользователь с ID: {id} не найден");
            }

            context.Users.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Пользователь с ID: {UserId} удалён", id);
            return Result.Success();
        }

        public Result<IQueryable<User>> GetAll()
        {
            logger.LogInformation($"Получение пользователей");
            var users = context.Users.AsQueryable();
            return Result<IQueryable<User>>.Success(users.Select(t => new User
            {
                Id = t.Id,
                Name = t.Name,
                Password = t.Password,
                Email = t.Email,
                Role = t.Role
            }));
        }

        public async Task<Result<User>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение пользователя с ID: {UserId}", id);

            var entity = await context.Users.FindAsync(keyValues: [id], cancellationToken: cancellationToken);
            if (entity is null)
            {
                logger.LogWarning("Пользователь с ID: {UserId}, для получения не найден", id);
                return Error.NotFound($"Пользователь с ID: {id} не найден");
            }

            logger.LogInformation("Пользователь с ID: {UserId} получен", id);
            return entity.Map();
        }
        public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            logger.LogInformation("Получение пользователя с почтой: {Email}", email);

            var entity = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (entity is null)
            {
                logger.LogWarning("Пользователь с почтой: {Email}, для получения не найден", email);
                return Error.NotFound($"Пользователь с почтой: {email} не найден");
            }

            logger.LogInformation("Пользователь с почтой: {Email} получен", email);
            return entity.Map();
        }

        public async Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            logger.LogInformation("Обновление пользователя с ID: {UserId}", user.Id);

            var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
            if(userEntity is null)
            {
                return Error.BadRequest($"Пользователь с ID: {user.Id} не найден");
            }
            userEntity.Name = user.Name;
            userEntity.Role = user.Role;
            context.Users.Update(userEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Пользователь с ID: {UserId} успешно обновлён", user.Id);
            return userEntity.Map();
        }
        public async Task<Result<User>> GetAdminAsync(Guid id, CancellationToken cancellationToken)
        {
            var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (userEntity is null)
            {
                return Error.BadRequest($"Пользователь с ID: {id} не найден");
            }
            userEntity.Role = Role.Admin;
            context.Users.Update(userEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Пользователь с ID: {UserId} успешно обновлён", id);
            return userEntity.Map();
        }
        public UserEntity GetUserEntity(User user)
        {
            var entity = user.Map();
            return entity;
        }
    }
}
