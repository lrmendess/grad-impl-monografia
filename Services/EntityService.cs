using FluentValidation;
using FluentValidation.Results;
using KissLog;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace SCAP.Services
{
    public abstract class EntityService<TEntity> : IEntityService<TEntity> where TEntity : Entity
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly INotificator Notificator;
        protected readonly ILogger Logger;

        public EntityService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger)
        {
            UnitOfWork = unitOfWork;
            Notificator = notificator;
            Logger = logger;
        }

        public virtual TEntity Get(Guid id) =>
            UnitOfWork.Repository<TEntity>().Get(id);

        public virtual IEnumerable<TEntity> GetAll() =>
            UnitOfWork.Repository<TEntity>().GetAll();

        public virtual TEntity Add(TEntity entity)
        {
            try
            {
                TEntity created = UnitOfWork.Repository<TEntity>().Add(entity);
                UnitOfWork.SaveChanges();
                
                return created;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Error(string.Empty, $"Ocorreu um erro ao adicionar {typeof(TEntity).Name}.");
                
                return null;
            }
        }

        public virtual void Remove(Guid id)
        {
            try
            {
                UnitOfWork.Repository<TEntity>().Remove(id);
                UnitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Error(string.Empty, $"Ocorreu um erro ao remover {typeof(TEntity).Name}.");
            }
        }

        public virtual TEntity Update(TEntity entity)
        {
            try
            {
                TEntity updated = UnitOfWork.Repository<TEntity>().Update(entity);
                UnitOfWork.SaveChanges();
                
                return updated;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Error(string.Empty, $"Ocorreu um erro ao atualizar {typeof(TEntity).Name}.");

                return null;
            }
        }

        public virtual bool Exists(Guid id) =>
            UnitOfWork.Repository<TEntity>().Exists(id);

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
            where TE : Entity
        {
            if (entity == null)
            {
                Error(string.Empty, $"{typeof(TE).Name} não encontrado.");
                return false;
            }

            var validation = validator.Validate(entity);

            Notify(validation);
            
            return validation.IsValid;
        }
    }
}
