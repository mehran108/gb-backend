namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IBaseInfrastructuree<T>
    {
        #region Interface Declarations
        /// <summary>
        /// Add adds new object in database and returns provided ObjectId.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Add(T entity);
        /// <summary>
        /// Update calls ExternalAuctionInfrastructure to update the external Acution
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Update(T entity);


        /// <summary>
        /// Activate activate/deactive provided record and returns true if action was successfull.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Activate(T entity);

        /// <summary>
        /// Get fetch and returns queried item from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Get(T entity);

        /// <summary>
        /// GetAll fetch and returns queried list of items with specific fields from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<List<T>> GetList(T entity);
        #endregion
    }
}
