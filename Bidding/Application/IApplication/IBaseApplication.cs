using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface IBaseApplication<T>
    {
        #region Methods
        /// <summary>
        /// Add adds new object in database and returns provided ObjectId.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(T entity);

        /// <summary>
        /// Activate activate/deactive provided record and returns true if action was successfull.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Activate(T entity);

        /// <summary>
        /// Get fetch and returns queried item.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Get(T entity);

        /// <summary>
        /// GetAll fetch and returns queried list of items.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>

        Task<List<T>> GetList(T entity);

        /// <summary>
        /// Update updates existing object in database and returns true if action was successfull.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Update(T entity);

        Task<AllResponse<T>> GetAll(AllRequest<T> entity);
        #endregion
    }

}
