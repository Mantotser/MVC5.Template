﻿using AutoMapper;
using System;
using Template.Components.Logging;
using Template.Objects;

namespace Template.Data.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private IEntityLogger logger;
        private AContext context;
        private Boolean disposed;

        public UnitOfWork(AContext context, IEntityLogger logger = null)
        {
            this.context = context;
            this.logger = logger;
        }

        public IRepository<TModel> Repository<TModel>()
            where TModel : BaseModel
        {
            return context.Repository<TModel>();
        }
        public TModel ToModel<TView, TModel>(TView view)
            where TView : BaseView
            where TModel : BaseModel
        {
            return Mapper.Map<TView, TModel>(view);
        }
        public TView ToView<TModel, TView>(TModel model)
            where TModel : BaseModel
            where TView : BaseView
        {
            return Mapper.Map<TModel, TView>(model);
        }

        public void Rollback()
        {
            context.Dispose();
            context = new Context();
        }
        public void Commit()
        {
            if (logger != null)
                logger.Log(context.ChangeTracker.Entries());

            context.SaveChanges();

            if (logger != null)
                logger.Save();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed) return;

            if (logger != null) logger.Dispose();
            context.Dispose();
            context = null;
            logger = null;

            disposed = true;
        }
    }
}
