using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.Microservices.Core;
using Calabonga.Microservices.Core.Exceptions;
using Calabonga.Microservices.Core.QueryParams;
using Calabonga.Microservices.Core.Validators;
using Calabonga.OperationResultsCore;
using Calabonga.PredicatesBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace Calabonga.UnitOfWork.Controllers.Controllers
{
    /// <summary>
    /// Represent read only operations controller for entity. It's not required special ViewModels and other things
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TQueryParams"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ReadOnlyController<TEntity, TViewModel, TQueryParams>
        : UnitOfWorkController
        where TEntity : Identity
        where TViewModel : ViewModelBase
        where TQueryParams : PagedListQueryParams
    {
        #region Fields

        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;

        #endregion

        /// <inheritdoc />
        protected ReadOnlyController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork)
        {
            CurrentMapper = mapper;
            Repository = unitOfWork.GetRepository<TEntity>();
        }

        /// <summary>
        /// Active AutoMapper instance
        /// </summary>
        protected IMapper CurrentMapper { get; }

        /// <summary>
        /// Current Entity repository
        /// </summary>
        protected IRepository<TEntity> Repository { get; }

        /// <summary>
        /// If you need to include something you need override this method
        /// </summary>
        /// <returns></returns>
        protected virtual Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> GetIncludes()
        {
            return null;
        }

        #region GetByIdAsync

        /// <summary>
        /// Returns entity from repository by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[action]/{id:guid}")]
        [ProducesResponseType(200)]
        public virtual async Task<ActionResult<OperationResult<TViewModel>>> GetById(Guid id)
        {
            var operation = OperationResult.CreateResult<TViewModel>();
            var includes = GetIncludes();
            var entity = await Repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id, include: includes);
            if (entity == null)
            {
                operation.AddError(new MicroserviceNotFoundException());
                return OperationResultBeforeReturn(operation);
            }

            var accessRights = ValidateUserAccessRights(entity);
            if (!accessRights.IsOk)
            {
                operation.AddError(new MicroserviceUnauthorizedException(accessRights.ToString()));
                return OperationResultBeforeReturn(operation);
            }

            var mapped = CurrentMapper.Map<TEntity, TViewModel>(entity);
            operation.Result = mapped;
            return OperationResultBeforeReturn(operation);
        }

        #endregion

        #region GetPaged

        /// <summary>
        /// Summary description will be replaced by IOperationFilter (swagger)
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="disabledDefaultIncludes"></param>
        /// <returns></returns>
        /// <response code="404">Unauthorized access detected or parameters are not valid</response>
        [HttpGet("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public virtual ActionResult<OperationResult<IPagedList<TViewModel>>> GetPaged([FromQuery] TQueryParams queryParams, bool disabledDefaultIncludes = false)
        {
            var operation = OperationResult.CreateResult<IPagedList<TViewModel>>();
            var validationResult = ValidateQueryParams(queryParams);
            if (!validationResult.IsOk)
            {
                operation.AddError(validationResult.ToString()).AddData(validationResult.Errors);
                return OperationResultBeforeReturn(operation);
            }

            var properName = GetPropertyNameForOrderBy();
            if (!string.IsNullOrEmpty(properName))
            {
                _orderBy = GetOrderBy(properName, queryParams.SortDirection.ToString());
            }
            else
            {
            }

            var includes = disabledDefaultIncludes ? null : GetIncludes();
            var predicate = PredicateBuilder.True<TEntity>();
            predicate = FilterItems(predicate, queryParams);
            var pagedList = Repository.GetPagedList(predicate: predicate, orderBy: _orderBy, include: includes, pageIndex: queryParams.PageIndex, pageSize: queryParams.PageSize);
            if (pagedList == null)
            {
                operation.Result = PagedList.Empty<TViewModel>();
                return OperationResultBeforeReturn(operation);
            }

            if (pagedList.PageIndex >= pagedList.TotalPages)
            {
                pagedList = Repository.GetPagedList(predicate: predicate, orderBy: _orderBy, include: includes, pageIndex: 0, pageSize: queryParams.PageSize);
            }

            foreach (var item in pagedList.Items)
            {
                var accessRights = ValidateUserAccessRights(item);
                if (!accessRights.IsOk)
                {
                    operation.Result = CurrentMapper.Map<IPagedList<TViewModel>>(pagedList);
                    operation.AddInfo($"$_TOTAL_COUNT_$: {pagedList.TotalCount}");
                    return OperationResultBeforeReturn(operation);
                }
            }
            var mapped = CurrentMapper.Map<IPagedList<TViewModel>>(pagedList);
            operation.Result = mapped;
            operation.AddInfo($"$_TOTAL_COUNT_$: {pagedList.TotalCount}");
            return OperationResultBeforeReturn(operation);
        }

        #endregion

        #region Abstract and Virtual

        private static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderBy(string orderColumn, string orderType)
        {
            var typeQueryable = typeof(IQueryable<TEntity>);
            var argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            var props = orderColumn.Split('.');
            var type = typeof(TEntity);
            var arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var lambda = Expression.Lambda(expr, arg);
            var methodName = orderType == "Ascending" ? "OrderBy" : "OrderByDescending";

            var resultExp = Expression.Call(typeof(Queryable), methodName, new[] { typeof(TEntity), type },
                outerExpression.Body, Expression.Quote(lambda));
            var finalLambda = Expression.Lambda(resultExp, argQueryable);
            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }

        /// <summary>
        /// Return default predicate for filtering PagedList result
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, bool>> FilterItems(Expression<Func<TEntity, bool>> predicate, TQueryParams queryParams)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// PropertyName for ordering by for enable pagination. You can override default behavior.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPropertyNameForOrderBy()
        {
            return string.Empty;
        }

        /// <summary>
        /// Validate current user roles and access rights
        /// </summary>
        /// <param name="queryParams"></param>
        protected virtual PermissionValidationResult ValidateQueryParams(TQueryParams queryParams)
        {
            if (queryParams.PageSize <= 1)
            {
                return new InvalidQueryParamsValidationResult(string.Format(AppContracts.Exceptions.InvalidOperationException, "Page size should be mode than 0"));
            }
            return new PermissionValidationResult();
        }

        /// <summary>
        /// Validate current user roles and access rights
        /// </summary>
        /// <param name="entity"></param>
        protected virtual PermissionValidationResult ValidateUserAccessRights(TEntity entity)
        {
            return new PermissionValidationResult();
        }
    }
}