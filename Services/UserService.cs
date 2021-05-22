
using FluentValidation;
using FluentValidation.Results;
using KissLog;
using Microsoft.AspNetCore.Identity;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SCAP.Services
{
    public class UserService<TUser> : IUserService<TUser> where TUser : Pessoa
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly INotificator Notificator;
        protected readonly ILogger Logger;

        protected readonly UserManager<Pessoa> UserManager;

        public UserService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger,
            UserManager<Pessoa> userManager)
        {
            UnitOfWork = unitOfWork;
            Notificator = notificator;
            Logger = logger;

            UserManager = userManager;
        }

        public virtual TUser Get(string id) =>
            UnitOfWork.UserRepository<TUser>().Get(id);

        public virtual TUser GetByEmail(string email) =>
            UnitOfWork.UserRepository<TUser>().GetByEmail(email);

        public virtual IEnumerable<TUser> GetAll() =>
            UnitOfWork.UserRepository<TUser>().GetAll();

        public virtual IEnumerable<TUser> GetAllAtivos() =>
            UnitOfWork.UserRepository<TUser>().GetAllAtivos();

        public virtual void Add(TUser user)
        {
            #region Validation
            if (!IsValid(new PessoaValidator<TUser>(), user))
                return;
            #endregion

            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                var createUser = UserManager.CreateAsync(user).Result;

                if (createUser.Succeeded)
                {
                    var addRole = UserManager.AddToRoleAsync(user, user.GetType().Name).Result;

                    if (!addRole.Succeeded)
                    {
                        transaction.Rollback();
                        Logger.Error(addRole.Errors);
                        Error(string.Empty, $"Ocorreu um erro ao adicionar {typeof(TUser).Name}.");
                    }
                }
                else
                {
                    Logger.Error(createUser.Errors);
                    
                    foreach (var error in createUser.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            Error("UserName", "Já existe um usuário cadastrado com essa matrícula.");
                        }
                        else if (error.Code == "DuplicateEmail")
                        {
                            Error("Email", "Já existe um usuário cadastrado com esse email.");
                        }
                        else
                        {
                            Error(string.Empty, error.Description);
                        }
                    }
                }

                if (Notificator.HasErrors())
                    return;

                UnitOfWork.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, $"Ocorreu um erro ao adicionar {typeof(TUser).Name}.");
            }
        }

        public virtual void Update(TUser user)
        {
            #region Validation
            if (!IsValid(new PessoaValidator<TUser>(), user))
                return;
            #endregion

            var updated = UserManager.UpdateAsync(user).Result;

            if (!updated.Succeeded)
            {
                Logger.Error(updated.Errors);
                Error(string.Empty, $"Ocorreu um erro ao atualizar {typeof(TUser).Name}.");
            }
        }

        public virtual void Remove(string id)
        {
            var user = UserManager.FindByIdAsync(id).Result;

            #region Validation
            if (!IsValid(new PessoaValidator<Pessoa>(), user))
                return;
            #endregion

            try
            {
                var deleted = UserManager.DeleteAsync(user).Result;

                if (!deleted.Succeeded)
                {
                    Logger.Error(deleted.Errors);
                    Error(string.Empty, $"Ocorreu um erro ao remover {typeof(TUser).Name}.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Error(string.Empty, $"Ocorreu um erro ao remover {typeof(TUser).Name}.");
            }
        }

        public virtual bool Exists(string id) =>
            UnitOfWork.UserRepository<TUser>().Exists(id);

        public string GetId(ClaimsPrincipal user) =>
            UserManager.GetUserId(user);

        public string GeneratePasswordResetToken(TUser user) =>
            UserManager.GeneratePasswordResetTokenAsync(user).Result;

        public void ResetPassword(TUser user, string token, string newPassword)
        {
            var reseted = UserManager.ResetPasswordAsync(user, token, newPassword).Result;

            if (!reseted.Succeeded)
            {
                Logger.Error(reseted.Errors);
                Error(string.Empty, "Não foi possível definir sua senha, por favor utilize a opção \"recuperar minha senha\" na página de login.");
            }
        }

        protected void Success(string key, string message, object parameters = null) =>
            Notify(NotificationType.SUCCESS, key, message, parameters);

        protected void Warning(string key, string message, object parameters = null) =>
            Notify(NotificationType.WARNING, key, message, parameters);

        protected void Error(string key, string message, object parameters = null) =>
            Notify(NotificationType.ERROR, key, message, parameters);

        protected void Notify(NotificationType type, string key, string message, object parameters = null)
        {
            if (parameters != null)
            {
                foreach (var param in parameters.GetType().GetProperties())
                {
                    message = message.Replace($":{param.Name}", param.GetValue(parameters).ToString());
                }
            }

            Notificator.Handle(new Notification(type, key, message));
        }

        protected void Notify(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notify(NotificationType.ERROR, string.Empty, error.ErrorMessage);
            }
        }

        protected virtual bool IsValid<TV, TE>(TV validator, TE entity)
            where TV : AbstractValidator<TE>
            where TE : Pessoa
        {
            if (entity == null)
            {
                Error(string.Empty, $"{typeof(TE).Name} não encontrado(a).");
                return false;
            }

            var validation = validator.Validate(entity);

            Notify(validation);

            return validation.IsValid;
        }
    }
}
